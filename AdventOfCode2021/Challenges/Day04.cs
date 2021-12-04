using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(4, "Giant Squid")]
    class Day04 : ChallengeBase
    {

        public override object Part1(string input) => Simulate(input).First();
        public override object Part2(string input) => Simulate(input).Last();

        public override void Part1Test()
        {
            Assert.AreEqual(4512, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(1924, Part2(sample));
        }

        private static bool Play(int value, Grid<Cell> grid)
        {
            foreach (var (row, col) in grid.Cells())
            {
                var cell = grid[row, col];
                if (cell.value == value)
                {
                    if (cell.called) return false;
                    grid[row, col] = cell with { called = true };
                    return grid.RowValues(row).All(x => x.called)
                        || grid.ColumnValues(col).All(x => x.called);
                }
            }

            return false;
        }

        private static IEnumerable<int> Simulate(string input)
        {
            var (moves, grids) = Parse(input);
            foreach (var num in moves)
            {
                foreach (var (grid, remove) in grids.AsRemovable())
                {
                    if (Play(num, grid))
                    {
                        yield return num * grid.Cells()
                            .Select(x => grid[x.row, x.col])
                            .SelectWhere(x => (x.value, !x.called))
                            .Sum();

                        remove();
                    }
                }
            }
        }

        private static (int[], List<Grid<Cell>> grids) Parse(string input)
        {
            var lines = input.ToLines().ToArray();
            var moves = lines.First()
                .CSV()
                .Select(int.Parse)
                .ToArray();

            var grids = new List<Grid<Cell>>();
            foreach (var board in lines.Skip(1).ChunkBy(6))
            {
                int row = 0;
                var grid = new Grid<Cell>(5, 5);
                foreach (var line in board.Skip(1))
                {
                    var cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int col = 0; col < grid.Columns; col++)
                        grid[row, col] = new Cell(int.Parse(cells[col]), false);
                    
                    row++;
                }

                grids.Add(grid);
            }

            return (moves, grids);
        }

        private sealed record Cell (int value, bool called);

        private const string sample = @"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7";
    }
}
