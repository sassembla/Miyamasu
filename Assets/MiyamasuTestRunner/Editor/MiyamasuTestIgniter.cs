using UnityEditor;
using UnityEngine;

namespace Miyamasu {
	/*
		Run tests when Initialize on load.
		this thread is Unity Editor's MainThread, is ≒ Unity Player's MainThread.
	*/
	[InitializeOnLoad] public class MiyamasuTestIgniter {
		static MiyamasuTestIgniter () {
			Debug.Log("executing.");
			var testRunner = new MiyamasuTestRunner();
			testRunner.RunTestsOnEditorMainThread();
		}

		public static void CloudBuildTest () {
			Debug.Log("executing.");
			#if CLOUDBUILD
			{
				Debug.Log("in cloudbuild.");
			}
			#else
			{
				var testRunner = new MiyamasuTestRunner();
				testRunner.RunTestsOnEditorMainThread();
			}
			#endif
		}
	}
}