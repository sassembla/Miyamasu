using System.IO;
using UnityEditor;
using UnityEngine;

namespace Miyamasu {
	/*
		Run tests when Initialize on load.
		this thread is Unity Editor's MainThread, is ≒ Unity Player's MainThread.
	*/
	[InitializeOnLoad] public class MiyamasuTestIgniter {
		static MiyamasuTestIgniter () {

			Debug.LogWarning("miyamasu start running. test results will be appear in " + Path.Combine(Directory.GetParent(Application.dataPath).FullName, MiyamasuTestRunner.MIYAMASU_TESTLOG_FILE_NAME));
			var testRunner = new MiyamasuTestRunner();
			testRunner.RunTestsOnEditorMainThread();
		}
	}
}