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
                var enumerator = new DefaultWordEnumerator();

                while (true)
                {
                    var word = enumerator.GetNextWord(reader);

                    if (string.IsNullOrEmpty(word))
                        break;

                    combinedWordCount[word] = combinedWordCount.ContainsKey(word) ? ++combinedWordCount[word] : 1;
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
