using Edokan.KaiZen.Colors;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace PvcPlugins
{
    internal class DefaultRunnerCallback : ITestMethodRunnerCallback
    {
        private readonly bool _displayFailureStack;
        private readonly bool _displaySuccess;
        private readonly int _testMethodCount;

        public DefaultRunnerCallback(bool displaySuccess, bool displayFailureStack, int testMethodCount)
        {
            _displaySuccess = displaySuccess;
            _displayFailureStack = displayFailureStack;
            _testMethodCount = testMethodCount;
        }

        public int TotalFailures { get; private set; }

        public int TotalSkips { get; private set; }

        public int TotalTests { get; private set; }

        public double TotalTime { get; private set; }

        public virtual void AssemblyFinished(TestAssembly testAssembly, int total, int failed, int skipped, double time)
        {
            TotalTests = total;
            TotalFailures = failed;
            TotalSkips = skipped;
            TotalTime = time;

            Console.WriteLine("{0} total, {1} failed, {2} skipped, took {3} seconds", total, failed, skipped, time.ToString("0.000", CultureInfo.InvariantCulture));
        }

        public virtual void AssemblyStart(TestAssembly testAssembly)
        { }

        public virtual bool ClassFailed(TestClass testClass, string exceptionType, string message, string stackTrace)
        {
            Console.WriteLine("[FIXTURE FAIL] : {0} ".Red(), testClass.TypeName);
            Console.WriteLine(Indent(message));

            if (stackTrace != null && _displayFailureStack)
            {
                Console.WriteLine(Indent("Stack Trace:"));
                Console.WriteLine(Indent(StackFrameTransformer.TransformStack(stackTrace)).DarkGrey());
            }

            Console.WriteLine();
            return true;
        }

        public virtual void ExceptionThrown(TestAssembly testAssembly, Exception exception)
        {
            Console.WriteLine();
            Console.WriteLine("CATASTROPHIC ERROR OCCURRED:");
            Console.WriteLine(exception.ToString());
            Console.WriteLine("WHILE RUNNING:");
            Console.WriteLine(testAssembly.AssemblyFilename);
            Console.WriteLine();
        }

        public virtual bool TestFinished(TestMethod testMethod)
        {
            var result = testMethod.RunResults.Last();

            var passedResult = result as TestPassedResult;
            if (passedResult != null && _displaySuccess)
            {
                TestPassed(passedResult);
            }
            else
            {
                var failedResult = result as TestFailedResult;
                if (failedResult != null)
                {
                    TestFailed(testMethod, failedResult);
                }
                else
                {
                    var skippedResult = result as TestSkippedResult;
                    if (skippedResult != null)
                    {
                        TestSkipped(testMethod, skippedResult);
                    }
                }
            }

            Console.WriteLine("Tests complete: {0} of {1}".DarkGrey(), ++TotalTests, _testMethodCount);
            return true;
        }

        public virtual bool TestStart(TestMethod testMethod)
        {
            return true;
        }

        protected static string Indent(string message, int additionalSpaces = 0)
        {
            var result = string.Empty;
            var indent = string.Empty.PadRight(additionalSpaces + 3);

            result = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Aggregate(result, (current, line) => current + (indent + line + Environment.NewLine));

            return result.TrimEnd();
        }

        protected virtual void TestFailed(TestMethod testMethod, TestFailedResult result)
        {
            Console.WriteLine("[FAILED] : {0} ".Red(), result.DisplayName);

            Console.WriteLine(Indent(result.ExceptionMessage));

            if (result.ExceptionStackTrace == null || !_displayFailureStack)
            {
                return;
            }

            Console.WriteLine(Indent("Stack Trace:").DarkGrey());
            Console.WriteLine(Indent(StackFrameTransformer.TransformStack(result.ExceptionStackTrace)).DarkGrey());
        }

        protected virtual void TestPassed(TestResult result)
        {
            Console.WriteLine("[SUCCESS] : {0}".Green(), result.DisplayName);
            Console.ResetColor();
        }

        protected virtual void TestSkipped(TestMethod testMethod, TestSkippedResult result)
        {
            Console.WriteLine("[SKIPPED] : {0} ".Yellow(), result.DisplayName);
            Console.WriteLine(Indent(result.Reason));
        }
    }
}