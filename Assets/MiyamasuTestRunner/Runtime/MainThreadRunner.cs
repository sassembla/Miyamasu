using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UUebView;

namespace Miyamasu {
	public class MainThreadRunner : MonoBehaviour, IUUebViewEventHandler {
		private int index = 0;
		private bool started;

		private string htmlContent = @"
<h1>Miyamasu Runtime Console</h1>
<p>
	ddd<br>
</p>";

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

			var canvasCor = Resources.LoadAsync<GameObject>("Prefabs/MiyamasuCanvas");

			while (!canvasCor.isDone) {
				yield return null;
			}

			var canvasPrefab = canvasCor.asset as GameObject;
			var canvas = Instantiate(canvasPrefab);

			var scrollViewRectCandidates = canvas.transform.GetComponentsInChildren<RectTransform>();
			GameObject attachTargetView = null;
			foreach (var rect in scrollViewRectCandidates) {
				if (rect.name == "Content") {
					attachTargetView = rect.gameObject;
					break;
				}
			}

			var view = UUebViewComponent.GenerateSingleViewFromHTML(this.gameObject, htmlContent, new Vector2(600,100));			
			view.transform.SetParent(attachTargetView.transform);


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

        void IUUebViewEventHandler.OnLoadStarted()
        {
            // throw new NotImplementedException();
        }

        void IUUebViewEventHandler.OnProgress(double progress)
        {
            // throw new NotImplementedException();
        }

        void IUUebViewEventHandler.OnLoaded()
        {
			Debug.LogWarning("load!");
            // throw new NotImplementedException();
        }

        void IUUebViewEventHandler.OnUpdated()
        {
            // throw new NotImplementedException();
        }

        void IUUebViewEventHandler.OnLoadFailed(ContentType type, int code, string reason)
        {
			Debug.LogError("loadFailed:" + type + " code:" + code + " reason:" + reason);
        }

        void IUUebViewEventHandler.OnElementTapped(ContentType type, GameObject element, string param, string id)
        {
            // throw new NotImplementedException();
        }

        void IUUebViewEventHandler.OnElementLongTapped(ContentType type, string param, string id)
        {
            // throw new NotImplementedException();
        }
    }
}