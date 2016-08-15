# Miyamasu
TestRunner which can wait async method / Unity's MainThread operation with "WaitUntil()" method.

## Requirement of Tests
		
1. Miyamasu requires extends "MiyamasuTestRunner" class for Target class. 
2. Miyamasu requires "[MTest]" attribute for running. additionary you can use [MSetup] and [MTeardown] attribute for setup/teardown.

```C#	
using Miyamasu;
public class Tests : MiyamasuTestRunner {
	[MSetup] public void Setup () {
		// setup
	}

	[MTeardown] public void Teardown () {
		// teardown
	}

	/**
		unit test method
	*/
	[MTest] public void SampleSuccess () {
		var a = 1;
		var b = 1;
		Assert(a == b, "a is not b, a:" + a + " b:" + b);
	}
}
```
	
## Installation
Use "MiyamasuTestRunner.unitypackage".
This contains sample tests. and it will start running automatically when compile is overed.


## Where I love "Miyamasu"

### WaitUntil(Action<bool> assertion, int waitSeconds)
This method can wait async ops / Unity's MainThread ops.

this method can wait async operation on that execution line with time limit.

### RunOnMainThread(Action) 
Can run action into Unity's MainThread(almost perfect pseudo.)

### report goes to logfile.
These tests results will be apper in YOUR_UNITY_PROJECT/miyamasu_test.log logFile.


## Sample code

```C#
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
```
	
full sample codes are [here](https://github.com/sassembla/Miyamasu/blob/master/Assets/SampleTests/Editor/SampleTest.cs)


## License
MIT.