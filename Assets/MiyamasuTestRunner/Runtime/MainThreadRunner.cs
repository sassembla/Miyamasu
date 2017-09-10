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
				yield return null;
			}
			
			// wait for check isTestRunning or not.
			yield return new WaitForSeconds(1);
			
			if (false) {
				yield break;
			}

			yield return ContCor(iEnumGens);
		}
		
		private Func<IEnumerator>[] iEnumGens;
		public void SequentialExecute (Func<IEnumerator>[] iEnumGens) {
			this.iEnumGens = iEnumGens;
        }

		private IEnumerator ContCor (Func<IEnumerator>[] iEnumGens) {
			Back:
			Debug.LogWarning("fmm-1 index:" + index);
			if (index == iEnumGens.Length) {
				yield break;
			}

			yield return iEnumGens[index]();
			index = index + 1;
			Debug.LogWarning("fmm");
			goto Back;
		}
	}
}