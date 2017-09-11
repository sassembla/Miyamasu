using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using NUnit.Framework;

namespace Miyamasu {
    /**
        generate UnityTest unit source code from miyamasu test unit.
     */
    public class Miyamasu2UnityTestConverter {
		public static string GenerateRuntimeTests() {
            var targetTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(MiyamasuTestRunner2).IsAssignableFrom(t)).ToArray();
            var classDescs = new List<TestEntryClass>();
			
            foreach (var targetType in targetTypes) {
                var allMethods = targetType.GetMethods();
                var setup = allMethods.Where(methods => 0 < methods.GetCustomAttributes(typeof(MSetup2Attribute), false).Length).ToArray();
                var teardown = allMethods.Where(methods => 0 < methods.GetCustomAttributes(typeof(MTeardown2Attribute), false).Length).ToArray();

				var testMethods = allMethods
					.Where(methods => 0 < methods.GetCustomAttributes(typeof(MTest2Attribute), false).Length)
					.ToArray();

				if (!testMethods.Any()) {
					continue;
				}

                var genTargetTestEntryClass = new TestEntryClass(targetType.Name, setup.FirstOrDefault(), teardown.FirstOrDefault(), testMethods.Select(m => m.Name).ToArray());
                classDescs.Add(genTargetTestEntryClass);
			}

            var totalClassDesc = RegenerateEntryClasses(classDescs);
            return totalClassDesc;
		}

        private static string RegenerateEntryClasses (List<TestEntryClass> classes) {
            var totalClassDesc = string.Empty;

            totalClassDesc += @"
using UnityEngine.TestTools;
using System;
using System.Collections;";
            
            var methodDesc = @"
    [UnityTest] public IEnumerator ";

            foreach (var klass in classes) {
                var classDesc = @"
public class " + klass.className + @"_Miyamasu {";

                /*
                    add method description.
                    setup -> method() -> teardown
                */
                foreach (var methodName in klass.methodNames) {
                    classDesc += methodDesc + methodName + @"() {
        var rec = new Miyamasu.Recorder(" + "\"" + klass.className + "\", \"" + methodName + "\"" + @");
        var instance = new " + klass.className + @"();
        instance.rec = rec;

        " + SetupDesc(klass.setupMethodName) + @"
        
        yield return instance." + methodName + @"();
        rec.MarkAsPassed();

        " + TeardownDesc(klass.teardownMethodName) + @"
    }";
                }
                classDesc += @"
}";

                totalClassDesc += classDesc;
            }

            return totalClassDesc;
        }

        private static string SetupDesc (string setupMethodName) {
            if (string.IsNullOrEmpty(setupMethodName)) {
                return string.Empty;
            }
            return @"
        try {
            instance." + setupMethodName + @"();
        } catch (Exception e) {
            rec.SetupFailed(e);
            throw;
        }";
        }

        private static string TeardownDesc (string teardownMethodName) {
            if (string.IsNullOrEmpty(teardownMethodName)) {
                return string.Empty;
            }
            return @"
        try {
            instance." + teardownMethodName + @"();
        } catch (Exception e) {
            rec.TeardownFailed(e);
            throw;
        }";
        }

        public class TestEntryClass {
            public readonly string className;
            public readonly string setupMethodName;
            public readonly string teardownMethodName;
            public readonly string[] methodNames;

            public TestEntryClass (string className, MethodInfo setupMethod, MethodInfo teardownMethod, string[] methodNames) {
                this.className = className;

                if (setupMethod != null) {
                    this.setupMethodName = setupMethod.Name;
                } else {
                    this.setupMethodName = string.Empty;
                }

                if (teardownMethod != null) {
                    this.teardownMethodName = teardownMethod.Name;
                } else {
                    this.teardownMethodName = string.Empty;
                }

                this.methodNames = methodNames;
            }
        }
	}
}