using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Miyamasu {
	public class MainThreadRunner : MonoBehaviour {
		private int index = 0;
		private object lockObj = new object();
		IEnumerator Start () {
			while (iEnumGens == null) {
				// wait to set enumGens;
				yield return null;
			}
			
			// wait for check UnityTest is running or not.
			yield return new WaitForSeconds(1);

			if (Miyamasu.Recorder.isRunning) {
				yield break;
			}

			yield return ContCor();
		}

		void Update () {
			if (Recorder.isStoppedByFail) {
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
			Debug.LogError("all tests passed.");
		}
	}
}