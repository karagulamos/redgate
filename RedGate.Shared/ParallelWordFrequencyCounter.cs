using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedGateTests;

namespace RedGate.Shared
{
    public class ParallelWordFrequencyCounter : IWordFrequencyCounter
    {
        private readonly ICharacterReader[] _readers;
        private readonly IEqualityComparer<string> _comparer;

        public ParallelWordFrequencyCounter(ICharacterReader[] readers) : this(readers, EqualityComparer<string>.Default)
        { }

        public ParallelWordFrequencyCounter(ICharacterReader[] readers, IEqualityComparer<string> comparer)
        {
            _readers = readers;
            _comparer = comparer;
        }

        public IDictionary<string, int> GetWordFrequency()
        {
            var combinedWordCount = new ConcurrentDictionary<string, int>(_comparer);
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.ForEach(_readers, options, reader =>
            {
                using (var wordEnumerator = new WordEnumerator(reader))
                {
                    var localWordCounter = new Dictionary<string, int>();

                    while (wordEnumerator.MoveNext())
                    {
                        var word = wordEnumerator.Current;
                        localWordCounter[word] = localWordCounter.ContainsKey(word) ? ++localWordCounter[word] : 1;
                    }

                    foreach (var wordCounter in localWordCounter)
                    {
                        var counter = wordCounter;
                        combinedWordCount.AddOrUpdate(counter.Key, counter.Value, (key, value) => counter.Value + value);
                    }
                }
            });

            return combinedWordCount;
        }

        public void Dispose()
        {
            foreach (var reader in _readers)
                reader.Dispose();
        }
    }
}
