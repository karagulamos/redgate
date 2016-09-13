using System.Collections;
using System.Collections.Generic;
using System.IO;
using RedGateTests;

namespace RedGate.Shared
{
    internal class WordEnumerator : IEnumerator<string>
    {
        private readonly ICharacterReader _reader;
        private string _currentWord;

        public WordEnumerator(ICharacterReader reader)
        {
            _reader = reader;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool MoveNext()
        {
            Current = null;
            return !string.IsNullOrEmpty(Current);
        }

        public void Reset()
        {
        }

        public string Current
        {
            get { return _currentWord; }

            private set
            {
                _currentWord = value;

                try
                {
                    while (true)
                    {
                        char character = _reader.GetNextChar();

                        if (char.IsLetter(character))
                        {
                            _currentWord += character;
                            continue; // lets try and grab more letters
                        }

                        // Have we found a word yet?
                        if (_currentWord != null) break;
                    }
                }
                catch (EndOfStreamException) { }
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
