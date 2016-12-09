using System;
using System.Net;
using System.Net.Sockets;
using Miyamasu;

/**
	samples of test.
*/
public class SampleTest : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		TestLogger.Log("setup");
	}

	[MTeardown] public void Teardown () {
		TestLogger.Log("teardown");
	}

	/**
		always a = b.
		"Assert(bool assertion, string message)" method can raise error when assertion failed.
	*/
	[MTest] public void SampleSuccess () {
		var a = 1;
		var b = 1;
		Assert(a == b, "a is not b, a:" + a + " b:" + b);
	}
	
	[MTest] public void SampleFail () {
		var a = 1;
		var b = 2;
		Assert(a == b, "a is not b, a:" + a + " b:" + b);
	}
	
	/**
		async operation with another thread.
		"WaitUntil(Func<bool> isCompleted, int waitSeconds)" can wait another thread's done.
	*/
	[MTest] public void SampleSuccessAsync () {
        var done = false;

		/*
			the sample case which method own their thread and done with asynchronously.
		*/
		{
			var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 80);

			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var connectArgs = new SocketAsyncEventArgs();
			connectArgs.AcceptSocket = socket;
			connectArgs.RemoteEndPoint = endPoint;
			connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(
				(obj, args) => {
					// failed or succeded, done will be true.
					done = true;
				}
			);

			// next Async operation will be done really soon in async.
			socket.ConnectAsync(connectArgs);
		}

        /*
			wait while 1st parameter "Func<bool> isCompleted" returns true.
			in this case, "done" flag become true when async operation is done.

			2nd parameter "1" means waiting 1 seconds.
        */ 
        WaitUntil(
			() => done, 
			1
		);
    }

	[MTest] public void SampleSuccessAsyncOnMainThread () {
		var dataPath = string.Empty;

		/*
			sometimes we should test the method which can only run in Unity's MainThread.
			this async operation will be running in Unity's Main Thread.
		*/
		Action onMainThread = () => {
			dataPath = UnityEngine.Application.dataPath;// this code is only available Unity's MainThread.
		};

		RunOnMainThread(onMainThread);

		/*
			wait until "dataPath" is not null or empty.
		*/ 
		WaitUntil(() => !string.IsNullOrEmpty(dataPath), 1);
	}
}