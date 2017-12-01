using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
            WriteReport(new string[]{className, methodName}, ReportType.AssertionFailed, string.Empty, e);
            isStoppedByFail = true;
        }

        public void TeardownFailed (Exception e) {
            WriteReport(new string[]{className, methodName}, ReportType.AssertionFailed, string.Empty, e);
            isStoppedByFail = true;
        }

        public void MarkAsAssertionFailed (Action act) {
            try {
                act();
            } catch (Exception e) {
                // tips1 このログメッセージ自体をここで発行することによって、Unityコンソール上からクリックした時にダイレクトにコードに飛ぶことができる。
                // このクラスをdll化していることに起因する。
                
                // tips2 特に次の行は、Assertionのログを出し、その直後にログファイルを漁ることで、このメソッド自体を呼んだファイルの行の特定を行なっている。
                Debug.LogError("Assertion failed. ");

                var lineNumber = GetLineNumber(methodName);
                Debug.LogError("    class:" + className + " method:" + methodName + " line:" + lineNumber + " e:" + e);

                WriteReport(new string[]{className, methodName, lineNumber}, ReportType.AssertionFailed, string.Empty, e);

                isStoppedByFail = true;
                throw;
            }
        }

        /**
            ログから特定のメソッドの実行記録のうち最後のものを取得してその行番号を取り出す
         */
        public static string GetLineNumber (string methodName) {
            var targetLineHeader = "<" + methodName + ">";
            
            var targetLine = string.Empty;
            
            
            if (Application.isEditor) {
                var logPath = string.Empty;

                if (Application.platform == RuntimePlatform.WindowsEditor) {
                    logPath = "C:/Users/" + Environment.UserName + "/AppData/Local/Unity/Editor/Editor.log";
                } else if (Application.platform == RuntimePlatform.OSXEditor) {
                    logPath = "/Users/" + Environment.UserName + "/Library/Logs/Unity/Editor.log";
                } else {
                    return string.Empty;
                }

                using (var reader = new StreamReader(logPath)) {
                    if (1024 < reader.BaseStream.Length) {
                        reader.BaseStream.Seek(-1024, SeekOrigin.End);
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.StartsWith(targetLineHeader)) {
                            
                            targetLine = line;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(targetLine)) {
                    var baseStr = targetLine.Substring(0, targetLine.Length - 1);
                    var countAndOther = baseStr.Split(':');
                    return countAndOther[countAndOther.Length-1];
                }
            }

            return "-";
        }

        public void MarkAsTimeout (Exception e) {
            WriteReport(new string[]{className, methodName}, ReportType.FailedByTimeout, string.Empty, e);
            isStoppedByFail = true;
        }
        
        public void MarkAsPassed (string dateDiff) {
            var descs = dateDiff.Split(':').Where(d => d != "00").ToArray();
            var timeDesc = string.Join(":", descs);
            WriteReport(new string[]{className, methodName}, ReportType.Passed, timeDesc);
        }

        public enum ReportType {
            Passed,

            FailedByTimeout,
            AssertionFailed,

            Error,

            SetupFailed,
            TeardownFailed,
        }

        public static Action<string[], ReportType, Exception> logAct;

        public void WriteReport (string[] message, ReportType type, string seconds="", Exception e=null) {
            if (logAct == null) {
                return;
            }

            if (Application.isEditor) {
                // ログを出す
                using (var sw = new StreamWriter("miyamasu.log", true)) {
                    var str = type + ":" + string.Join(" ", message);

                    // 時間がセットされていれば記載
                    if (!string.IsNullOrEmpty(seconds)) {
                        str += " in " + seconds + " sec";
                    }
                    
                    sw.WriteLine(str);

                    // errorを次の行から追記
                    if (e != null) {
                        sw.WriteLine("  " + e);
                    }
                }
            }
            
            if (logAct != null) {
                logAct(message, type, e);
            }
        }
    }
 }