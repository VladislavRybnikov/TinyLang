using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Common.Attributes
{
    public class ParserOrderAttribute : Attribute
    {
        public ParserOrderAttribute(int order) 
        {
            Order = order;
        }

        public int Order { get; }
    }
}
