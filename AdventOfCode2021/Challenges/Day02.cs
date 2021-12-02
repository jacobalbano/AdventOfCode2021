using AdventOfCode2021.Util;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(2, "Dive!")]
    class Day02 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(150, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(900, Part2(sample));
        }

        public override object Part1(string input)
        {
            int x = 0, y = 0;
            foreach (var (dir, number) in Parse(input))
            {
                switch (dir)
                {
                    case "up": y -= number; break;
                    case "down": y += number; break;
                    case "forward": x += number; break;
                    default: Assert.Unreachable(); break;
                }
            }

            return x * y;
        }

        public override object Part2(string input)
        {
            int x = 0, y = 0, aim = 0;
            foreach (var (dir, number) in Parse(input))
            {
                switch (dir)
                {
                    case "up": aim -= number; break;
                    case "down": aim += number; break;
                    case "forward":
                        x += number;
                        y += aim * number;
                        break;
                    default: Assert.Unreachable(); break;
                }
            }
            return x * y;
        }

        private static IEnumerable<(string, int)> Parse(string input)
        {
            return input.ToLines()
                .Select(x => x.Split(' '))
                .Select(x => (x[0], int.Parse(x[1])));
        }

        private const string sample = @"forward 5
down 5
forward 8
up 3
down 8
forward 2";
    }
}
