using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Types
{
    public class Word : Term
    {
        public readonly string Value;
        public Word(string value) => Value = value;
    }
}
