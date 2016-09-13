﻿using System;
using System.Collections.Generic;
﻿using System.Linq;
﻿using RedGate.Shared;
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
                wordCounter.EnableLogging(PrintOrderedWordCount, TimeSpan.FromSeconds(10));
                var wordCounts = wordCounter.GetWordFrequency(); 
            }

            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        private static void PrintOrderedWordCount(KeyValuePair<string, int>[] wordFrequencyMap)
        {
            var sortedWordFrequencies = wordFrequencyMap.OrderByDescending(d => d.Value)
                                                        .ThenBy(d => d.Key);

            Console.WriteLine("Current word counts");
            Console.WriteLine("===================");

            foreach (var map in sortedWordFrequencies)
            {
                Console.WriteLine("{0} - {1}", map.Key.ToLower(), map.Value);
            }

            Console.WriteLine();
        }

    }
}