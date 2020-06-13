using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Common.Attributes
{
    public class ApplicableForAttribute : Attribute
    {
        public ApplicableForAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
