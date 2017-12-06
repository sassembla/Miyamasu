using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Networking;

/**
    NUnitをラップしつつなおかつログ出力を行うラップ部。
    coroutineからの復帰が可能な形でWaitUntilが実装されている。

    このクラスを拡張したクラスでテストを書くことによって、自然とWaitUntilやAreEqなどが使える。
 */
namespace Miyamasu {

    public struct ReportSource {
        public string className;
        public string methodName;
        public string lineNumber;

        public ReportSource (string className, string methodName, string lineNumber=null) {
            this.className = className;
            this.methodName = methodName;
            this.lineNumber = lineNumber;
        }

        public string Description () {
            var desc = "case:" + className + "/" + methodName;
            if (!string.IsNullOrEmpty(lineNumber)) {
                desc += " lineNumber:" + lineNumber;
            }
            return desc;
        }
    }
    [Serializable] public class RunnerSettings {
		[SerializeField] public bool runOnPlay = true;
		[SerializeField] public string slackToken = string.Empty;
		[SerializeField] public string slackChannelName = string.Empty;
	}
    
    public class MiyamasuTestRunner : Assert {

        public string className {private set; get;}
        public string methodName {private set; get;}
        
        public void SetInfo (string className, string methodName) {
            this.className = className;
            this.methodName = methodName;
        }

        public _WaitUntil WaitUntil (Func<bool> assert, Action onTimeout, double sec=5.0) {
            return new _WaitUntil(assert, onTimeout, sec, this);
        }

        public static Func<string, int, CustomYieldInstruction> SendLogFunc;
        public CustomYieldInstruction SendLogToSlack (string message, int type) {
            if (SendLogFunc != null) {
                return SendLogFunc(message, type);
            } else {
                return Empty();
            }
        }

        public static Func<string, CustomYieldInstruction> SendScreenshotFunc;
        public CustomYieldInstruction SendScreenshotToSlack (string message) {
            if (SendScreenshotFunc != null) {
                return SendScreenshotFunc(message);
            } else {
                return Empty();
            }
        }

