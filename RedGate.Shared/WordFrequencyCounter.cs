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
            var wordEnumerator = new DefaultWordEnumerator();

            string word = wordEnumerator.GetNextWord(_reader);

            while (!string.IsNullOrEmpty(word))
            {
                wordFrequency[word] = wordFrequency.ContainsKey(word) ? ++wordFrequency[word] : 1;
                word = wordEnumerator.GetNextWord(_reader);
            }

            return wordFrequency;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
