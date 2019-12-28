using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace TinyLang.IDE.Utils
{
    public class TxtRange : TextRange
    {
        public TxtRange(TextPointer position1, TextPointer position2) : base(position1, position2)
        {
        }

        public static implicit operator TxtRange((TextPointer from, TextPointer to) range) 
            => new TxtRange(range.from, range.to);
    }

    public static class TxtRageExt
    {
        public static TxtRange AsTxtRange(this ValueTuple<TextPointer, TextPointer> range) 
            => range;

        public static TxtRange FullRange(this FlowDocument document) 
            => (document.ContentStart, document.ContentEnd);
    }
}
