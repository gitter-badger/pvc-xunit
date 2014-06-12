using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PvcPlugins
{
    /// <summary>
    /// Transforms stack frames and stack traces into compiler-like output
    /// </summary>
    /// <remarks>
    /// Based on implementation of Xunit.ConsoleClient.StackFrameTransformer
    /// </remarks>
    internal static class StackFrameTransformer
    {
        private static readonly Regex Regex;

        static StackFrameTransformer()
        {
            Regex = new Regex(@"^(?<spaces>\s*)at (?<method>.*) in (?<file>.*):(line )?(?<line>\d+)$");
        }

        public static string TransformFrame(string stackFrame)
        {
            if (stackFrame == null)
                return null;

            var match = Regex.Match(stackFrame);
            if (match == Match.Empty)
                return stackFrame;

            return String.Format("{0}{1}({2},0): at {3}",
                                 match.Groups["spaces"].Value,
                                 match.Groups["file"].Value,
                                 match.Groups["line"].Value,
                                 match.Groups["method"].Value);
        }

        public static string TransformStack(string stack)
        {
            return stack == null ? null : String.Join(Environment.NewLine, stack.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(TransformFrame));
        }
    }
}