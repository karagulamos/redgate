using System.Collections.Generic;
using RedGateTests;

namespace RedGate.Shared
{
    public class WordCounter : IWordCounter
    {
        private readonly ICharacterReader _reader;
        private readonly IEqualityComparer<string> _comparer;

        public WordCounter(ICharacterReader reader) : this(reader, EqualityComparer<string>.Default)
        {}

        public WordCounter(ICharacterReader reader, IEqualityComparer<string> comparer)
        {
            _reader = reader;
            _comparer = comparer;
        }

        public IDictionary<string, int> GetWordCount()
        {
            var wordFrequency = new Dictionary<string, int>(_comparer);

            using (var wordEnumerator = new WordEnumerator(_reader))
            {
                while (wordEnumerator.MoveNext())
                {
                    var word = wordEnumerator.Current;
                    int value;
                    wordFrequency.TryGetValue(word, out value);
                    wordFrequency[word] = ++value;
                }
            }

            return wordFrequency;
        }

        public void Dispose()
        {
        }
    }
}
