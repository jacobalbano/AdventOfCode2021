using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;
using Indigo.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(15, "Chiton")]
    class Day15 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(40, Part1(sample));
        }

        public override object Part1(string input)
        {
            var grid = new Grid<int>(input, x => x - '0');
            var solver = new GridPathSolver(grid, allowDiagonal: false);
            var start = solver.GetNodeFromGridPosition(0, 0);
            var end = solver.GetNodeFromGridPosition(grid.Rows - 1, grid.Columns - 1);
            return solver.Solve(start, end)
                .Skip(1)
                .Sum(x => x.Value);
        }

        public override void Part2Test()
        {
            Assert.AreEqual(315, Part2(sample));
        }

        public override object Part2(string input)
        {
            var grid = Complexify(new Grid<int>(input, x => x - '0'));
            var solver = new GridPathSolver(grid, allowDiagonal: false);
            var start = solver.GetNodeFromGridPosition(0, 0);
            var end = solver.GetNodeFromGridPosition(grid.Rows - 1, grid.Columns - 1);
            return solver.Solve(start, end)
                .Skip(1)
                .Sum(x => x.Value);
        }

        private static Grid<int> Complexify(Grid<int> grid)
        {
            var newGrid = new Grid<int>(grid.Rows * 5, grid.Columns * 5);
            foreach (var cell in grid.Cells())
                newGrid[cell] = grid[cell];

            for (int bigC = 0; bigC < 5; bigC++)
            {
                for (int bigR = 0; bigR < 5; bigR++)
                {
                    if (bigC == 0 && bigR == 0) continue;
                    foreach (var (row, col) in grid.Cells())
                    {
                        newGrid[
                            row + grid.Rows * bigR,
                            col + grid.Columns * bigC
                        ] = newGrid[
                            row + grid.Rows * ((bigR > 0 ? -1 : 0) + bigR),
                            col + grid.Columns * ((bigR > 0 ? 0 : -1) + bigC)
                        ] % 9 + 1;
                    }
                }
            }

            return newGrid;
        }

        private const string sample = @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581";

    }
}
