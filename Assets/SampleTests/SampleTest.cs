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
public class SampleTest : MiyamasuTestRunner {
	// [MSetup] public void Setup () {
	// 	// Log("setup");
	// }

	// [MTeardown] public void Teardown () {
	// 	// Log("teardown");
	// }

	// /**
	// 	always a = b.
	// 	"Assert(bool assertion, string message)" method can raise error when assertion failed.
	// */
	// [MTest] public void SampleSuccess () {
	// 	var a = 1;
	// 	var b = 1;
	// 	Assert(a == b, "a is not b, a:" + a + " b:" + b);
	// }
	
	// [MTest] public void SampleFail () {
	// 	var a = 1;
	// 	var b = 2;
	// 	Assert(a == b, "a is not b, a:" + a + " b:" + b);
	// }
	
	// /**
	// 	async operation with another thread.
	// 	"WaitUntil(Func<bool> isCompleted, int waitSeconds)" can wait another thread's done.
	// */
	// [MTest] public void SampleSuccessAsync () {
    //     var done = false;

	// 	/*
	// 		the sample case which method own their thread and done with asynchronously.
	// 	*/
	// 	{
	// 		var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 80);

	// 		var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	// 		var connectArgs = new SocketAsyncEventArgs();
	// 		connectArgs.AcceptSocket = socket;
	// 		connectArgs.RemoteEndPoint = endPoint;
	// 		connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(
	// 			(obj, args) => {
	// 				// failed or succeded, done will be true.
	// 				done = true;
	// 			}
	// 		);

	// 		// next Async operation will be done really soon in async.
	// 		socket.ConnectAsync(connectArgs);
	// 	}

    //     /*
	// 		wait while 1st parameter "Func<bool> isCompleted" returns true.
	// 		in this case, "done" flag become true when async operation is done.

	// 		2nd parameter "1" means waiting 1 seconds.
    //     */ 
    //     WaitUntil(
	// 		() => done, 
	// 		1
	// 	);
    // }

	// [MTest] public void SampleSuccessOnMainThread () {
	// 	var dataPath = string.Empty;

	// 	/*
	// 		sometimes we should test the method which can only run in Unity's MainThread.
	// 		this Action will be running in Unity's Main Thread.
	// 	*/
	// 	Action onMainThread = () => {
	// 		dataPath = UnityEngine.Application.dataPath;// this code is only available Unity's MainThread.
	// 	};

	// 	/*
	// 		run this Action on the MainThread.
	// 		wait that Action's end synchronously.
	// 	*/
	// 	RunOnMainThread(onMainThread);

	// 	/*
	// 		check if "dataPath" is not null or empty.
	// 	*/ 
	// 	Assert(!string.IsNullOrEmpty(dataPath), "dataPath is empty or null.");
	// }

	// [MTest] public void SampleSuccessOnMainThreadAsync () {
	// 	var dataPath = string.Empty;

	// 	/*
	// 		sometimes we should test the method which can only run in Unity's MainThread.
	// 		this Action will be running in Unity's Main Thread.
	// 	*/
	// 	Action onMainThread = () => {
	// 		dataPath = UnityEngine.Application.dataPath;// this code is only available Unity's MainThread.
	// 	};

	// 	/*
	// 		run async.
	// 		do not wait the end of the Action.
	// 	*/
	// 	RunOnMainThread(onMainThread, false);

	// 	/*
	// 		wait until "dataPath" is not null or empty.
	// 	*/ 
	// 	WaitUntil(() => !string.IsNullOrEmpty(dataPath), 1, "failed to set dataPath in time.");
	// }

	// [MTest] public void SampleSuccessEnumeratorOnMainThread () {
	// 	var words = new List<string>();
		
	// 	/*
	// 		run coroutine on Unity's main thread.
	// 		to the end of coroutine. (until coroutine.MoveNext() returns false.)
	// 		synchronously.
	// 	*/
	// 	RunEnumeratorOnMainThread(ShouldRunOnMainThreadEnum(words));

	// 	Assert(words.Count == 3, "not match, count:" + words.Count);
	// }
	
	// private IEnumerator ShouldRunOnMainThreadEnum (List<string> words) {
	// 	words.Add("C#");
	// 	yield return null;
	// 	words.Add("is");
	// 	yield return null;
	// 	words.Add("awesome.");
	// }
	
	// [MTest] public void SampleSuccessEnumeratorOnMainThreadAsync () {
	// 	var words = new List<string>();

	// 	/*
	// 		run coroutine on Unity's main thread.
	// 		to the end of coroutine. (until coroutine.MoveNext() returns false.)
	// 		this is asynchronous version.
	// 	*/
	// 	RunEnumeratorOnMainThread(ShouldRunOnMainThreadEnum(words), false);

	// 	// wait above coroutine sets 3 words to "words" list.
	// 	WaitUntil(() => words.Count == 3, 1, "not match, count:" + words.Count);
	// 	Log("result:" + string.Join(" ", (words.ToArray())));
	// }
	
	// [MTest] public void SampleFailByTimeout () {
	// 	var done = false;

	// 	/*
	// 		timeout happens.
    //     */ 
    //     WaitUntil(
	// 		() => done, 
	// 		1,
	// 		"never done."
	// 	);
	// }
}

public class MiyamasuTestRunner2Sample : MiyamasuTestRunner2 {
	[MSetup2] public void Setup () {
		Debug.Log("setup!");
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
			() => {throw new TimeoutException("not yet.");},
			1
		);

		Debug.Log("after");
	}
}

public class MiyamasuTestRunner3Sample : MiyamasuTestRunner2 {
	[MSetup2] public void Setup () {
		Debug.Log("setup!");
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
			() => {throw new TimeoutException("not yet.");},
			1
		);

		Debug.Log("after");
	}
}