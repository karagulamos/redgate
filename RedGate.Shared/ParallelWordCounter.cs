using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedGateTests;

namespace RedGate.Shared
{
    public class ParallelWordCounter : IWordCounter
    {
        private readonly ICharacterReader[] _readers;
        private readonly ConcurrentDictionary<string, int> _combinedWordCount;

        private volatile bool _consumerStarted;
        private Task _wordFrequencyConsumerTask;

        public ParallelWordCounter(ICharacterReader[] readers) : this(readers, EqualityComparer<string>.Default)
        { }

        public ParallelWordCounter(ICharacterReader[] readers, IEqualityComparer<string> comparer)
        {
            _readers = readers;
            _combinedWordCount = new ConcurrentDictionary<string, int>(comparer);
        }

        /// <summary>
        /// Runs a task at the specified <see cref="interval"/> that logs the working result of a <see cref="ParallelWordCounter"/> instance.
        /// </summary>
        /// <param name="logAction">Logging callback</param>
        /// <param name="interval">Interval between logs</param>
        /// <exception cref="ArgumentException"><see cref="logAction"/> is null.</exception>
        /// <exception cref="NotSupportedException">The method was called more than once for the current instance.</exception>
        public void EnableLogging(Action<KeyValuePair<string, int>[]> logAction, TimeSpan interval = default(TimeSpan))
        {
            if (logAction == null)
                throw new ArgumentNullException("logAction");

            if (_wordFrequencyConsumerTask != null)
                throw new NotSupportedException("Logging should only be enabled once per object instance");

            _consumerStarted = true;

            _wordFrequencyConsumerTask = Task.Run(async () =>   // consumer
            {
                while (_consumerStarted)
                {
                    await Task.Delay(interval);
                    logAction(_combinedWordCount.ToArray());
                    // NOTE: 
                    // Using ToArray() on the concurrent dictionary can be slow as it first has to reacquire all underlying locks 
                    // to give us an in-moment snapshot of the collection. We could safely use the default enumerator but we're 
                    // not guaranteed to receive the most recent snapshot of the collection.
                }
            });
        }

        public IDictionary<string, int> GetWordCount()
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
            _combinedWordCount.Clear();
        }
    }
}
