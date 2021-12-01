using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(1, "Sonar Sweep")]
    class Day01 : ChallengeBase
    {

        public override object Part1(string input)
        {
            return Scan(1, input.ToLines()
                .Select(int.Parse)
                .ToArray());
        }

        public override object Part2(string input)
        {
            return Scan(3, input.ToLines()
                .Select(int.Parse)
                .ToArray());
        }

        public override void Part1Test()
        {
            Assert.AreEqual(7, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(5, Part2(sample));
        }

        private static int Scan(int windowSize, int[] depths)
        {
            int result = 0, lastWindow = SumWindow(windowSize, depths, windowSize - 1);
            for (int i = windowSize; i < depths.Length; i++)
            {
                if (lastWindow < (lastWindow = SumWindow(windowSize, depths, i)))
                    result++;
            }

            return result;
        }

        private static int SumWindow(int windowSize, int[] depths, int i)
        {
            int value = 0;
            while (windowSize --> 0)
                value += depths[i - windowSize];
            return value;
        }

        const string sample = @"199
200
208
210
200
207
240
269
260
263";
    }
}
