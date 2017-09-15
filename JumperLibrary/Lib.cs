using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;

/**
    NUnitをラップしつつなおかつログ出力を行うラップ部。
    coroutineからの復帰が可能な形でWaitUntilが実装されている。
 */
namespace Miyamasu {
    public class MiyamasuTestRunner : Assert {
        public Recorder rec;

        public _WaitUntil WaitUntil (Func<bool> assert, Action onTimeout, double sec=5.0) {
            return new _WaitUntil(assert, onTimeout, sec, rec);
        }

        public new void AreEqual(object expected, object actual) {
            rec.MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual));
        }
        public new void AreEqual(object expected, object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual, message, args));
        }
        public new void AreEqual(double expected, double? actual, double delta) {
            rec.MarkAsAssertionFailed(() => Assert.AreEqual(expected, actual, delta));
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
        // public new void Fail(string message, params object[] args) {
        //     Fail(string message, params object[] args)
        // }
        // public new void Fail(string message) {
        //     Fail(string message)
        // }
        // public new void Fail() {
        //     Fail()
        // }
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
            rec.MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(float arg1, float arg2) {
            rec.MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2, message, args));
        }
        public new void GreaterOrEqual(double arg1, double arg2) {
            rec.MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void GreaterOrEqual(uint arg1, uint arg2) {
            rec.MarkAsAssertionFailed(() => GreaterOrEqual(arg1, arg2));
        }
        public new void Ignore() {
            rec.MarkAsAssertionFailed(() => Ignore());
        }
        public new void Ignore(string message) {
            rec.MarkAsAssertionFailed(() => Ignore(message));
        }
        public new void Ignore(string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Ignore(message, args));
        }
        public new void Inconclusive(string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Inconclusive(message, args));
        }
        public new void Inconclusive() {
            rec.MarkAsAssertionFailed(() => Inconclusive());
        }
        public new void Inconclusive(string message) {
            rec.MarkAsAssertionFailed(() => Inconclusive(message));
        }
        public new void IsAssignableFrom<TExpected>(object actual) {
            rec.MarkAsAssertionFailed(() => IsAssignableFrom<TExpected>(actual));
        }
        public new void IsAssignableFrom<TExpected>(object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsAssignableFrom<TExpected>(actual, message, args));
        }
        public new void IsAssignableFrom(Type expected, object actual) {
            rec.MarkAsAssertionFailed(() => IsAssignableFrom(expected, actual));
        }
        public new void IsAssignableFrom(Type expected, object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsAssignableFrom(expected, actual, message, args));
        }
        public new void IsEmpty(IEnumerable collection) {
            rec.MarkAsAssertionFailed(() => IsEmpty(collection));
        }
        public new void IsEmpty(IEnumerable collection, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsEmpty(collection, message, args));
        }
        public new void IsEmpty(string aString, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsEmpty(aString, message, args));
        }
        public new void IsEmpty(string aString) {
            rec.MarkAsAssertionFailed(() => IsEmpty(aString));
        }
        public new void IsFalse(bool? condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsFalse(condition, message, args));
        }
        public new void IsFalse(bool condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsFalse(condition, message, args));
        }
        public new void IsFalse(bool? condition) {
            rec.MarkAsAssertionFailed(() => IsFalse(condition));
        }
        public new void IsFalse(bool condition) {
            rec.MarkAsAssertionFailed(() => IsFalse(condition));
        }
        public new void IsInstanceOf(Type expected, object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsInstanceOf(expected, actual, message, args));
        }
        public new void IsInstanceOf<TExpected>(object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsInstanceOf<TExpected>(actual, message, args));
        }
        public new void IsInstanceOf(Type expected, object actual) {
            rec.MarkAsAssertionFailed(() => IsInstanceOf(expected, actual));
        }
        public new void IsInstanceOf<TExpected>(object actual) {
            rec.MarkAsAssertionFailed(() => IsInstanceOf<TExpected>(actual));
        }
        public new void IsNaN(double aDouble, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsNaN(aDouble, message, args));
        }
        public new void IsNaN(double aDouble) {
            rec.MarkAsAssertionFailed(() => IsNaN(aDouble));
        }
        public new void IsNaN(double? aDouble, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsNaN(aDouble, message, args));
        }
        public new void IsNaN(double? aDouble) {
            rec.MarkAsAssertionFailed(() => IsNaN(aDouble));
        }
        public new void IsNotAssignableFrom(Type expected, object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsNotAssignableFrom(expected, actual, message, args));
        }
        public new void IsNotAssignableFrom(Type expected, object actual) {
            rec.MarkAsAssertionFailed(() => IsNotAssignableFrom(expected, actual));
        }
        public new void IsNotAssignableFrom<TExpected>(object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => IsNotAssignableFrom<TExpected>(actual, message, args));
        }
        public new void IsNotAssignableFrom<TExpected>(object actual) {
            rec.MarkAsAssertionFailed(() => IsNotAssignableFrom<TExpected>(actual));
        }
        public new void IsNotEmpty(IEnumerable collection, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotEmpty(collection, message, args));
        }
        public new void IsNotEmpty(string aString) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotEmpty(aString));
        }
        public new void IsNotEmpty(IEnumerable collection) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotEmpty(collection));
        }
        public new void IsNotEmpty(string aString, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotEmpty(aString, message, args));
        }
        public new void IsNotInstanceOf(Type expected, object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotInstanceOf(expected, actual, message, args));
        }
        public new void IsNotInstanceOf(Type expected, object actual) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotInstanceOf(expected, actual));
        }
        public new void IsNotInstanceOf<TExpected>(object actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotInstanceOf<TExpected>(actual, message, args));
        }
        public new void IsNotInstanceOf<TExpected>(object actual) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotInstanceOf<TExpected>(actual));
        }
        public new void IsNotNull(object anObject, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotNull(anObject, message, args));
        }
        public new void IsNotNull(object anObject) {
            rec.MarkAsAssertionFailed(() => Assert.IsNotNull(anObject));
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
            rec.MarkAsAssertionFailed(() => Assert.That(code, constraint));
        }
        public new void That(bool condition, Func<string> getExceptionMessage) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition, getExceptionMessage));
        }
        public new void That(bool condition) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression, Func<string> getExceptionMessage) {
            rec.MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression, getExceptionMessage));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression, message, args));
        }
        public new void That<TActual>(TActual actual, IResolveConstraint expression) {
            rec.MarkAsAssertionFailed(() => Assert.That<TActual>(actual, expression));
        }
        public new void That(TestDelegate code, IResolveConstraint constraint, Func<string> getExceptionMessage) {
            rec.MarkAsAssertionFailed(() => Assert.That(code, constraint, getExceptionMessage));
        }
        public new void That(Func<bool> condition) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition));
        }
        public new void That(TestDelegate code, IResolveConstraint constraint, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.That(code, constraint, message, args));
        }
        public new void That(Func<bool> condition, Func<string> getExceptionMessage) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition, getExceptionMessage));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr) {
            rec.MarkAsAssertionFailed(() => Assert.That<TActual>(del, expr));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.That<TActual>(del, expr, message, args));
        }
        public new void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr, Func<string> getExceptionMessage) {
            rec.MarkAsAssertionFailed(() => Assert.That(del, expr, getExceptionMessage));
        }
        public new void That(bool condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition, message, args));
        }
        public new void That(Func<bool> condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.That(condition, message, args));
        }
        public new void True(bool? condition) {
            rec.MarkAsAssertionFailed(() => Assert.True(condition));
        }
        public new void True(bool condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.True(condition, message, args));
        }
        public new void True(bool? condition, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.True(condition, message, args));
        }
        public new void True(bool condition) {
            rec.MarkAsAssertionFailed(() => Assert.True(condition));
        }
        public new void Zero(int actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(int actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(uint actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(uint actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(long actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(ulong actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(ulong actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(decimal actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(decimal actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(double actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(double actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(float actual) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual));
        }
        public new void Zero(long actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        public new void Zero(float actual, string message, params object[] args) {
            rec.MarkAsAssertionFailed(() => Assert.Zero(actual, message, args));
        }
        
        public class _WaitUntil : CustomYieldInstruction {
            private Func<bool> assert;
            private readonly Action onTimeout;
            private readonly long timelimit;

            private readonly IEnumerator t;

            public _WaitUntil (Func<bool> assert, Action onTimeout, double sec, Recorder rec) {
                this.assert = assert;
                this.onTimeout = onTimeout;
                this.timelimit = (DateTime.Now + TimeSpan.FromSeconds(sec)).Ticks;
                this.t = _WaitCor(rec);
            }
            
            public override bool keepWaiting {
                get {
                    return t.MoveNext();
                }
            }

            private IEnumerator _WaitCor (Recorder rec) {
                while (true) {
                    if (assert()) {
                        // done.
                        yield break;
                    }

                    if (timelimit < DateTime.Now.Ticks) {
                        try {
                            onTimeout();
                        } catch (Exception e) {
                            rec.MarkAsTimeout(e);
                            throw;
                        }

                        yield break;
                    }

                    yield return null;
                }
            }
        }
    }
}