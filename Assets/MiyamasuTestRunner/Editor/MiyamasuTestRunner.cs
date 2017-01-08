using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using Diag = System.Diagnostics;
using UnityEngine;
using UnityEditor;

/**
	MiyamasuTestRunner
		this testRunner is only test runner which contains "WaitUntil" method.
		"WaitUntil" method can wait Async code execution until specified sec passed.

		This can be replacable when Unity adopts C# 4.0 by using "async" and "await" keyword.
*/
namespace Miyamasu {
	public class MiyamasuTestRunner {
		public MiyamasuTestRunner () {}

		private class TypeAndMedhods {
			public Type type;

			public bool hasTests = false;
			
			public MethodInfo[] asyncMethodInfos;
			public MethodInfo setupMethodInfo = null;
			public MethodInfo teardownMethodInfo = null;

			public TypeAndMedhods (Type t) {
				var testMethods = t.GetMethods()
					.Where(methods => 0 < methods.GetCustomAttributes(typeof(MTestAttribute), false).Length)
					.ToArray();

				if (!testMethods.Any()) return; 
				this.hasTests = true;

				/*
					hold type.
				*/
				this.type = t;

				/*
					collect tests.
				*/
				this.asyncMethodInfos = testMethods;
				
				/*
					collect setup and teardown.
				*/
				this.setupMethodInfo = t.GetMethods().Where(methods => 0 < methods.GetCustomAttributes(typeof(MSetupAttribute), false).Length).FirstOrDefault();
				this.teardownMethodInfo = t.GetMethods().Where(methods => 0 < methods.GetCustomAttributes(typeof(MTeardownAttribute), false).Length).FirstOrDefault();
			}
		}

		public object lockObj = new object();

		public IEnumerator RunTestsOnEditorMainThread (Action done) {
			var typeAndMethodInfos = Assembly.GetExecutingAssembly().GetTypes()
				.Select(t => new TypeAndMedhods(t))
				.Where(tAndMInfo => tAndMInfo.hasTests)
				.ToArray();

			
			if (!typeAndMethodInfos.Any()) {
				TestLogger.Log("no tests found. please set \"[MTest]\" attribute to method.", true);
				yield break;
			}

			var passed = 0;
			var failed = 0;

			TestLogger.Log("tests started.", true);
			
			var totalMethodCount = typeAndMethodInfos.Count() -1;
			
			// generate waitingThread for waiting asynchronous(=running on MainThread or other thread) ops on Not-MainThread.
			Thread thread = null;
			thread = new Thread(
				() => {
					var count = 0;
					foreach (var typeAndMethodInfo in typeAndMethodInfos) {
						var instance = Activator.CreateInstance(typeAndMethodInfo.type);
						
						TestLogger.Log("start tests of class:" + typeAndMethodInfo.type + ". classes:" + count + " of " + totalMethodCount, true);
						foreach (var methodInfo in typeAndMethodInfo.asyncMethodInfos) {
							var methodName = methodInfo.Name;
							TestLogger.Log("start methodName:" + methodName, true);

							// setup.
							try {
								if (typeAndMethodInfo.setupMethodInfo != null) {
									typeAndMethodInfo.setupMethodInfo.Invoke(instance, null);
								}
							} catch (Exception e) {
								failed++;
								LogTestFailed(e, methodName);
								continue;
							}
							
							/*
								run test.
							*/
							try {
								methodInfo.Invoke(instance, null);
								passed++;
							} catch (Exception e) {
								failed++;
								LogTestFailed(e);
							}

							// teardown.
							try {
								if (typeAndMethodInfo.teardownMethodInfo != null) {
									typeAndMethodInfo.teardownMethodInfo.Invoke(instance, null);
								}
							} catch (Exception e) {
								LogTestFailed(e, methodName);
							}
						}

						TestLogger.Log("done tests of class:" + typeAndMethodInfo.type + ". classes:" + count + " of " + totalMethodCount, true);
						
						count++;
					}

					thread.Abort();
				}
			);
			

			try {
				thread.Start();
			} catch (Exception e) {
				TestLogger.Log("Miyamasu TestRunner error:" + e);
			}
			
			yield return null;

			while (true) {
				if (!thread.IsAlive) break; 
				yield return null;
			}
			
			TestLogger.Log("tests end. passed:" + passed + " failed:" + failed, true);
			TestLogger.LogEnd();
			done();
		}

		private void LogTestFailed (Exception e, string subLocation=null) {
			var location = string.Empty;
			var errorStackLines = e.ToString().Split('\n');
			
			for (var i = 0; i < errorStackLines.Length; i++) {
				var line = errorStackLines[i];
				
				if (line.StartsWith("  at Miyamasu.MiyamasuTestRunner.Assert")) {
					location = errorStackLines[i+1].Substring("  at ".Length);
					break;
				}
				if (line.StartsWith("  at Miyamasu.MiyamasuTestRunner.WaitUntil")) {
					location = errorStackLines[i+1].Substring("  at ".Length);
					break;
				}
			}

			if (string.IsNullOrEmpty(subLocation)) TestLogger.Log("test FAILED by:" + e.InnerException.Message + " @ " + location, true);
			else TestLogger.Log("test FAILED by:" + e.InnerException.Message + " @ " + location + " of " + subLocation, true);
		}
		
