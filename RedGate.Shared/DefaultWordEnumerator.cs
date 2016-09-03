using System.IO;
using RedGateTests;

namespace RedGate.Shared
{
    internal class DefaultWordEnumerator
    {
        public string GetNextWord(ICharacterReader reader)
        {
            string word = string.Empty;
            try
            {
                while (true)
                {
                    char character = reader.GetNextChar();

                    if (char.IsLetter(character))
                    {
                        word += character;
                        continue; // lets try to grab more letters
                    }

                    if (!string.IsNullOrEmpty(word)) return word;
                }
            }
            catch (EndOfStreamException)
            {
                // get the last word read before the exception was thrown
                return !string.IsNullOrEmpty(word) ? word : string.Empty;
            }

        }
    }
}
