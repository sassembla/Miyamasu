using System;
using UnityEngine;

namespace Miyamasu {
    public class Reporter {
        private readonly string className;
        private readonly string methodName;
        public Reporter (string className, string methodName) {
            this.className = className;
            this.methodName = methodName;
        }

        public void SetupFailed (Exception e) {
            Debug.Log("setup failed. class:" + className + " method:" + methodName + " e:" + e);
        }

        public void TeardownFailed (Exception e) {
            Debug.Log("teardown failed. class:" + className + " method:" + methodName + " e:" + e);
        }

        public void MarkAsAssertionFailed (Action act) {
            try {
                act();
            } catch (Exception e) {
                Debug.Log("assert failed. class:" + className + " method:" + methodName + " e:" + e);
                throw;
            }
        }

        public void MarkAsTimeout (Exception e) {
            Debug.Log("timeout. class:" + className + " method:" + methodName + " e:" + e);
        }
        
        public void MarkAsPassed () {
            Debug.Log("passed. class:" + className + " method:" + methodName);
        }
    }
}