		/**
			can wait Async code execution until specified sec passed.
		*/
		public void WaitUntil (Func<bool> isCompleted, int timeoutSec=1, string message="") {
			var methodName = new Diag.StackFrame(1).GetMethod().Name;
			Exception error = null;
			
			var resetEvent = new ManualResetEvent(false);
			var waitingThread = new Thread(
				() => {
					resetEvent.Reset();
					var endTick = (DateTime.Now + TimeSpan.FromSeconds(timeoutSec)).Ticks;
					
					while (!isCompleted()) {
						var current = DateTime.Now.Ticks;
						
						if (0 < timeoutSec && endTick < current) {
							if (!string.IsNullOrEmpty(message)) {
								error = new Exception("timeout. reason:" + message);
							} else {
								error = new Exception("timeout.");
							}
							break;
						}
						
						System.Threading.Thread.Sleep(10);
					}
					resetEvent.Set();
				}
			);
			
			waitingThread.Start();
			
			resetEvent.WaitOne();
			
			if (error != null) {
				throw error;
			}
		}

		/**
			Run action on UnityEditor's MainThread.
			let set [bool sync] = false if you want to execute action on MainThread but async.
			default is sync.
		*/
		public void RunOnMainThread (Action invokee, bool @sync = true) {
			UnityEditor.EditorApplication.CallbackFunction runner = null;
			
			var done = false;
			
			runner = () => {
				// run only once.
				EditorApplication.update -= runner;
				if (invokee != null) invokee();
				done = true;
				// 実際にインスタンスを作ってUpdateさせるモード、大差なかった。残念。
				// var sr = new GameObject("test");
				// var c = sr.AddComponent<CoroutineExecutor>();
				// c.Set(invokee);
			};
			
			EditorApplication.update += runner;
			if (@sync) {
				WaitUntil(() => done, -1);
			}
		}
		
		/**
			IEnumerator version. continue running while IEnumerator is running.
		*/
		public void RunEnumeratorOnMainThread (IEnumerator invokee, bool @sync = true) {
			UnityEditor.EditorApplication.CallbackFunction runner = null;
			
			var done = false;
			
			runner = () => {
				var result = invokee.MoveNext();
				if (!result) {
					EditorApplication.update -= runner;
					done = true;
				}
			};
			
			EditorApplication.update += runner;
			if (@sync) {
				WaitUntil(() => done, -1);
			}
		}

		public bool IsTestRunningInPlayingMode () {
			bool isRunningInPlayingMode = false;
			RunOnMainThread(
				() => {
					isRunningInPlayingMode = Application.isPlaying;
				}
			);
			return isRunningInPlayingMode;
		}

		public void SkipCurrentTest (string message) {
			throw new Exception("test skipped with message:" + message);
		}
		

		public void Assert (bool condition, string message) {
			var callerMethodName = new Diag.StackFrame(2).GetMethod().Name;
			if (!condition) {
				throw new Exception("assert failed @ " + callerMethodName + " message:" + message);
			}
		}

		public const string MIYAMASU_TESTLOG_FILE_NAME = "miyamasu_test.log";

		public static class TestLogger {
			static TestLogger () {
				// ログ操作に関する処理で、ハンドラ周りをセットすると良さそう。
			}

			public static bool outputLog = true;
			private static object lockObject = new object();

			private static string pathOfLogFile;
			private static StringBuilder _logs = new StringBuilder();
			
			public static void Log (string message, bool writeSoon=false) {
				if (outputLog) UnityEngine.Debug.Log("log:" + message);
				lock (lockObject) {
					if (!writeSoon) {
						_logs.AppendLine(message);
						return;
					}

					pathOfLogFile = MIYAMASU_TESTLOG_FILE_NAME;
					
					// file write
					using (var fs = new FileStream(
						pathOfLogFile,
						FileMode.Append,
						FileAccess.Write,
						FileShare.ReadWrite)
					) {
						using (var sr = new StreamWriter(fs)) {
							if (0 < _logs.Length) {
								sr.WriteLine(_logs.ToString());
								_logs = new StringBuilder();
							}
							sr.WriteLine("log:" + message);
						}
					}
				}
			}

			public static void LogEnd () {
				lock (lockObject) {
					// file write
					using (var fs = new FileStream(
						pathOfLogFile,
						FileMode.Append,
						FileAccess.Write,
						FileShare.ReadWrite)
					) {
						using (var sr = new StreamWriter(fs)) {
							if (0 < _logs.Length) {
								sr.WriteLine(_logs.ToString());
								_logs = new StringBuilder();
							}
						}
					}
				}
			}
		}

		/**
			write log message into test log.
		*/
		public static void Log (string message) {
			TestLogger.Log(message, true);
		}
	}

	/**
		attributes for TestRunner.
	*/
	[AttributeUsage(AttributeTargets.Method)] public class MSetupAttribute : Attribute {
		public MSetupAttribute() {}
	}

	[AttributeUsage(AttributeTargets.Method)] public class MTestAttribute : Attribute {
		public MTestAttribute() {}
	}

	[AttributeUsage(AttributeTargets.Method)] public class MTeardownAttribute : Attribute {
		public MTeardownAttribute() {}
	}
}