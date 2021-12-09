using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(8, "Seven Segment Search")]
    class Day08 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(26, Part1(sample));
        }

        public override object Part1(string input)
        {
            return Solve(input)
                .Where(x => x == 1 || x == 4 || x == 7 || x == 8)
                .Count();
        }

        public override void Part2Test()
        {
            Assert.AreEqual(61229, Part2(sample));
        }

        public override object Part2(string input)
        {
            var tens = new[] { 1000, 100, 10, 1 };
            return Solve(input)
                .ChunkBy(4)
                .Select(x => x.Select((y, i) => (y, i)).Aggregate(0, (a, b) => a + b.y * tens[b.i]))
                .Sum();
        }

        private static IEnumerable<int> Solve(string input)
        {

            foreach (var line in input.ToLines())
            {
                var (signals, hashes, displays) = ParseLine(line);

                var vars = new Dictionary<char, char>();
                var known = new Dictionary<int, string>();

                Queue<Condition> queue = null;
                queue = new(new[] {
                    //  known
                    new Condition(1, x => x.Length == 2),
                    new Condition(4, x => x.Length == 4),
                    new Condition(8, x => x.Length == 7),
                    new Condition(7, x => {
                        if (x.Length != 3) return false;

                        //  we could get 7 automatically but let's defer it
                        //  once we have 1, 7 will give us $a
                        if (!known.TryGetValue(1, out var one)) return false;

                        var set = x.Except(one).ToHashSet();
                        if (set.Count != 1) return false;
                        vars['a'] = set.Single();
                        return true;
                    }),

                    //  length 5
                    //  and the set of all five-seg hashes
                    //  excluding the 8 and 4 sets
                    //  leaves only one value $e
                    new Condition(2, x => {
                        if (x.Length != 5) return false;
                        if (!vars.TryGetValue('a', out var a)) return false;
                        if (!vars.TryGetValue('c', out var c)) return false;
                        if (!vars.TryGetValue('d', out var d)) return false;
                        if (!vars.TryGetValue('g', out var g)) return false;

                        var set = x.Except(
                            new[] { a, c, d, g }
                        ).ToHashSet();

                        if (set.Count != 1) return false;
                        vars['e'] = set.Single();
                        return true;
                    }),

                    //  length 6
                    //  and this hash, excluding the set of hash 7 and hash 4
                    //  leaves only one value $g
                    new Condition(9, x => {
                        if (x.Length != 6) return false;
                        if (!known.TryGetValue(7, out var seven)) return false;
                        if (!known.TryGetValue(4, out var four)) return false;

                        var set = x.Except(
                            seven.Concat(four)
                        ).ToHashSet();

                        if (set.Count != 1) return false;
                        vars['g'] = set.Single();
                        return true;
                    }),
                
                    //  length 5
                    //  and this hash, excluding the set of hash 7 and $g
                    //  leaves only one value $d
                    new Condition(3, x => {
                        if (x.Length != 5) return false;
                        if (!vars.TryGetValue('g', out var g)) return false;
                        if (!known.TryGetValue(7, out var seven)) return false;

                        var set = x.Except(
                            seven.Concat(new[] { g })
                        ).ToHashSet();

                        if (set.Count != 1) return false;
                        vars['d'] = set.Single();
                        return true;
                    }),

                    //  length 6
                    //  and this hash is identical to hash 8 excluding $d
                    new Condition(0, x => {
                        if (x.Length != 6) return false;
                        if (!vars.TryGetValue('d', out var d)) return false;
                        if (!known.TryGetValue(8, out var eight)) return false;

                        return eight
                            .Except(new[] { d })
                            .ToHashSet()
                            .SetEquals(x);
                    }),

                    //  
                    new Condition(6, x => {
                        if (x.Length != 6) return false;
                        if (!vars.TryGetValue('g', out var g)) return false;
                        if (!known.TryGetValue(7, out var seven)) return false;
                        if (!known.TryGetValue(4, out var four)) return false;

                        var set = x.Except(
                            seven.Concat(four.Concat(new[] { g }))
                        ).ToHashSet();

                        if (set.Count != 1) return false;
                        vars['c'] = set.Single();
                        return true;
                    }),

                    //  last value in the queue
                    new Condition(5, x => queue.Count == 0)
                });

                while (queue.Count != 0)
                {
                    bool found = false;
                    var condition = queue.Dequeue();
                    foreach (var str in signals.Except(known.Values))
                    {
                        if (found = condition.predicate(str))
                        {
                            known[condition.number] = str;
                            break;
                        }
                    }

                    if (!found)
                        queue.Enqueue(condition);
                }

                var strToNum = known.ToDictionary(
                    x => new string(x.Value.OrderBy(y => y).ToArray()),
                    x => x.Key
                );

                foreach (var x in displays.Select(y => strToNum[y]))
                    yield return x;
            }
        }

        private sealed record Condition (int number, Predicate<string> predicate);

        private static (string[], Dictionary<int, HashSet<string>>, string[]) ParseLine(string line)
        {
            var (wires, output) = line.Split('|');
            var signals = wires.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new string(x.OrderBy(y => y).ToArray()))
                .ToArray();

            return (
                signals,

                signals.GroupBy(x => x.Length)
                    .ToDictionary(x => x.Key, x => x.ToHashSet()),

                output.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new string(x.OrderBy(y => y).ToArray()))
                    .ToArray()
            );
        }

        private const string sample = @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce";
    }
}
