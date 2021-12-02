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
            var (x, y, _) = Parse(input).Aggregate(new State(), (pos, inst) => inst.dir switch
            {
                "up" => pos with { y = pos.y - inst.val },
                "down" => pos with { y = pos.y + inst.val },
                "forward" => pos with { x = pos.x + inst.val },
                _ => throw new UnreachableCodeException()
            });

            return x * y;
        }

        public override object Part2(string input)
        {
            var (x, y, _) = Parse(input).Aggregate(new State(), (pos, inst) => inst.dir switch
            {
                "up" => pos with { aim = pos.aim - inst.val },
                "down" => pos with { aim = pos.aim + inst.val },
                "forward" => pos with { x = pos.x + inst.val, y = pos.y + pos.aim * inst.val },
                _ => throw new UnreachableCodeException()
            });

            return x * y;
        }

        private sealed record State (int x = 0, int y = 0, int aim = 0);

        private static IEnumerable<(string dir, int val)> Parse(string input)
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
