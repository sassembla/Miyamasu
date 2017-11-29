# Miyamasu

Make asynchronous tests on Device easy.  
for Unity 5.6 or later.

![@iPhone](docs/@iPhone.jpg)

## Motivation

Running Unity Tests on Devices and show results.
Also running Unity Tests on Editor.



## How to test sample project

1. Open Window > Test Runner > click PlayMode button.
1. click Create PlayMode test button.
1. Restart Unity once.
1. Open **/Assets/SampleScene.unity**
1. All sample tests will run automatically when play.(for Player)
1. or, click RunAll button on Test Runner window.(for Editor)


## Install Miyamasu to your project

1. Open Window > Test Runner > click PlayMode button.
1. click Create PlayMode test button.
1. Restart Unity once.
1. Open "MiyamasuTestRunner.unitypackage".
1. write new test in your Assets/ folder.


## Example of test

```C#
public class SuccessSample : MiyamasuTestRunner {
	// MSetup is annotation for setup.
	// method should return void or IEnumerator.
	[MSetup] public void Setup () {
		Debug.Log("setup!");
	}

	// MTeardown is annotation for setup.
	// method should return void or IEnumerator.
	[MTeardown] public void Teardown () {
		Debug.Log("Teardown!");
	}

	// MTest is annotation for test case. 
	// method should return IEnumerator.
	[MTest] public IEnumerator Same () {
		AreEqual("a", "a");
		yield return null;
	}

	[MTest] public IEnumerator DoneInTime () {
		var obj = new GameObject("runner");
		IsNotNull(obj);

		var runner = obj.AddComponent<Runner>();

		// WaitUntil method can wait that some condition is achieved. 
		// 1st func<bool> is the condition of this waiting. when returns true, finish waiting.
		// 2nd action is for throw timeout exception. you can set original message for fail by timeout.
		// 3rd double parameter is time limit in sec. default is 5sec.
		yield return WaitUntil(
			() => runner.n == 10,// enough small.
			() => {throw new TimeoutException("not yet. runner.n:" + runner.n);},
			1.0//sec
		);
	}
}
```


## Run tests and Settings

You can run miyamasu tests both UnityEditor and device.

see Unity > Window > Miyamasu Test Runner > Open Settings.

if "runOnPlay" is checked, all tests will run on Play, even if running on Device.
else, you can run tests by Unity -> Window -> TestRunner -> PlayMode button -> Run tests which you need.


## Future

* run on UnityCloudBuild.
* device tests orchestration.
* send failed test logs to Slack.
* accept command from Slack then run specific tests on device.


## License

see below.  
[LICENSE](./LICENSE)
