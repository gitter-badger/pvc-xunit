using Edokan.KaiZen.Colors;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace PvcPlugins
{
    internal static class XunitProjectExtension
    {
        internal static void RunTestProject(this XunitProject project, bool displaySuccess, bool displayFailureStack)
        {
            var mate = new MultiAssemblyTestEnvironment();
            var tests = project.Assemblies.Select(x => mate.Load(x.AssemblyFilename, x.ConfigFilename, x.ShadowCopy));

            var totalAssemblies = 0;
            var totalTests = 0;
            var totalFailures = 0;
            var totalSkips = 0;
            var totalTime = 0.0;

            foreach (var test in tests)
            {
                Console.WriteLine();
                Console.WriteLine("Test assembly: {0}", test.AssemblyFilename);
                Console.WriteLine();

                try
                {
                    var methods = test.EnumerateTestMethods(project.Filters.Filter).ToList();

                    if (!methods.Any())
                    {
                        Console.WriteLine("Skipping assembly (no tests match the specified filter).");
                    }
                    else
                    {
                        var callback = new DefaultRunnerCallback(displaySuccess, displayFailureStack, methods.Count);
                        test.Run(methods, callback);

                        ++totalAssemblies;
                        totalTests += callback.TotalTests;
                        totalFailures += callback.TotalFailures;
                        totalSkips += callback.TotalSkips;
                        totalTime += callback.TotalTime;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message.Red());
                }

                mate.Unload(test);
            }

            if (totalAssemblies <= 1)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== {0} total, {1} failed, {2} skipped, took {3} seconds ===",
                totalTests, totalFailures, totalSkips, totalTime.ToString("0.000", CultureInfo.InvariantCulture));
        }
    }
}