using AdventOfCode2021.Common;
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

        public override object Part1(string input)
        {
            FindPowerConsumption(input, out int gamma, out int epsilon);
            return gamma * epsilon;
        }

        private static void FindPowerConsumption(string input, out int gamma, out int epsilon)
        {
            gamma = 0;
            var grid = new Grid<bool>(input, x => x == '1');
            for (int c = 0; c < grid.Columns; c++)
            {
                if (grid.ColumnValues(c).Count(x => x) > grid.Rows / 2)
                    gamma |= 1 << (grid.Columns - c - 1);
            }

            epsilon = ~gamma & (1 << grid.Columns) - 1;
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
