using System;
using System.Collections.Generic;

namespace Core
{
    public interface IParserService<T>
    {
        IEnumerable<IEnumerable<T>> ParseText(string inputText);
    }
}
