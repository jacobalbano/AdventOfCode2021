using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(9, "Smoke Basin")]
    class Day9 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(15, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(1134, Part2(sample));
        }

        public override object Part1(string input)
        {
            var grid = new Grid<int>(input, x => x - '0');
            return FindLowPoints(grid)
                .Select(x => grid[x] + 1)
                .Sum();
        }

        public override object Part2(string input)
        {
            var grid = new Grid<int>(input, x => x - '0');
            return FindLowPoints(grid)
                .Select(x => FillBasin(grid, x.row, x.col).Length)
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate((x, y) => x * y);
        }

        private static IEnumerable<(int row, int col)> FindLowPoints(Grid<int> grid)
        {
            foreach (var (row, col) in grid.Cells())
            {
                var value = grid[row, col];
                var lowestNeighbor = grid.SurroundingCells(row, col, false)
                    .Select(x => grid[x])
                    .Min();

                if (value < lowestNeighbor)
                    yield return (row, col);
            }
        }

        private static int[] FillBasin(Grid<int> grid, int lowRow, int lowCol)
        {
            var frontier = new Queue<(int row, int col)>(new[] { (lowRow, lowCol) });
            var visited = new HashSet<(int row, int col)>();

            while (frontier.Any())
            {
                var (row, col) = frontier.Dequeue();
                visited.Add((row, col));

                foreach (var cell in grid.SurroundingCells(row, col, false))
                {
                    if (grid[cell] < 9 && visited.Add(cell))
                        frontier.Enqueue(cell);
                }
            }

            return visited.Select(x => grid[x])
                .ToArray();
        }

        private const string sample = @"2199943210
3987894921
9856789892
8767896789
9899965678";
    }
}
