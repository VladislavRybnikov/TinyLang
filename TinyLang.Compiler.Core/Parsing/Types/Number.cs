using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Types
{
    public class Number : Term
    {
        public readonly int Value;
        public Number(int value) => Value = value;
    }
}
