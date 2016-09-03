using System;
using System.Collections.Generic;

namespace RedGate.Shared
{
    public interface IWordFrequencyCounter : IDisposable
    {
        IDictionary<string, int> GetWordFrequency();
    }
}