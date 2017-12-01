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

public class MoreTests : MiyamasuTestRunner {
	[MTest] public IEnumerator A () {
		AreEqual("", "");
		yield return new WaitForSeconds(1);
		yield return null;
	}

	[MTest] public IEnumerator B () {
		True("" == string.Empty);
		yield return null;
	}

	[MTest] public IEnumerator C () {
		True("" == string.Empty);
		yield return null;
	}

	[MTest] public IEnumerator D () {
		True("" == string.Empty);
		yield return null;
	}

	[MTest] public IEnumerator E () {
		True("" == string.Empty);
		yield return null;
	}

	[MTest] public IEnumerator F () {
		Zero(0);
		yield return null;
	}

	[MTest] public IEnumerator G () {
		Zero(1);
		yield return null;
	}

	[MTest] public IEnumerator H () {
		IsNotEmpty("0");
		yield return null;
	}

	[MTest] public IEnumerator I () {
		IsNotEmpty("0");
		yield return null;
	}

	[MTest] public IEnumerator J () {
		IsNotEmpty("0");
		yield return null;
	}

	[MTest] public IEnumerator K () {
		IsNotEmpty("0");
		yield return null;
	}

	[MTest] public IEnumerator SendLogToSlack () {
		yield return SendLogToSlack("here log!", "miyamasu", 0);
	}

	[MTest] public IEnumerator SendScreenshotToSlack () {
		yield return SendScreenshotToSlack("here scr!", "miyamasu", 0);
	}
}