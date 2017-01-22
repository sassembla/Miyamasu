# Miyamasu
UnitTesting-kit and TestRunner on Unity.  
Make asynchronous tests easy.

This TestRunner can selectively run tests **on asynchronous context** or **on Unity's MainThread**.


v 1.4.0

## Motivation
Running asynchronous testing on Unity is still hard. Because some methods of Unity are restricted as being executed on Unity's MainThread.

Miyamas helps it.

All tests are executed in sub threads.  
Also, you can execute the method you want to execute on Unity's MainThread in the test.

## How to run tests
**Run Miyamasu on Player on Editor** : simply play app. all tests will run on playmode.  

**Run Miyamasu on Editor** : write code of tests & compile it, then Miyamasu start running tests automatically.  

**Run Miyamasu on Batch**  : 'sh run_miyamasu_tests.sh' or 'exec run_miyamasu_tests.bat'.  

**Run Miyamasu on Player** : run automatic. interfaces is in progress.

**Run Miyamasu on CloudBuild** : in progress.


## Where I love "Miyamasu"

### WaitUntil(Action<bool> isCompleted, int waitSeconds) method
This method can wait async ops / Unity's MainThread ops on current context, on execution line with time limit.

```C#
// wait on next line. while isCompleted() return true or timelimit comes.
WaitUntil(() => true, 1);
// isCompleted returns true in 1 sec.

// or, Timeout Exception raised.
```

### RunOnMainThread(Action, bool sync=true) method
Can run action into Unity's MainThread(almost perfect pseudo.)

```C#
var dataPath = string.Empty;

/*
	sometimes we should test the method which can only run in Unity's MainThread.
	this Action will be running in Unity's Main Thread.
*/
Action onMainThread = () => {
	dataPath = UnityEngine.Application.dataPath;// this code is only available Unity's MainThread.
};

/*
	run this Action on the MainThread.
	wait that Action's end synchronously.
*/
RunOnMainThread(onMainThread);

```

and IEnumerator version is also available.

```C#
private IEnumerator ShouldRunOnMainThreadEnum (List<string> words) {
	words.Add("C#");
	yield return null;
	words.Add("is");
	yield return null;
	words.Add("awesome.");
}

var words = new List<string>();
		
/*
	run coroutine on Unity's main thread.
	to the end of coroutine. (until coroutine.MoveNext() returns false.)
	synchronously.
*/
		RunEnumeratorOnMainThread(ShouldRunOnMainThreadEnum(words));
```

####note that:
RunOnMainThread and RunEnumeratorOnMainThread is almost perfect psuedo of Unity's MainThread. But sometimes, in case of testing arround AssetBundle and IAP, Unity's IEnumerator does not progress in Editor. These features requires playmode. means, Applicatiin.IsPlaying == true.


### report goes to logfile.
These tests results will be apper in YOUR_UNITY_PROJECT/miyamasu_test.log logFile.

no need to see Unity's GUI.

```
(YOUR_UNITY_PROJECT/miyamasu_test.log)

log:tests started.
test:SampleFail ASSERT FAILED:a is not b, a:1 b:2
log:tests end. passed:3 failed:1
```

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
		
		
		/*
			default behaviour is "sync."
			this thread -> mainThread -> back to this thread.
			and async mode is optionally available.
			set [bool sync] = false.
		*/ 
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
see below.  
[LICENSE](./LICENSE)
