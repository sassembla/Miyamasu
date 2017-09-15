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
public class SuccessSample : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		Debug.Log("setup!");
	}

	[MTeardown] public void Teardown () {
		Debug.Log("Teardown!");
	}

	[MTest] public IEnumerator Same () {
		AreEqual("a", "a");
		yield return null;
	}
	[MTest] public IEnumerator DoneInTime () {
		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		yield return WaitUntil(
			() => runner.n == 10,// enough small.
			() => {throw new TimeoutException("not yet. runner.n:" + runner.n);},
			1.0//sec
		);
	}
}

public class FailSample : MiyamasuTestRunner {
	[MTest] public IEnumerator Different () {
		AreEqual("", 1);
		yield return null;
	}

	[MTest] public IEnumerator Timeout () {
		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		yield return WaitUntil(
			() => runner.n == 100,// too much.
			() => {throw new TimeoutException("not yet. runner.n:" + runner.n);},
			1//sec
		);
	}
}