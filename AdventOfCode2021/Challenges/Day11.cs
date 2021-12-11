using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(11, "Dumbo Octopus")]
    class Day11 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(1656, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(195, Part2(sample));
        }

        public override object Part1(string input)
        {
            return Simulate(input)
                .Take(100)
                .Sum();
        }

        public override object Part2(string input)
        {
            return Simulate(input)
                .TakeWhile(x => x != 100)
                .Count() + 1;
        }

        private static IEnumerable<int> Simulate(string input)
        {
            var grid = new Grid<int>(input, x => x - '0');
            while (true)
            {
                var flashing = grid.Cells()
                    .Where(x => ++grid[x] > 9)
                    .ToHashSet();

                for (var wave = flashing.ToHashSet()
                    ; wave.Any()
                    ; wave = wave.SelectMany(x => grid.SurroundingCells(x, true))
                        .Where(n => ++grid[n] > 9 && flashing.Add(n))
                        .ToHashSet()) { }

                yield return flashing
                    .Select(x => grid[x] = 0)
                    .Count();
            }
        }

        private void PrintGrid(Grid<int> grid, object stage)
        {
            using var ctx = Logger.Context($"Stage {stage}");
            for (int r = 0; r < grid.Rows; r++)
                Logger.LogLine(string.Join("", grid.RowValues(r)));
        }

        private const string sample = @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526";
    }
}
