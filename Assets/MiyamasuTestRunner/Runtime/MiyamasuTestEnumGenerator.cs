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
    public class MiyamasuTestEnumGenerator {

        /**
            collect UnityTestAttribute attributed methods then generate coroutines from that.
        */
        public static Queue<Func<IEnumerator>> TestMethods () {
            var testTargetMethods = Assembly.GetExecutingAssembly().
                GetTypes().SelectMany(t => t.GetMethods()).
                Where(method => 0 < method.GetCustomAttributes(typeof(UnityTestAttribute), false).Length).ToArray();

            var typeAndMethogs = new Dictionary<Type, List<MethodInfo>>();

            foreach (var method in testTargetMethods) {
                var type = method.DeclaringType;
                if (!typeAndMethogs.ContainsKey(type)) {
                    typeAndMethogs[type] = new List<MethodInfo>();
                }
                typeAndMethogs[type].Add(method);
            }

            /*
                metod -> coroutine -> coroutines.
                */
            var enumFuncArray = typeAndMethogs.SelectMany(
                t => {
                    var enumFuncList = new List<Func<IEnumerator>>();
                    foreach (var method in t.Value) {
                        Func<IEnumerator> enumFunc = () => {
                            return MethodCoroutines(t.Key, method);
                        };
                        enumFuncList.Add(enumFunc);
                    }
                    return enumFuncList;
                }
            );

            return new Queue<Func<IEnumerator>>(enumFuncArray);
        }

        /**
            generate instance contained method coroutine.
        */
        private static IEnumerator cor;
        private static IEnumerator MethodCoroutines (Type type, MethodInfo methodInfo) {
            var instance = Activator.CreateInstance(type);
            cor = methodInfo.Invoke(instance, null) as IEnumerator;
            
            yield return cor;
        }
    }
}