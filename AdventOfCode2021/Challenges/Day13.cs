using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(13, "Transparent Origami")]
    class Day13 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(17, Part1(sample));
        }

        public override object Part1(string input)
        {
            return Fold(input)
                .Select(GridToString)
                .First()
                .Count(x => x == '#');
        }

        public override object Part2(string input)
        {
            return Fold(input)
                .Select(GridToString)
                .Last();
        }

        private IEnumerable<Grid<bool>> Fold(string input)
        {
            ParseGrid(input, out var grid, out var folds);

            foreach (var (dimension, line) in folds)
                yield return grid = Fold(grid, dimension, line);
        }

        private static Grid<bool> Fold(Grid<bool> grid, char dimension, int line)
        {
            IStrategy strategy = dimension == 'x'
                ? new FoldOnXStrategy()
                : new FoldOnYStrategy();

            var newGrid = strategy.MakeGrid(grid, line);
            foreach (var (row, col) in grid.Cells())
                if (!strategy.SkipCell(row, col, line))
                    newGrid[strategy.Transform(row, col, line)] |= grid[row, col];

            return newGrid;
        }

        private interface IStrategy
        {
            Grid<bool> MakeGrid(Grid<bool> oldGrid, int foldLine);
            bool SkipCell(int row, int col, int foldLine);
            (int row, int col) Transform(int row, int col, int foldLine);
        }

        private class FoldOnXStrategy : IStrategy
        {
            public Grid<bool> MakeGrid(Grid<bool> oldGrid, int foldLine)
            {
                return new Grid<bool>(oldGrid.Rows, foldLine);
            }

            public bool SkipCell(int row, int col, int foldLine)
            {
                return col == foldLine;
            }

            public (int row, int col) Transform(int row, int col, int foldLine)
            {
                return col < foldLine
                    ? (row, col) :
                    (row, foldLine - (col - foldLine));
            }
        }

        private class FoldOnYStrategy : IStrategy
        {
            public Grid<bool> MakeGrid(Grid<bool> oldGrid, int foldLine)
            {
                return new Grid<bool>(foldLine, oldGrid.Columns);
            }

            public bool SkipCell(int row, int col, int foldLine)
            {
                return row == foldLine;
            }

            public (int row, int col) Transform(int row, int col, int foldLine)
            {
                return row < foldLine
                    ? (row, col)
                    : (foldLine - (row - foldLine), col);
            }
        }

        private static void ParseGrid(string input, out Grid<bool> grid, out (char, int)[] folds)
        {
            var lines = input.ToLines()
                .ToArray();
            var points = new List<(int col, int row)>();

            int i = 0;
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    break;

                var (col, row) = line.Split(',');
                points.Add((int.Parse(col), int.Parse(row)));
            }

            grid = new Grid<bool>(
                points.Max(x => x.row) + 1,
                points.Max(x => x.col) + 1
            );
            foreach (var (col, row) in points)
                grid[row, col] = true;

            folds = new (char, int)[lines.Length - ++i];
            for (int j = 0; j < folds.Length; j++)
            {
                var line = lines[i++].Substring("fold along ".Length);
                var (dimension, num) = line.Split('=');
                folds[j] = (dimension[0], int.Parse(num));
            }
        }

        private static string GridToString(Grid<bool> grid)
        {
            return string.Join("\n", Enumerable.Range(0, grid.Rows)
                .Select(x => grid.RowValues(x))
                .Select(x => x.Select(y => y ? '#' : '.'))
                .Select(x => string.Join("", x)));
        }

        private const string sample = @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5";
    }
}
