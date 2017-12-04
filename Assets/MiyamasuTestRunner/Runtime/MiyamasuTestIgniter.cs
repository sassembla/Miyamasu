using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using Diag = System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace Miyamasu {
	public class MiyamasuTestIgniter {
		/**
			run on app playing handler.
		 */
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] public static void RunTestsFromCode () {
			var runnerSettings = Settings.LoadSettings();
			if (!runnerSettings.runOnPlay) {
				// do nothing.
				return;
			}
			
			// setup slack integration.
			{
				MiyamasuTestRunner.SendLogFunc = (message, type) => {
					return new SlackIntegration._SendLog(message, type);
				};

				MiyamasuTestRunner.SendScreenshotFunc = (message) => {
					return new SlackIntegration._SendScreenshot(message);
				};
			}

			// ready running.
			
			var go = new GameObject("MiyamasuTestMainThreadRunner");
			go.hideFlags = go.hideFlags | HideFlags.HideAndDontSave;
			
			var queuedTests = MiyamasuTestEnumGenerator.TestMethods();

			var runner = go.AddComponent<MainThreadRunner>();
			runner.SetTests(queuedTests);
		}
	}

	[AttributeUsage(AttributeTargets.Method)] public class MSetupAttribute : Attribute {
		public MSetupAttribute() {}
	}
	[AttributeUsage(AttributeTargets.Method)] public class MTeardownAttribute : Attribute {
		public MTeardownAttribute() {}
	}
	[AttributeUsage(AttributeTargets.Method)] public class MTestAttribute : Attribute {
		public MTestAttribute() {}
	}
}