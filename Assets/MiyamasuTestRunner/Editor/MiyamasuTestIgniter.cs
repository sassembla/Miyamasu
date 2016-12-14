using System;
using System.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Miyamasu {
	/*
		Run tests when Initialize on load.
		this thread is Unity Editor's MainThread, is ≒ Unity Player's MainThread.
	*/
	[InitializeOnLoad] public class MiyamasuTestIgniter {
		static MiyamasuTestIgniter () {
			/*
				ローカルでのビルド時には実行、
			*/
			#if CLOUDBUILD
			{
				// do nothing.
			}
			#else
			{
				RunTests();
			}
			#endif
		}

		
		/**
			テスト実行
		*/
		public static void RunTests () {
			var testRunner = new MiyamasuTestRunner();
			var cor = testRunner.RunTestsOnEditorMainThread();
			// var resetEvent = new ManualResetEvent(false);
			// var first = true;

			loop = () => {
				// if (first) {
				// 	resetEvent.Reset();
				// 	first = false;
				// }

				var result = cor.MoveNext();
				if (!result) {
					EditorApplication.update -= loop;
					// resetEvent.Set();
				}
			};

			// main thread dispatcher.
			EditorApplication.update += loop;
			
			
			// ここで待ちたい、、
			// resetEvent.WaitOne();
		}
		static UnityEditor.EditorApplication.CallbackFunction loop;
	}



	public class CloudBuildTestEntryPoint {
		[Test] public static void Test () {
			MiyamasuTestIgniter.RunTests();
		}
	}
}