# Miyamasu
UnitTesting-kit and TestRunner on Unity.

This TestRunner can wait async / Unity's MainThread operation on single context. with "WaitUntil()" method.

v 1.0.0

## Requirement of Tests on Miyamasu
1. *.cs file which contains tests should be under "Editor" folder.
2. Miyamasu requires extends "MiyamasuTestRunner" class for supplying Assert and WaitUntil method.
3. Miyamasu requires "[MTest]" attribute for running method as Unit Test. additionary you can use [MSetup] and [MTeardown] attribute for setup/teardown.

```C#	
// (this file is located at "Assets/SOMEWHERE/Editor/Test.cs")

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
This contains sample tests. and it will start running automatically when compile is done.


## Where I love "Miyamasu"

### WaitUntil(Action<bool> isCompleted, int waitSeconds) method
This method can wait async ops / Unity's MainThread ops on current context, on execution line with time limit.

```C#
// wait on next line. while isCompleted() return true or timelimit comes.
WaitUntil(() => true, 1);
// isCompleted returns true in 1 sec.

// or, Timeout Exception raised.
```

### RunOnMainThread(Action) method
Can run action into Unity's MainThread(almost perfect pseudo.)

### report goes to logfile.
These tests results will be apper in YOUR_UNITY_PROJECT/miyamasu_test.log logFile.

no need to see Unity's GUI.

```
(YOUR_UNITY_PROJECT/miyamasu_test.log)

log:tests started.
test:SampleFail ASSERT FAILED:a is not b, a:1 b:2
log:tests end. passed:3 failed:1
```

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


## Manual running button and GUI
missing part of Miyamasu. contribution welcome!

## License
MIT.