using System;
using System.IO;
using UnityEngine;

namespace Miyamasu {
    public class Recorder {
        public static bool isRunning = false;
        public static bool isStoppedByFail = false;

        private readonly string className;
        private readonly string methodName;
        public Recorder (string className, string methodName) {
            isRunning = true;
            this.className = className;
            this.methodName = methodName;
        }

        public void SetupFailed (Exception e) {
            WriteReport("setup failed. class:" + className + " method:" + methodName, ReportType.AssertionFailed, e);
            isStoppedByFail = true;
        }

        public void TeardownFailed (Exception e) {
            WriteReport("teardown failed. class:" + className + " method:" + methodName, ReportType.AssertionFailed, e);
            isStoppedByFail = true;
        }

        public void MarkAsAssertionFailed (Action act) {
            try {
                act();
            } catch (Exception e) {
                // このログメッセージ自体をここで発行することによって、Unityコンソール上からクリックした時にダイレクトにコードに飛ぶことができる。
                // このクラスをdll化していることに起因する。
                Debug.LogError("Assertion failed. class:" + className + " method:" + methodName + " e:" + e);

                WriteReport("class:" + className + " method:" + methodName, ReportType.AssertionFailed, e);

                isStoppedByFail = true;
                throw;
            }
        }

        public void MarkAsTimeout (Exception e) {
            WriteReport("class:" + className + " method:" + methodName, ReportType.FailedByTimeout, e);
            isStoppedByFail = true;
        }
        
        public void MarkAsPassed () {
            // このへんでレポート書く
            // Debug.Log("passed. class:" + className + " method:" + methodName);
            WriteReport("class:" + className + " method:" + methodName, ReportType.Passed);
        }

        public enum ReportType {
            Passed,

            FailedByTimeout,
            AssertionFailed,

            SetupFailed,
            TeardownFailed,
        }

        public void WriteReport (string message, ReportType type, Exception e=null) {
            
            // #if UNITY_EDITOR
            // ログを出す
            using (var sw = new StreamWriter("miyamasu.log", true)) {
                sw.WriteLine(type + ":" + message);
                if (e != null) {
                    sw.WriteLine("  " + e);
                }
            }

            // #endif
            // UUebViewでGUIを出す。

        }
    }
 }