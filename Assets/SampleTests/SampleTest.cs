using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Miyamasu;
using UnityEngine;

/**
	samples of test.
*/
public class MiyamasuTestRunnerSample : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		Debug.Log("setup!");
	}

	[MTeardown] public void Teardown () {
		Debug.Log("Teardown!");
	}

	[MTest] public IEnumerator SomeA () {
		Debug.Log("before");
		
		AreEqual("", "");
		

		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		yield return WaitUntil(
			() => runner.n == 100,
			() => {},
			1
		);

		Debug.Log("after");
	}
}

public class MiyamasuTestRunnerSample2 : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		Debug.Log("setup2!");
	}

	[MTeardown] public void Teardown () {
		Debug.Log("Teardown2!");
	}

	[MTest] public IEnumerator Some () {
		Debug.Log("before2");
		
		AreEqual("", "");
		

		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		yield return WaitUntil(
			() => runner.n == 100,
			() => {throw new TimeoutException("not yet.");},
			1
		);

		// エラーの時だけなぜか遠く離れたsetupとteardownが呼ばれるのなんで
		yield return null;
		Debug.Log("after2");
	}

	[MTest] public IEnumerator Else () {
		Debug.Log("before3");
		
		AreEqual("", "");
		

		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		yield return WaitUntil(
			() => runner.n == 10,
			() => {throw new TimeoutException("not yet.");},
			1
		);

		Debug.Log("after3");
	}

	[MTest] public IEnumerator Other () {
		Debug.Log("before4");
		
		yield return null;
		AreEqual("", 1);
		yield return null;

		Debug.Log("after4");
	}
}