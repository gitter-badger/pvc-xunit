using PvcCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace PvcPlugins
{
    /// <summary>
    /// Implements methods for plugin of PVC Build Engine <see href="http://http://pvcbuild.com/"/>
    /// enables running testcases written for xUnit test framework <see gref="https://github.com/xunit/xunit"/>
    /// </summary>
    public class PvcXunit : PvcPlugin
    {
        private readonly bool _displayFailureStack;
        private readonly bool _displaySuccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="PvcXunit"/> class.
        /// </summary>
        /// <param name="displaySuccess">if set to <c>true</c> displays succeed testcases.</param>
        /// <param name="displayFailureStack">if set to <c>true</c> displays stacktrace of failed testcases.</param>
        public PvcXunit(bool displaySuccess = true, bool displayFailureStack = true)
        {
            _displaySuccess = displaySuccess;
            _displayFailureStack = displayFailureStack;

            Console.WriteLine("Display success test : {0}, Display failure stack trace {1}", displaySuccess, displayFailureStack);
        }

        /// <summary>
        /// Gets the supported tags.
        /// </summary>
        /// <value>The supported tags.</value>
        public override string[] SupportedTags
        {
            get { return new[] { ".dll" }; }
        }

        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Executes the specified input streams.
        /// </summary>
        /// <param name="inputStreams">The input streams.</param>
        /// <returns>IEnumerable&lt;PvcStream&gt;.</returns>
        public override IEnumerable<PvcStream> Execute(IEnumerable<PvcStream> inputStreams)
        {
            var unit = new XunitProject();

            foreach (var testAssembly in inputStreams.Select(x => x.OriginalSourcePath).Where(File.Exists))
            {
                unit.AddAssembly(new XunitProjectAssembly { AssemblyFilename = testAssembly, ShadowCopy = true });
            }

            unit.RunTestProject(_displaySuccess, _displayFailureStack);

            return inputStreams;
        }
        // ReSharper restore PossibleMultipleEnumeration
    }
}