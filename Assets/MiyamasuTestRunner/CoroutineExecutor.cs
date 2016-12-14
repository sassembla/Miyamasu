using UnityEngine;
using System.Collections;
using System;

namespace Miyamasu {
	[ExecuteInEditMode] public class CoroutineExecutor : MonoBehaviour {
		IEnumerator enu;
		Action act;

		public void Set (IEnumerator enu) {
			this.enu = enu;
		}
		public void Set (Action act) {
			this.act = act;
		}
		// Use this for initialization
		void Start () {
			// StartCoroutine(enu);
		}
		
		bool first = true;
		// Update is called once per frame
		public void Update () {
			// このUpdateを動かすのとUnityEditor.Updateは変わらなかった。
			// if (first) {
			// 	// Debug.LogError("before!");
			// 	first = false;
			// 	act();
			// 	// Debug.LogError("done!");

			// }

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