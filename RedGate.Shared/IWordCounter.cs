using System;
using System.Collections.Generic;

namespace RedGate.Shared
{
    public interface IWordCounter : IDisposable
    {
        IDictionary<string, int> GetWordCount();
    }
}