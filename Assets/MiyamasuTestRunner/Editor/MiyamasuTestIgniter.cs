using System.IO;
using Miyamasu;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad] public class MiyamasuTestIgniter {
	static MiyamasuTestIgniter () {

		Debug.LogWarning("miyamasu start running. test results will be appear in " + Path.Combine(Directory.GetParent(Application.dataPath).FullName, MiyamasuTestRunner.MIYAMASU_TESTLOG_FILE_NAME));
		var testRunner = new MiyamasuTestRunner();
		testRunner.RunTestsOnEditorMainThread();
	}
}