using AdventOfCode2021.Common;
using AdventOfCode2021.Util;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(3, "Binary Diagnostic")]
    class Day03 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(198, Part1(part1sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(230, Part2(part1sample));
        }

        public override object Part1(string input)
        {
            FindPowerConsumption(input, out int gamma, out int epsilon);
            return gamma * epsilon;
        }

        public override object Part2(string input)
        {
            FindLifeSupportRating(input, out int oxygen, out int co2);
            return oxygen * co2;
        }

        private static void FindPowerConsumption(string input, out int gamma, out int epsilon)
        {
            gamma = 0;
            var grid = new Grid<int>(input, x => x - '0');
            for (int c = 0; c < grid.Columns; c++)
            {
                if (grid.ColumnValues(c).Count(x => x == 1) > grid.Rows / 2)
                    gamma |= 1 << (grid.Columns - c - 1);
            }

            epsilon = ~gamma & (1 << grid.Columns) - 1;
        }


        private static void FindLifeSupportRating(string input, out int oxygen, out int co2)
        {
            var grid = new Grid<int>(input, x => x - '0');
            var values = Enumerable.Range(0, grid.Rows)
                .Select(r => grid.RowValues(r).Reverse().Select((v, i) => v == 0 ? 0 : Math.Pow(2, i)).Sum())
                .Select(x => (int)x)
                .ToArray();

            oxygen = Diagnose(grid, values, 1);
            co2 = Diagnose(grid, values, 0);
        }

        private static int Diagnose(Grid<int> grid, int[] values, int target)
        {
            var candidates = new List<int>(Enumerable.Range(0, grid.Rows));
            for (int c = 0; c < grid.Columns; c++)
            {
                var matching = candidates.Count(x => grid[x, c] == target);
                int criteria = target == 0
                    ? matching <= (candidates.Count - matching) ? 0 : 1
                    : matching >= (candidates.Count - matching) ? 1 : 0;

                candidates = candidates
                    .Where(x => grid[x, c] == criteria)
                    .ToList();

                if (candidates.Count == 1)
                    return values[candidates.Single()];
            }

            throw new UnreachableCodeException();
        }

        private const string part1sample = @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010
";
    }
}
