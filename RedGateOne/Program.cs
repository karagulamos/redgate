using System;
using System.Collections.Generic;
using System.Linq;
using RedGate.Shared;
using RedGateTests;

namespace RedGateOne
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var wordCounter = new WordFrequencyCounter(new SimpleCharacterReader(), StringComparer.InvariantCultureIgnoreCase))
            {
                PrintOrderedWordFrequency(wordCounter.GetWordFrequency());
            }
            
            Console.ReadKey();
        }

        private static void PrintOrderedWordFrequency(IDictionary<string, int> wordFrequencyMap)
        {
            var sortedWordFrequencies = wordFrequencyMap.OrderByDescending(d => d.Value)
                                                        .ThenBy(d => d.Key);

            foreach (var map in sortedWordFrequencies)
            {
                Console.WriteLine("{0} - {1}", map.Key.ToLower(), map.Value);
            }
        }

    }
}
