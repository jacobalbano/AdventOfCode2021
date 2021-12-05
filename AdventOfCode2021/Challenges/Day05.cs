using AdventOfCode2021.Common;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(5, "Hydrothermal Venture")]
    class Day05 : ChallengeBase
    {
        public override object Part1(string input)
        {
            return ScanVents(input, orthoOnly: true);
        }

        public override object Part2(string input)
        {
            return ScanVents(input, orthoOnly: false);
        }

        public override void Part1Test()
        {
            Assert.AreEqual(5, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(12, Part2(sample));
        }

        private int ScanVents(string input, bool orthoOnly)
        {
            var (lines, rows, columns) = ParseLines(input);
            var grid = new Grid<int>(rows + 1, columns + 1);

            foreach (var line in lines)
            {
                if (!orthoOnly || line.start.x == line.end.x || line.start.y == line.end.y)
                foreach (var (row, col) in FollowLine(line))
                    grid[row, col]++;
            }

            return grid.Cells().Count(x => grid[x] > 1);
        }

        private static (List<Line> lines, int rows, int columns) ParseLines(string input)
        {
            var result = new List<Line>();
            int rows = 0, columns = 0;
            foreach (var src in input.ToLines())
            {
                var line = ParseLine(src);
                rows = Math.Max(rows, Math.Max(line.start.y, line.end.y));
                columns = Math.Max(columns, Math.Max(line.start.x, line.end.x));
                result.Add(line);
            }

            return (result, rows, columns);
        }

        private static Line ParseLine(string line)
        {
            var (start, end) = line.Split(" -> ");
            var (x1, y1) = start.Split(',').Select(int.Parse);
            var (x2, y2) = end.Split(',').Select(int.Parse);

            return new Line(new Point(x1, y1), new Point(x2, y2));
        }

        private static IEnumerable<(int row, int col)> FollowLine(Line line)
        {
            var stepX = Math.Sign(line.end.x - line.start.x);
            var stepY = Math.Sign(line.end.y - line.start.y);

            var (x, y) = line.start;
            while (true)
            {
                yield return (y, x);
                if ((x, y) == (line.end.x, line.end.y))
                    break;

                x += stepX;
                y += stepY;
            }
        }

        private record Line (Point start, Point end);
        private record Point (int x = 0, int y = 0);

        private const string sample = @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2";
    }
}
