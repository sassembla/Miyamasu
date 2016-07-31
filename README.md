# Miyamasu
TestRunner which can wait async method with  WaitUntil() method.

## Requirement
		
1. Miyamasu requires extends "MiyamasuTestRunner" class for Target class. 
2. Miyamasu requires "[MTest]" attribute for running.
	
## Where I love

### WaitUntil(string methodName, Action<bool> assertion, int waitDurationCount) method is good for me.

this method can wait async operation on that execution line with time limit. 

### report will appears in file.
these tests results will be apper in YOUR_UNITY_PROJECT/miyamasu_test.log


## Sample code

```C#
	[MTest] public bool SyncWait_Success () {
		var done = false;

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
		
		// wait while "done" flag become true on next line.
		// 1 is waiting duration.
		var wait = WaitUntil("SyncWait_Success", () => done, 1);

		// wait will become false when timeout occured.
		if (!wait) return false;

		return true;
	}
```
	
full sample codes are [here](https://github.com/sassembla/Miyamasu/blob/master/Assets/MiyamasuTestRunner/Editor/SampleTests/SampleTestSuite.cs)


## License
MIT.