using System.IO;
using RedGateTests;

namespace RedGate.Shared
{
    public class TestStringReader : ICharacterReader
    {
        private int m_Pos = 0;
        private string m_Content = "It was the best of times, it was the worst of times";

        public char GetNextChar()
        {
            if (m_Pos >= m_Content.Length)
                throw new EndOfStreamException();

            return m_Content[m_Pos++];
        }

        public void Dispose()
        {
        }
    }
}
