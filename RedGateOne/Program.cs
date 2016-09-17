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
            using (var wordCounter = new WordCounter(new SimpleCharacterReader(), StringComparer.InvariantCultureIgnoreCase))
            {
                PrintOrderedWordCount(wordCounter.GetWordCount());
            }
            
            Console.ReadKey();
        }

        private static void PrintOrderedWordCount(IDictionary<string, int> wordCount)
        {
            var sortedWordCount = wordCount.OrderByDescending(d => d.Value)
                                           .ThenBy(d => d.Key);

            foreach (var pair in sortedWordCount)
            {
                Console.WriteLine("{0} - {1}", pair.Key.ToLower(), pair.Value);
            }
        }

    }
}
