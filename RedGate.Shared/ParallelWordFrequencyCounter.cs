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
        private readonly ConcurrentDictionary<string, int> _combinedWordCount;

        private volatile bool _consumerStarted; 
        private Task _wordFrequencyConsumerTask;

        public ParallelWordFrequencyCounter(ICharacterReader[] readers) : this(readers, EqualityComparer<string>.Default)
        { }

        public ParallelWordFrequencyCounter(ICharacterReader[] readers, IEqualityComparer<string> comparer)
        {
            _readers = readers;
            _combinedWordCount = new ConcurrentDictionary<string, int>(comparer);
        }

        public void EnableLogging(Action<KeyValuePair<string, int>[]> logAction, TimeSpan delay = default(TimeSpan))
        {
            if (logAction == null) return;

            _consumerStarted = true;

            _wordFrequencyConsumerTask = Task.Run(async () =>   // consumer
            {
                while (_consumerStarted)
                {
                    await Task.Delay(delay);
                    logAction(_combinedWordCount.ToArray());
                    // NOTE: 
                    // Doing a ToArray() reacquires all underlying locks to give us an in-moment snapshot of the collection.
                    // We could have safely used the default enumerator except that we're not guaranteed to receive the most 
                    // recent snapshot of collection.
                }
            });
        }

        public IDictionary<string, int> GetWordFrequency() 
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.ForEach(_readers, options, reader =>   // producers
            {
                using (var wordEnumerator = new WordEnumerator(reader))
                {
                    while (wordEnumerator.MoveNext())
                    {
                        var word = wordEnumerator.Current;
                        _combinedWordCount.AddOrUpdate(word, 1, (key, value) => value + 1);
                    }
                }
            });

            return _combinedWordCount;
        }

        public void Dispose()
        {
            if (_wordFrequencyConsumerTask == null) return;

            _consumerStarted = false;
            _wordFrequencyConsumerTask.Wait();
            _wordFrequencyConsumerTask.Dispose();
        }
    }
}
