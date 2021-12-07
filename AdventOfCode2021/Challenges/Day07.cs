using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(7, "The Treachery of Whales")]
    class Day07 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(37, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(168, Part2(sample));
        }

        public override object Part1(string input)
        {
            return CalculateFuel(input, (distance) => distance);
        }

        public override object Part2(string input)
        {
            return CalculateFuel(input, (distance) => distance * (distance + 1) / 2);
        }

        private object CalculateFuel(string input, Func<int, int> fuelRate)
        {
            var positions = input.CSV()
                .Select(int.Parse)
                .GroupBy(x => x)
                .Select(x => (Pos: x.Key, Count: x.Count()))
                .OrderBy(x => x.Pos)
                .ToArray();

            var cost = int.MaxValue;
            int min = positions.First().Pos, max = positions.Last().Pos + 1;
            for (int pos = min; pos < max; pos++)
            {
                cost = Math.Min(cost, positions
                    .Select(x => fuelRate(Math.Abs(x.Pos - pos)) * x.Count)
                    .Sum());
            }

            return cost;
        }


        private const string sample = @"16,1,2,0,4,2,7,1,2,14";
    }
}
