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
			
			CoroutineHolder runner = null;

			var runnerObj = GameObject.Find("MiyamasuTestRunner") as GameObject;
			if (runnerObj == null) {
				runner = new GameObject("MiyamasuTestRunner").AddComponent<CoroutineHolder>();
			} else {
				runner = runnerObj.GetComponent<CoroutineHolder>();
			}

			runner.StartCoroutine(testRunner.RunTestsOnEditorMainThread(runner));
		}

		public class CoroutineHolder : MonoBehaviour {}
	}



	public class CloudBuildTestEntryPoint {
		[Test] public static void Test () {
			MiyamasuTestIgniter.RunTests();
		}
	}
}