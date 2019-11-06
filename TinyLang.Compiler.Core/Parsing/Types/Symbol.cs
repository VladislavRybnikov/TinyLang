using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Types
{
    public class Symbol : Term
    {
        public readonly char Value;

        public Symbol(char value) => Value = value;
    }
}
