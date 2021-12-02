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
            return input.ToLines()
                .Select(int.Parse)
                .Lag()
                .Count(x => x.Item1 < x.Item2);
        }

        public override object Part2(string input)
        {
            return input.ToLines()
                .Select(int.Parse)
                .WindowBy(3)
                .Select(x => x.Sum())
                .Lag()
                .Count(x => x.Item1 < x.Item2);
        }

        public override void Part1Test()
        {
            Assert.AreEqual(7, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(5, Part2(sample));
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
