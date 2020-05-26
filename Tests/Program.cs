// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Aximo.AxTests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // var tester = new ShadowTypeTests();
            // tester.Box(new ShadowTypeTests.TestCase
            // {
            //     Pipeline = PipelineType.Deferred,
            //     LightType = LightType.Directional,
            //     ComparisonName = "Deferred",
            // });
            // tester.Dispose();

            // var tester2 = new ShadowTypeTests();
            // tester2.Box(new ShadowTypeTests.TestCase
            // {
            //     Pipeline = PipelineType.Deferred,
            //     LightType = LightType.Point,
            //     ComparisonName = "Deferred",
            // });
            // tester.Dispose();

            foreach (var testCaseArgs in ShadowTypeTests.GetTestData().Reverse())
            {
                var testCase = (ShadowTypeTests.TestCase)testCaseArgs[0];
                if (testCase.CompareWith != null)
                    continue;

                using (var tester = new ShadowTypeTests())
                    tester.Box(testCase);
            }

            //Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
