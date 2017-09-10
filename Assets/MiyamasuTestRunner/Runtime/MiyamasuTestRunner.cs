using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using Diag = System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

/**
	MiyamasuTestRunner
*/
namespace Miyamasu {
	public class MiyamasuTestRunner {
		/**
			run on app playing handler.
		 */
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] public static void RunTestsFromCode () {
			Debug.LogError("fmm " + Application.isPlaying);
			var runnerSettings = Settings.LoadSettings();
			if (!runnerSettings.runOnPlay) {
				// do nothing.
				return;
			}

			// ready running.
			
			var go = new GameObject("MiyamasuTestMainThreadRunner");
			go.hideFlags = go.hideFlags | HideFlags.HideAndDontSave;
			
			var mb = go.AddComponent<MainThreadRunner>();
			var s = new MiyamasuTestRunner();

			mb.SequentialExecute(s.TestMethodEnums());
		}

		public MiyamasuTestRunner () {}
		public Func<IEnumerator>[] TestMethodEnums () {
			var testTargetMethods = Assembly.GetExecutingAssembly().
				GetTypes().SelectMany(t => t.GetMethods()).
				Where(method => 0 < method.GetCustomAttributes(typeof(UnityTestAttribute), false).Length).ToArray();

			var typeAndMethogs = new Dictionary<Type, List<MethodInfo>>();

			foreach (var method in testTargetMethods) {
				var type = method.DeclaringType;
				if (!typeAndMethogs.ContainsKey(type)) {
					typeAndMethogs[type] = new List<MethodInfo>();
				}
				typeAndMethogs[type].Add(method);
			}

			var enums = typeAndMethogs.SelectMany(
				t => {
					var ss = new List<Func<IEnumerator>>();
					foreach (var method in t.Value) {
						Func<IEnumerator> s = () => {
							Debug.LogError("-2");
							return MethodCoroutines(t.Key, method);
						};
						ss.Add(s);
					}
					return ss;
				}
			).ToArray();

			return enums;
		}

		private IEnumerator MethodCoroutines (Type type, MethodInfo methodInfo) {
			Debug.LogError("良い感じにここまできてる -1");
			var instance = Activator.CreateInstance(type);
			var cor = methodInfo.Invoke(instance, null) as IEnumerator;
			Debug.LogError("良い感じにここまできてる");
			yield return cor;
		}
		
		// private class RunnerInstance {
		// 	public IEnumerator Runner (Action act) {
		// 		act();
		// 		yield break;
		// 	}

		// 	public IEnumerator Runner (IEnumerator actEnum, Action done) {
		// 		while (actEnum.MoveNext()) {
		// 			yield return null;
		// 		}
		// 		done();
		// 	}
		// }

		// public bool IsTestRunningInPlayingMode () {
		// 	bool isRunningInPlayingMode = false;
		// 	RunOnMainThread(
		// 		() => {
		// 			isRunningInPlayingMode = Application.isPlaying;
		// 		}
		// 	);
		// 	return isRunningInPlayingMode;
		// }

		public const string MIYAMASU_TESTLOG_FILE_NAME = "miyamasu_test.log";
	}

	[AttributeUsage(AttributeTargets.Method)] public class MSetup2Attribute : Attribute {
		public MSetup2Attribute() {}
	}
	[AttributeUsage(AttributeTargets.Method)] public class MTeardown2Attribute : Attribute {
		public MTeardown2Attribute() {}
	}
	[AttributeUsage(AttributeTargets.Method)] public class MTest2Attribute : Attribute {
		public MTest2Attribute() {}
	}
}