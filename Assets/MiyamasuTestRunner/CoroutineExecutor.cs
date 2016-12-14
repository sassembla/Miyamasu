using UnityEngine;
using System.Collections;
using System;

namespace Miyamasu {
	[ExecuteInEditMode] public class CoroutineExecutor : MonoBehaviour {
		IEnumerator act;

		public void Set (IEnumerator act) {
			this.act = act;
		}
		// Use this for initialization
		void Start () {
			StartCoroutine(act);
		}
		
		// Update is called once per frame
		public void Update () {
			// Debug.LogError("updating");
			// StartCoroutine(WaitOne());
			// 一回は呼ばれるので、次のフレームでまた呼ばれるようにすればよさげ。
			// var obj = new GameObject();
			// obj.transform.SetParent(this.gameObject.transform);
			
		}

		// private IEnumerator WaitOne () {
		// 	yield return new WaitForSeconds(1);
		// // 	Update();
		// }
	}
}