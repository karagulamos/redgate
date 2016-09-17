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

            using (var wordCounter = new ParallelWordCounter(characterReaders, StringComparer.InvariantCultureIgnoreCase))
            {
                wordCounter.EnableLogging(PrintOrderedWordCount, TimeSpan.FromSeconds(10));
                var wordCount = wordCounter.GetWordCount();
            }

            Console.WriteLine("Done...");
            Console.ReadKey();
        }

        private static void PrintOrderedWordCount(KeyValuePair<string, int>[] wordCount)
        {
            var sortedWordCount = wordCount.OrderByDescending(d => d.Value)
                                           .ThenBy(d => d.Key);

            Console.WriteLine("Current word counts");
            Console.WriteLine("===================");

            foreach (var pair in sortedWordCount)
            {
                Console.WriteLine("{0} - {1}", pair.Key.ToLower(), pair.Value);
            }

            Console.WriteLine();
        }

    }
}