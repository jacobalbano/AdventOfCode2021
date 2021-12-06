using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2021.Challenges
{
    [Challenge(6, "Lanterfish")]
    class Day06 : ChallengeBase
    {
        public override void Part1Test()
        {
            Assert.AreEqual(5934, Part1(sample));
        }

        public override void Part2Test()
        {
            Assert.AreEqual(26984457539, Part2(sample));
        }

        public override object Part1(string input)
        {
            return BruteForce(input, 80);
        }

        public override object Part2(string input)
        {
            return Optimized(input, 256);
        }

        private static int BruteForce(string input, int runs)
        {
            var fishes = input.CSV().Select(int.Parse).ToList();
            for (int r = runs; r --> 0;)
            {
                int fishCount = fishes.Count;
                for (int i = 0; i < fishCount; i++)
                {
                    if (fishes[i]-- == 0)
                    {
                        fishes[i] = 6;
                        fishes.Add(8);
                    }
                }
            }

            return fishes.Count;
        }

        private static long Optimized(string input, int runs)
        {
            var fishes = new long[9];
            foreach (var seed in input.CSV())
                fishes[int.Parse(seed)]++;

            for (int r = runs; r --> 0;)
            {
                long newFish = fishes[0];
                for (int f = 1; f < fishes.Length; f++)
                    fishes[f - 1] = fishes[f];

                fishes[6] += fishes[8] = newFish;
            }

            return fishes.Sum();
        }

        private const string sample = "3,4,3,1,2";
    }
}
