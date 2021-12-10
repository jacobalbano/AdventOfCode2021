using AdventOfCode2021.Util;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(10, "Syntax Scoring")]
    class Day10 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(26397, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(288957L, Part2(sample));
        }

        public override object Part1(string input)
        {
            var scores = new Dictionary<char, int>() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 } };
            return input.ToLines()
                .SelectWhere(x => (FindFirstIllegalCharacter(x, out var result), result))
                .Select(x => scores[x])
                .Sum();
        }

        public override object Part2(string input)
        {
            var scores = new Dictionary<char, int>() { { ')', 1 }, { ']', 2 }, { '}', 3 }, { '>', 4 } };
            var results = input.ToLines()
                .Where(x => !FindFirstIllegalCharacter(x, out _))
                .Select(FindCompletionString)
                .Select(s => s.Aggregate(0L, (x, y) => x * 5 + scores[y]))
                .OrderBy(x => x)
                .ToArray();

            return results[results.Length / 2];
        }

        private static bool FindFirstIllegalCharacter(string line, out char illegal)
        {
            illegal = default;
            var open = new Stack<char>();
            foreach (var c in line)
            {
                switch (c)
                {
                    case '(': case '[': case '{': case '<': open.Push(c); break;
                    case ')': case ']': case '}': case '>':
                        if (Math.Abs(c - open.Pop()) <= 2) break;
                        illegal = c;
                        return true;
                }
            }

            return false;
        }

        private static string FindCompletionString(string line)
        {
            var open = new Stack<char>();
            foreach (var c in line)
            {
                switch (c)
                {
                    case '(': case '[': case '{': case '<': open.Push(c); break;
                    case ')': case ']': case '}': case '>': open.Pop(); break;
                }
            }

            return string.Join("", open.Select(x => openToClosed[x]));
        }

        private static readonly Dictionary<char, char> openToClosed = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' },
        };

        private const string sample = @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]";
    }
}
