using UnityEditor;
using UnityEngine;

namespace Miyamasu {
	/*
		Run tests when Initialize on load.
		this thread is Unity Editor's MainThread, is ≒ Unity Player's MainThread.
	*/
	[InitializeOnLoad] public class MiyamasuTestIgniter {
		static MiyamasuTestIgniter () {
			/*
				ローカルでのビルド時には実行、
			*/
			#if CLOUDBUILD
			{
				// do nothing.
			}
			#else
			{
				RunTests();
			}
			#endif
		}

		/**
			テスト実行
		*/
		public static void RunTests () {
			var testRunner = new MiyamasuTestRunner();
			testRunner.RunTestsOnEditorMainThread();
		}

		/**
			クラウドビルド時、コマンドラインから実行される関数
		*/
		public static void CloudBuildTest () {
			RunTests();
		}
	}
}