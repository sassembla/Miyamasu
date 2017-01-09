using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Miyamasu {
	public enum TestRunnerMode {
		None,
		Editor,
		CommandLine,
		Player,
		NUnit,
		CloudBuild
	}
	
	/*
		Run tests when Initialize on load.
		this thread is Unity Editor's MainThread, is ≒ Unity Player's MainThread.
	*/
    [InitializeOnLoad] public class MiyamasuTestIgniter {
		static MiyamasuTestIgniter () {

			/*
				・CLOUDBUILD フラグがついてる場合は実行しない
				・コマンドラインからの実行時にはテストを実行しない(コマンドラインで特定のメソッドから実行する)
				・上記以外のケースでは、ローカルでのビルド時には、コンパイル後ごとにテストを実行する
			*/

			// このコンパイルフラグが実行時に判断できるといいんだけどな〜 取得法がわからん。

			#if CLOUDBUILD
			{
				// do nothing.
				return;
			}
			#endif

			var commandLineOptions = System.Environment.CommandLine;
			if (commandLineOptions.Contains("-batchmode")) {
				// batchmode なので、無視する。実際に実行可能なメソッドの定義を確認しとこう。
				return;
			}
			
			// playing mode.
			if (EditorApplication.isPlayingOrWillChangePlaymode) {
				RunTests(
					TestRunnerMode.Editor, 
					(iEnum) => {
						/*
							set gameObject from Editor thread(pre-mainThread.)
						*/
                        EditorApplication.CallbackFunction exe = null;
						
						exe = () => {
							var go = new GameObject("test");
							var mb = go.AddComponent<MainThreadRunner>();
							mb.Commit(
								iEnum,
								() => {
									GameObject.Destroy(go);
								}
							);
							EditorApplication.update -= exe;
						};

						EditorApplication.update += exe;
					}, 
					() => {
						Debug.LogError("player test done.");
					}
				);
			} else {
				// editor mode.
				RunTests(
					TestRunnerMode.Editor, 
					RunOnEditorThread, 
					() => {
						Debug.LogError("editor test done.");
					}
				);
			}
		}

		/*
			テスト本体を動かすIEnumを返すが、これ内部にMainThreadさえ渡せればこれ自体はIEnumな必要がないな、、
			と思ってたんだけど、なんか渡したmainThreadDispatcherがnullになってるな、、
		*/
		public static void RunTests (TestRunnerMode mode, Action<IEnumerator> mainThreadDispatcher, Action onEnd) {
			Debug.Log("start test mode:" + mode);
			var testRunner = new MiyamasuTestRunner(mainThreadDispatcher, onEnd);
			testRunner.RunTestsOnEditorThread();
		}

		private static void RunOnEditorThread (IEnumerator cor) {
			UnityEditor.EditorApplication.CallbackFunction exe = null;
			
			exe = () => {
				var contiune = cor.MoveNext();
				if (!contiune) {
					EditorApplication.update -= exe;
				}
			};

			EditorApplication.update += exe;
		}
	}

	/**
		in cloudBuild, this NUnit test method is as entrypoint of UnitTest.
		but this mechanism is not work yet.
	*/
	public class CloudBuildTestEntryPoint {
		[Test] public static void RunFromNUnit () {
			var go = new GameObject("test");
			var mb = go.AddComponent<MainThreadRunner>();
			
			MiyamasuTestIgniter.RunTests(
				TestRunnerMode.NUnit, 
				(iEnum) => {
					mb.Commit(
						iEnum,
						() => {
							// GameObject.Destroy(go);
						}
					);
				},
				() => {
					Debug.LogError("NUnit test done.");
				}
			);

			// ここでテストの終了を待てないと、非同期メソッドのテストの際に使い物にならない。
			// また、このメソッド起動中はEditorUpdateも走らないので、mainThread的な動作を待つこともできない。
			
			// IEnumeratorを放り込めるユニットテストが欲しい。
			// ここでwait系をかけてしまうと、mainThreadがロックしてしまって死ぬので、asyncができればいいというものでもない。
			// できて欲しいのは、「このメソッドが実行されているスレッドをロックせずに」「特定の処理が終わるまでこのメソッドの終了を遅らせる」という矛盾した処理。
		}
	}
}