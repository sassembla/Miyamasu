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
public class MiyamasuTestRunnerSample : MiyamasuTestRunner2 {
	[MSetup2] public void Setup () {
		Debug.Log("setup!");// 他のテストでエラーが出ると何故か呼ばれる。
	}

	[MTeardown2] public void Teardown () {
		Debug.Log("Teardown!");
	}

	[MTest2] public IEnumerator Some () {
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

public class MiyamasuTestRunnerSample2 : MiyamasuTestRunner2 {
	[MSetup2] public void Setup () {
		Debug.Log("setup2!");
	}

	[MTeardown2] public void Teardown () {
		Debug.Log("Teardown2!");
	}

	[MTest2] public IEnumerator Some () {
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

	[MTest2] public IEnumerator Else () {
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

	[MTest2] public IEnumerator Other () {
		Debug.Log("before4");
		yield return null;
		AreEqual("", 1);
		yield return null;

		Debug.Log("after4");
	}
}