using System.Collections.Generic;
using RedGateTests;

namespace RedGate.Shared
{
    public class WordFrequencyCounter : IWordFrequencyCounter
    {
        private readonly ICharacterReader _reader;
        private readonly IEqualityComparer<string> _comparer;

        public WordFrequencyCounter(ICharacterReader reader) : this(reader, EqualityComparer<string>.Default)
        {}

        public WordFrequencyCounter(ICharacterReader reader, IEqualityComparer<string> comparer)
        {
            _reader = reader;
            _comparer = comparer;
        }

        public IDictionary<string, int> GetWordFrequency()
        {
            var wordFrequency = new Dictionary<string, int>(_comparer);
            using (var wordEnumerator = new WordEnumerator(_reader))
            {
                while (wordEnumerator.MoveNext())
                {
                    var word = wordEnumerator.Current;
                    wordFrequency[word] = wordFrequency.ContainsKey(word) ? ++wordFrequency[word] : 1;
                }
            }

            return wordFrequency;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
