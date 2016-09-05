using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RedGate.Shared;
using RedGateTests;

namespace RedGateTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            var characterReaders = new ICharacterReader[]
            {
                new SlowCharacterReader(), new SimpleCharacterReader()
            };

            using (var wordCounter = new ParallelWordFrequencyCounter(characterReaders, StringComparer.InvariantCultureIgnoreCase))
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var counter = wordCounter.GetWordFrequency();
                stopWatch.Stop();

                Console.WriteLine("Time: {0}", stopWatch.Elapsed.TotalSeconds);
                PrintOrderedWordCount(counter, TimeSpan.FromSeconds(10));
            }

            Console.ReadKey();
        }

        private static void PrintOrderedWordCount(IDictionary<string, int> wordFrequencyMap, TimeSpan delay = default(TimeSpan))
        {
            var sortedWordFrequencies = wordFrequencyMap.OrderByDescending(d => d.Value)
                                                        .ThenBy(d => d.Key);

            foreach (var map in sortedWordFrequencies)
            {
                Console.WriteLine("{0} - {1}", map.Key.ToLower(), map.Value);
                Thread.Sleep(delay);
            }
        }

    }
}
