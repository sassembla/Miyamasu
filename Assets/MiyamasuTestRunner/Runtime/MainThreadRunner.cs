using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Miyamasu {
	public class MainThreadRunner : MonoBehaviour {
		private int index = 0;
		private object lockObj = new object();
		private bool started;
		IEnumerator Start () {
			while (iEnumGens == null) {
				// wait to set enumGens;
				yield return null;
			}
			
			// wait for check UnityTest is running or not.
			yield return new WaitForSeconds(1);
			
			if (Miyamasu.Recorder.isRunning) {
				Destroy(this);
				yield break;
			}

			started = true;
			yield return ContCor();
		}

		void Update () {
			if (started && Recorder.isStoppedByFail) {
				Recorder.isStoppedByFail = false;

				// continue test.
				StartCoroutine(ContCor());
			}
		}
		
		private Func<IEnumerator>[] iEnumGens;
		public void SequentialExecute (Func<IEnumerator>[] iEnumGens) {
			this.iEnumGens = iEnumGens;
        }

		private IEnumerator ContCor () {
			while (index < iEnumGens.Length) {
				yield return iEnumGens[index++]();
			}

			Debug.Log("maybe all tests passed.");
		}

		/**
			this method will be called from jumber lib via SendMessage.
		 */
		public void AddLog (object[] logSource) {
			var type = (int)logSource[0];
			var message = logSource[1] as string;

			// 受け取ることができたので、viewに足す。
		}


	}
}