        public new void AreEqual(object expected, object actual) {
            MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual));
        }
        public new void AreEqual(object expected, object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual, message, args));
        }
        public new void AreEqual(double expected, double? actual, double delta) {
            MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual, delta));
        }
        // public new void AreEqual(double expected, double? actual, double delta, string message, params object[] args) {
        //     AreEqual(double expected, double? actual, double delta, string message, params object[] args)
        // }
        // public new void AreEqual(double expected, double actual, double delta) {
        //     AreEqual(double expected, double actual, double delta)
        // }
        // public new void AreEqual(double expected, double actual, double delta, string message, params object[] args) {
        //     AreEqual(double expected, double actual, double delta, string message, params object[] args)
        // }
        // public new void AreNotEqual(object expected, object actual) {
        //     AreNotEqual(object expected, object actual)
        // }
        // public new void AreNotEqual(object expected, object actual, string message, params object[] args) {
        //     AreNotEqual(object expected, object actual, string message, params object[] args)
        // }
        // public new void AreNotSame(object expected, object actual) {
        //     AreNotSame(object expected, object actual)
        // }
        // public new void AreNotSame(object expected, object actual, string message, params object[] args) {
        //     AreNotSame(object expected, object actual, string message, params object[] args)
        // }
        // public new void AreSame(object expected, object actual, string message, params object[] args) {
        //     AreSame(object expected, object actual, string message, params object[] args)
        // }
        // public new void AreSame(object expected, object actual) {
        //     AreSame(object expected, object actual)
        // }
        // public new void ByVal(object actual, IResolveConstraint expression) {
        //     ByVal(object actual, IResolveConstraint expression)
        // }
        // public new void ByVal(object actual, IResolveConstraint expression, string message, params object[] args) {
        //     ByVal(object actual, IResolveConstraint expression, string message, params object[] args)
        // }
        // public new TActual Catch<TActual>(TestDelegate code) where TActual : Exception {
        //     Catch<TActual>(TestDelegate code)
        // }
        // public new TActual Catch<TActual>(TestDelegate code, string message, params object[] args) where TActual : Exception {
        //     Catch<TActual>(TestDelegate code, string message, params object[] args)
        // }
        // public new Exception Catch(Type expectedExceptionType, TestDelegate code) {
        //     Catch(Type expectedExceptionType, TestDelegate code)
        // }
        // public new Exception Catch(Type expectedExceptionType, TestDelegate code, string message, params object[] args) {
        //     Catch(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
        // }
        // public new Exception Catch(TestDelegate code) {
        //     Catch(TestDelegate code)
        // }
        // public new Exception Catch(TestDelegate code, string message, params object[] args) {
        //     Catch(TestDelegate code, string message, params object[] args)
        // }
        // public new void Contains(object expected, ICollection actual) {
        //     Contains(object expected, ICollection actual)
        // }
        // public new void Contains(object expected, ICollection actual, string message, params object[] args) {
        //     Contains(object expected, ICollection actual, string message, params object[] args)
        // }
        // public new void DoesNotThrow(TestDelegate code, string message, params object[] args) {
        //     DoesNotThrow(TestDelegate code, string message, params object[] args)
        // }
        // public new void DoesNotThrow(TestDelegate code) {
        //     DoesNotThrow(TestDelegate code)
        // }
        public new bool Equals(object a, object b) {
            var result = false;
            MarkAsAssertionFailed(() => {
                result = Assert.Equals(a, b);
            });
            return result;
        }
        public new void Fail(string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Fail(message, args));
            
        }
        public new void Fail(string message) {
            MarkAsAssertionFailed(() => Assert.Fail(message));
        }
        public new void Fail() {
            MarkAsAssertionFailed(() => Assert.Fail());
        }
        // public new void False(bool? condition, string message, params object[] args) {
        //     False(bool? condition, string message, params object[] args)
        // }
        // public new void False(bool condition) {
        //     False(bool condition)
        // }
        // public new void False(bool? condition) {
        //     False(bool? condition)
        // }
        // public new void False(bool condition, string message, params object[] args) {
        //     False(bool condition, string message, params object[] args)
        // }
        // public new void Greater(int arg1, int arg2, string message, params object[] args) {
        //     Greater(int arg1, int arg2, string message, params object[] args)
        // }
        // public new void Greater(int arg1, int arg2) {
        //     Greater(int arg1, int arg2)
        // }
        // public new void Greater(uint arg1, uint arg2, string message, params object[] args) {
        //     Greater(uint arg1, uint arg2, string message, params object[] args)
        // }
        // public new void Greater(long arg1, long arg2, string message, params object[] args) {
        //     Greater(long arg1, long arg2, string message, params object[] args)
        // }
        // public new void Greater(long arg1, long arg2) {
        //     Greater(long arg1, long arg2)
        // }
        // public new void Greater(ulong arg1, ulong arg2) {
        //     Greater(ulong arg1, ulong arg2)
        // }
        // public new void Greater(decimal arg1, decimal arg2, string message, params object[] args) {
        //     Greater(decimal arg1, decimal arg2, string message, params object[] args)
        // }
        // public new void Greater(decimal arg1, decimal arg2) {
        //     Greater(decimal arg1, decimal arg2)
        // }
        // public new void Greater(double arg1, double arg2, string message, params object[] args) {
        //     Greater(double arg1, double arg2, string message, params object[] args)
        // }
        // public new void Greater(float arg1, float arg2, string message, params object[] args) {
        //     Greater(float arg1, float arg2, string message, params object[] args)
        // }
        // public new void Greater(float arg1, float arg2) {
        //     Greater(float arg1, float arg2)
        // }
        // public new void Greater(IComparable arg1, IComparable arg2, string message, params object[] args) {
        //     Greater(IComparable arg1, IComparable arg2, string message, params object[] args)
        // }
        // public new void Greater(IComparable arg1, IComparable arg2) {
        //     Greater(IComparable arg1, IComparable arg2)
        // }
        // public new void Greater(ulong arg1, ulong arg2, string message, params object[] args) {
        //     Greater(ulong arg1, ulong arg2, string message, params object[] args)
        // }
        // public new void Greater(double arg1, double arg2) {
        //     Greater(double arg1, double arg2)
        // }
        // public new void Greater(uint arg1, uint arg2) {
        //     Greater(uint arg1, uint arg2)
        // }
        // public new void GreaterOrEqual(long arg1, long arg2, string message, params object[] args) {
        //     GreaterOrEqual(long arg1, long arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(long arg1, long arg2) {
        //     GreaterOrEqual(long arg1, long arg2)
        // }
        // public new void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args) {
        //     GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(int arg1, int arg2) {
        //     GreaterOrEqual(int arg1, int arg2)
        // }
        // public new void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args) {
        //     GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(ulong arg1, ulong arg2) {
        //     GreaterOrEqual(ulong arg1, ulong arg2)
        // }
        // public new void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args) {
        //     GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(int arg1, int arg2, string message, params object[] args) {
        //     GreaterOrEqual(int arg1, int arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(double arg1, double arg2, string message, params object[] args) {
        //     GreaterOrEqual(double arg1, double arg2, string message, params object[] args)
        // }
        // public new void GreaterOrEqual(decimal arg1, decimal arg2) {
        //     GreaterOrEqual(decimal arg1, decimal arg2)
        // }
        // public new void GreaterOrEqual(float arg1, float arg2, string message, params object[] args) {
        //     GreaterOrEqual(float arg1, float arg2, string message, params object[] args)
        // }
        public new void GreaterOrEqual(IComparable arg1, IComparable arg2) {
            MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(float arg1, float arg2) {
            MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args) {
            MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2, message, args));
        }
        public new void GreaterOrEqual(double arg1, double arg2) {
            MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(uint arg1, uint arg2) {
            MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void Ignore() {
            MarkAsAssertionFailed(() => Ignore());
        }
        public new void Ignore(string message) {
            MarkAsAssertionFailed(() => Ignore(message));
        }
        public new void Ignore(string message, params object[] args) {
            MarkAsAssertionFailed(() => Ignore(message, args));
        }
        public new void Inconclusive(string message, params object[] args) {
            MarkAsAssertionFailed(() => Inconclusive(message, args));
        }
        public new void Inconclusive() {
            MarkAsAssertionFailed(() => Inconclusive());
        }
        public new void Inconclusive(string message) {
            MarkAsAssertionFailed(() => Inconclusive(message));
        }
        public new void IsAssignableFrom<TExpected>(object actual) {
            MarkAsAssertionFailed(() => IsAssignableFrom<TExpected>(actual));
        }
        public new void IsAssignableFrom<TExpected>(object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsAssignableFrom<TExpected>(actual, message, args));
        }
        public new void IsAssignableFrom(Type expected, object actual) {
            MarkAsAssertionFailed(() => IsAssignableFrom(expected, actual));
        }
        public new void IsAssignableFrom(Type expected, object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsAssignableFrom(expected, actual, message, args));
        }
        public new void IsEmpty(IEnumerable collection) {
            MarkAsAssertionFailed(() => IsEmpty(collection));
        }
        public new void IsEmpty(IEnumerable collection, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsEmpty(collection, message, args));
        }
        public new void IsEmpty(string aString, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsEmpty(aString, message, args));
        }
        public new void IsEmpty(string aString) {
            MarkAsAssertionFailed(() => IsEmpty(aString));
        }
        public new void IsFalse(bool? condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsFalse(condition, message, args));
        }
        public new void IsFalse(bool condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsFalse(condition, message, args));
        }
        public new void IsFalse(bool? condition) {
            MarkAsAssertionFailed(() => IsFalse(condition));
        }
        public new void IsFalse(bool condition) {
            MarkAsAssertionFailed(() => IsFalse(condition));
        }
        public new void IsInstanceOf(Type expected, object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsInstanceOf(expected, actual, message, args));
        }
        public new void IsInstanceOf<TExpected>(object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsInstanceOf<TExpected>(actual, message, args));
        }
        public new void IsInstanceOf(Type expected, object actual) {
            MarkAsAssertionFailed(() => IsInstanceOf(expected, actual));
        }
        public new void IsInstanceOf<TExpected>(object actual) {
            MarkAsAssertionFailed(() => IsInstanceOf<TExpected>(actual));
        }
        public new void IsNaN(double aDouble, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsNaN(aDouble, message, args));
        }
        public new void IsNaN(double aDouble) {
            MarkAsAssertionFailed(() => IsNaN(aDouble));
        }
        public new void IsNaN(double? aDouble, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsNaN(aDouble, message, args));
        }
        public new void IsNaN(double? aDouble) {
            MarkAsAssertionFailed(() => IsNaN(aDouble));
        }
        public new void IsNotAssignableFrom(Type expected, object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsNotAssignableFrom(expected, actual, message, args));
        }
        public new void IsNotAssignableFrom(Type expected, object actual) {
            MarkAsAssertionFailed(() => IsNotAssignableFrom(expected, actual));
        }
        public new void IsNotAssignableFrom<TExpected>(object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => IsNotAssignableFrom<TExpected>(actual, message, args));
        }
        public new void IsNotAssignableFrom<TExpected>(object actual) {
            MarkAsAssertionFailed(() => IsNotAssignableFrom<TExpected>(actual));
        }
        public new void IsNotEmpty(IEnumerable collection, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.IsNotEmpty(collection, message, args));
        }
        public new void IsNotEmpty(string aString) {
            MarkAsAssertionFailed(() => Assert.IsNotEmpty(aString));
        }
        public new void IsNotEmpty(IEnumerable collection) {
            MarkAsAssertionFailed(() => Assert.IsNotEmpty(collection));
        }
        public new void IsNotEmpty(string aString, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.IsNotEmpty(aString, message, args));
        }
        public new void IsNotInstanceOf(Type expected, object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.IsNotInstanceOf(expected, actual, message, args));
        }
        public new void IsNotInstanceOf(Type expected, object actual) {
            MarkAsAssertionFailed(() => Assert.IsNotInstanceOf(expected, actual));
        }
        public new void IsNotInstanceOf<TExpected>(object actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.IsNotInstanceOf<TExpected>(actual, message, args));
        }
        public new void IsNotInstanceOf<TExpected>(object actual) {
            MarkAsAssertionFailed(() => Assert.IsNotInstanceOf<TExpected>(actual));
        }
        public new void IsNotNull(object anObject, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.IsNotNull(anObject, message, args));
        }
        public new void IsNotNull(object anObject) {
            MarkAsAssertionFailed(() => Assert.IsNotNull(anObject));
        }
        // public new void IsNull(object anObject, string message, params object[] args) {
        //     IsNull(object anObject, string message, params object[] args)
        // }
        // public new void IsNull(object anObject) {
        //     IsNull(object anObject)
        // }
        // public new void IsTrue(bool condition) {
        //     IsTrue(bool condition)
        // }
        // public new void IsTrue(bool? condition) {
        //     IsTrue(bool? condition)
        // }
        // public new void IsTrue(bool? condition, string message, params object[] args) {
        //     IsTrue(bool? condition, string message, params object[] args)
        // }
        // public new void IsTrue(bool condition, string message, params object[] args) {
        //     IsTrue(bool condition, string message, params object[] args)
        // }
        // public new void Less(IComparable arg1, IComparable arg2, string message, params object[] args) {
        //     Less(IComparable arg1, IComparable arg2, string message, params object[] args)
        // }
        // public new void Less(int arg1, int arg2, string message, params object[] args) {
        //     Less(int arg1, int arg2, string message, params object[] args)
        // }
        // public new void Less(int arg1, int arg2) {
        //     Less(int arg1, int arg2)
        // }
        // public new void Less(uint arg1, uint arg2, string message, params object[] args) {
        //     Less(uint arg1, uint arg2, string message, params object[] args)
        // }
        // public new void Less(uint arg1, uint arg2) {
        //     Less(uint arg1, uint arg2)
        // }
        // public new void Less(long arg1, long arg2, string message, params object[] args) {
        //     Less(long arg1, long arg2, string message, params object[] args)
        // }
        // public new void Less(IComparable arg1, IComparable arg2) {
        //     Less(IComparable arg1, IComparable arg2)
        // }
        // public new void Less(long arg1, long arg2) {
        //     Less(long arg1, long arg2)
        // }
        // public new void Less(ulong arg1, ulong arg2) {
        //     Less(ulong arg1, ulong arg2)
        // }
        // public new void Less(decimal arg1, decimal arg2, string message, params object[] args) {
        //     Less(decimal arg1, decimal arg2, string message, params object[] args)
        // }
        // public new void Less(decimal arg1, decimal arg2) {
        //     Less(decimal arg1, decimal arg2)
        // }
        // public new void Less(double arg1, double arg2, string message, params object[] args) {
        //     Less(double arg1, double arg2, string message, params object[] args)
        // }
        // public new void Less(float arg1, float arg2, string message, params object[] args) {
        //     Less(float arg1, float arg2, string message, params object[] args)
        // }
        // public new void Less(float arg1, float arg2) {
        //     Less(float arg1, float arg2)
        // }
        // public new void Less(ulong arg1, ulong arg2, string message, params object[] args) {
        //     Less(ulong arg1, ulong arg2, string message, params object[] args)
        // }
        // public new void Less(double arg1, double arg2) {
        //     Less(double arg1, double arg2)
        // }
        // public new void LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args) {
        //     LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(int arg1, int arg2, string message, params object[] args) {
        //     LessOrEqual(int arg1, int arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(uint arg1, uint arg2, string message, params object[] args) {
        //     LessOrEqual(uint arg1, uint arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(uint arg1, uint arg2) {
        //     LessOrEqual(uint arg1, uint arg2)
        // }
        // public new void LessOrEqual(long arg1, long arg2, string message, params object[] args) {
        //     LessOrEqual(long arg1, long arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(long arg1, long arg2) {
        //     LessOrEqual(long arg1, long arg2)
        // }
        // public new void LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args) {
        //     LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(int arg1, int arg2) {
        //     LessOrEqual(int arg1, int arg2)
        // }
        // public new void LessOrEqual(decimal arg1, decimal arg2) {
        //     LessOrEqual(decimal arg1, decimal arg2)
        // }
        // public new void LessOrEqual(ulong arg1, ulong arg2) {
        //     LessOrEqual(ulong arg1, ulong arg2)
        // }
        // public new void LessOrEqual(double arg1, double arg2) {
        //     LessOrEqual(double arg1, double arg2)
        // }
        // public new void LessOrEqual(float arg1, float arg2, string message, params object[] args) {
        //     LessOrEqual(float arg1, float arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(float arg1, float arg2) {
        //     LessOrEqual(float arg1, float arg2)
        // }
        // public new void LessOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args) {
        //     LessOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
        // }
        // public new void LessOrEqual(IComparable arg1, IComparable arg2) {
        //     LessOrEqual(IComparable arg1, IComparable arg2)
        // }
        // public new void LessOrEqual(double arg1, double arg2, string message, params object[] args) {
        //     LessOrEqual(double arg1, double arg2, string message, params object[] args)
        // }
        // public new void Negative(int actual) {
        //     Negative(int actual)
        // }
        // public new void Negative(int actual, string message, params object[] args) {
        //     Negative(int actual, string message, params object[] args)
        // }
        // public new void Negative(uint actual) {
        //     Negative(uint actual)
        // }
        // public new void Negative(uint actual, string message, params object[] args) {
        //     Negative(uint actual, string message, params object[] args)
        // }
        // public new void Negative(double actual, string message, params object[] args) {
        //     Negative(double actual, string message, params object[] args)
        // }
        // public new void Negative(float actual) {
        //     Negative(float actual)
        // }
        // public new void Negative(long actual) {
        //     Negative(long actual)
        // }
        // public new void Negative(double actual) {
        //     Negative(double actual)
        // }
        // public new void Negative(long actual, string message, params object[] args) {
        //     Negative(long actual, string message, params object[] args)
        // }
        // public new void Negative(decimal actual) {
        //     Negative(decimal actual)
        // }
        // public new void Negative(ulong actual, string message, params object[] args) {
        //     Negative(ulong actual, string message, params object[] args)
        // }
        // public new void Negative(decimal actual, string message, params object[] args) {
        //     Negative(decimal actual, string message, params object[] args)
        // }
        // public new void Negative(float actual, string message, params object[] args) {
        //     Negative(float actual, string message, params object[] args)
        // }
        // public new void Negative(ulong actual) {
        //     Negative(ulong actual)
        // }
        // public new void NotNull(object anObject, string message, params object[] args) {
        //     NotNull(object anObject, string message, params object[] args)
        // }
        // public new void NotNull(object anObject) {
        //     NotNull(object anObject)
        // }
        // public new void NotZero(float actual) {
        //     NotZero(float actual)
        // }
        // public new void NotZero(double actual, string message, params object[] args) {
        //     NotZero(double actual, string message, params object[] args)
        // }
        // public new void NotZero(double actual) {
        //     NotZero(double actual)
        // }
        // public new void NotZero(decimal actual, string message, params object[] args) {
        //     NotZero(decimal actual, string message, params object[] args)
        // }
        // public new void NotZero(decimal actual) {
        //     NotZero(decimal actual)
        // }
        // public new void NotZero(long actual, string message, params object[] args) {
        //     NotZero(long actual, string message, params object[] args)
        // }
        // public new void NotZero(long actual) {
        //     NotZero(long actual)
        // }
        // public new void NotZero(uint actual, string message, params object[] args) {
        //     NotZero(uint actual, string message, params object[] args)
        // }
        // public new void NotZero(uint actual) {
        //     NotZero(uint actual)
        // }
        // public new void NotZero(int actual, string message, params object[] args) {
        //     NotZero(int actual, string message, params object[] args)
        // }
        // public new void NotZero(int actual) {
        //     NotZero(int actual)
        // }
        // public new void NotZero(float actual, string message, params object[] args) {
        //     NotZero(float actual, string message, params object[] args)
        // }
        // public new void NotZero(ulong actual, string message, params object[] args) {
        //     NotZero(ulong actual, string message, params object[] args)
        // }
        // public new void NotZero(ulong actual) {
        //     NotZero(ulong actual)
        // }
        // public new void Null(object anObject, string message, params object[] args) {
        //     Null(object anObject, string message, params object[] args)
        // }
        // public new void Null(object anObject) {
        //     Null(object anObject)
        // }
        // public new void Pass(string message, params object[] args) {
        //     Pass(string message, params object[] args)
        // }
        // public new void Pass(string message) {
        //     Pass(string message)
        // }
        // public new void Pass() {
        //     Pass()
        // }
        // public new void Positive(long actual) {
        //     Positive(long actual)
        // }
        // public new void Positive(int actual, string message, params object[] args) {
        //     Positive(int actual, string message, params object[] args)
        // }
        // public new void Positive(int actual) {
        //     Positive(int actual)
        // }
        // public new void Positive(long actual, string message, params object[] args) {
        //     Positive(long actual, string message, params object[] args)
        // }
        // public new void Positive(ulong actual) {
        //     Positive(ulong actual)
        // }
        // public new void Positive(ulong actual, string message, params object[] args) {
        //     Positive(ulong actual, string message, params object[] args)
        // }
        // public new void Positive(double actual) {
        //     Positive(double actual)
        // }
        // public new void Positive(decimal actual, string message, params object[] args) {
        //     Positive(decimal actual, string message, params object[] args)
        // }
        // public new void Positive(uint actual) {
        //     Positive(uint actual)
        // }
        // public new void Positive(double actual, string message, params object[] args) {
        //     Positive(double actual, string message, params object[] args)
        // }
        // public new void Positive(float actual) {
        //     Positive(float actual)
        // }
        // public new void Positive(float actual, string message, params object[] args) {
        //     Positive(float actual, string message, params object[] args)
        // }
        // public new void Positive(decimal actual) {
        //     Positive(decimal actual)
        // }
        // public new void Positive(uint actual, string message, params object[] args) {
        //     Positive(uint actual, string message, params object[] args)
        // }
        // public new void ReferenceEquals(object a, object b) {
        //     ReferenceEquals(object a, object b)
        // }
        public new void That(TestDelegate code, IResolveConstraint constraint) {
            MarkAsAssertionFailed(() => Assert.That(code, constraint));
        }
        public new void That(bool condition, Func<string> getExceptionMessage) {
            MarkAsAssertionFailed(() => Assert.That(condition, getExceptionMessage));
        }
        public new void That(bool condition) {
            MarkAsAssertionFailed(() => Assert.That(condition));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression, Func<string> getExceptionMessage) {
            MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression, getExceptionMessage));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression, message, args));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression) {
            MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression));
        }
        public new void That(TestDelegate code, IResolveConstraint constraint, Func<string> getExceptionMessage) {
            MarkAsAssertionFailed(() => Assert.That(code, constraint, getExceptionMessage));
        }
        public new void That(Func<bool> condition) {
            MarkAsAssertionFailed(() => Assert.That(condition));
        }
        public new void That(TestDelegate code, IResolveConstraint constraint, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.That(code, constraint, message, args));
        }
        public new void That(Func<bool> condition, Func<string> getExceptionMessage) {
            MarkAsAssertionFailed(() => Assert.That(condition, getExceptionMessage));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr) {
            MarkAsAssertionFailed(() => Assert.That<TActual>(del, expr));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.That<TActual>(del, expr, message, args));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, Func<string> getExceptionMessage) {
            MarkAsAssertionFailed(() => Assert.That(del, expr, getExceptionMessage));
        }
        public new void That(bool condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.That(condition, message, args));
        }
        public new void That(Func<bool> condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.That(condition, message, args));
        }
        public new void True(bool? condition) {
            MarkAsAssertionFailed(() => Assert.True(condition));
        }
        public new void True(bool condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.True(condition, message, args));
        }
        public new void True(bool? condition, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.True(condition, message, args));
        }
        public new void True(bool condition) {
            MarkAsAssertionFailed(() => Assert.True(condition));
        }
        public new void Zero(int actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(int actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(uint actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(uint actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(long actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(ulong actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(ulong actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(decimal actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(decimal actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(double actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(double actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(float actual) {
            MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(long actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(float actual, string message, params object[] args) {
            MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        
        public class _WaitUntil : CustomYieldInstruction {
            private Func<bool> assert;
            private readonly Action onTimeout;
            private readonly long timelimit;

            private readonly IEnumerator t;

            public _WaitUntil (Func<bool> assert, Action onTimeout, double sec, MiyamasuTestRunner runner) {
                this.assert = assert;
                this.onTimeout = onTimeout;
                this.timelimit = (DateTime.Now + TimeSpan.FromSeconds(sec)).Ticks;
                this.t = _WaitCor(runner);
            }
            
            public override bool keepWaiting {
                get {
                    return t.MoveNext();
                }
            }

            private IEnumerator _WaitCor (MiyamasuTestRunner runner) {
                while (true) {
                    if (assert()) {
                        // done.
                        yield break;
                    }

                    if (timelimit < DateTime.Now.Ticks) {
                        try {
                            onTimeout();
                        } catch (Exception e) {
                            runner.MarkAsTimeout(e);
                            throw;
                        }

                        yield break;
                    }

                    yield return null;
                }
            }
        }

        public void SetupFailed (Exception e) {
            WriteReport(new ReportSource(className, methodName), ReportType.AssertionFailed, string.Empty, e);
            
        }

        public void TeardownFailed (Exception e) {
            WriteReport(new ReportSource(className, methodName), ReportType.AssertionFailed, string.Empty, e);
            
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

                WriteReport(new ReportSource(className, methodName, lineNumber), ReportType.AssertionFailed, string.Empty, e);

                
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
            WriteReport(new ReportSource(className, methodName), ReportType.FailedByTimeout, string.Empty, e);
        }
        
        public void MarkAsPassed (string dateDiff) {
            var descs = dateDiff.Split(':').Where(d => d != "00").ToArray();
            var timeDesc = string.Join(":", descs);
            WriteReport(new ReportSource(className, methodName), ReportType.Passed, timeDesc);
        }

        public static Action<ReportSource, ReportType, Exception, string> logAct;

        public void WriteReport (ReportSource message, ReportType type, string seconds="", Exception e=null) {
            if (logAct != null) {
                logAct(message, type, e, seconds);
            }
        }


        private CustomYieldInstruction Empty () {
            return new EmptyInstruction();
        }

        public class EmptyInstruction : CustomYieldInstruction {
            public override bool keepWaiting => false;
        }



    }

    public enum ReportType {
        Passed,

        FailedByTimeout,
        AssertionFailed,

        Error,

        SetupFailed,
        TeardownFailed,
    }
}