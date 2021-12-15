using AdventOfCodeScaffolding;
using Indigo.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(12, "Passage Pathing")]
    class Day12 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(10, Part1(sample1));
            Assert.AreEqual(19, Part1(sample2));
            Assert.AreEqual(226, Part1(sample3));
        }

        public override object Part1(string input)
        {
            var connections = ParseConnections(input);

            var paths = Traverse1("start", connections).ToList();
            using var ctx = Logger.Context("scan");
            foreach (var path in paths)
                Logger.LogLine(path);

            return paths.Count;
        }
        private static IEnumerable<string> Traverse1(string cave, Graph connections, HashSet<string> visited = null, List<string> route = null)
        {
            visited ??= new HashSet<string>();
            if (char.IsLower(cave[0]))
                visited.Add(cave);

            route ??= new List<string>();
            route.Add(cave);

            if (cave == "end")
                yield return string.Join(",", route);
            else foreach (var next in connections[cave].Where(x => !visited.Contains(x)))
                    foreach (var path in Traverse1(next, connections, visited.ToHashSet(), route.ToList()))
                        yield return path;
        }

        private class Graph : Dictionary<string, List<string>> { }
        private static Graph ParseConnections(string input)
        {
            var connections = new Graph();
            foreach (var line in input.ToLines())
            {
                var (from, to) = line.Split('-');
                connections.Establish(from).Add(to);
                connections.Establish(to).Add(from);
            }

            return connections;
        }

        private const string sample1 = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

        private const string sample2 = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";

        private const string sample3 = @"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW";
    }
}
