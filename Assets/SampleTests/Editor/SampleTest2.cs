using System;
using System.Net;
using System.Net.Sockets;
using Miyamasu;
using UniRx;
using UnityEngine;
/**
samples of test.
*/
public class SampleTest2 : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		Debug.Log("before");
		RunOnMainThread(
			() => {
				Debug.Log("on main thread");
				/*
					this call will create "MainThreadDispatcher" instance.
				*/
				Observable.EveryUpdate().Subscribe(
					_ => {
						Debug.Log("hereComes");
					}
				);
			}
		);
		Debug.Log("after");
	}
	[MTeardown] public void Teardown () {
		RunOnMainThread(
			() => {
				/*
					kind of UniRx required this trick.
					because MainThreadDispatcher.cs L:500 contains DontDestroyOnLoad method call.

					if "MainThreadDispatcher" is already exists, calling DontDestroyOnLoad makes error like below:

					"The following game object is invoking the DontDestroyOnLoad method: MainThreadDispatcher. Notice that DontDestroyOnLoad can only be used in play mode and, as such, cannot be part of an editor script."

					this makes us unhappy.
					we should destroy created instance. and should be automate this feature.
				*/
				var obj = GameObject.Find("MainThreadDispatcher");
				if (obj != null) GameObject.DestroyImmediate(obj); 
			}
		);
	}

	[MTest] public void First () {
		// do nothing.
	}
	
	[MTest] public void Second () {
		// do nothing.
	}

}