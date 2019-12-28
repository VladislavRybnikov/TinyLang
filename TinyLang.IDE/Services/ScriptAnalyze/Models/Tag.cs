using System;
using System.Windows.Documents;

namespace TinyLang.IDE.Services.ScriptAnalyze.Models
{
    public enum TagType 
    {
        FuncName, 
        TypeName,
        VarName,
        RecordName,
        StringLiteral
    }

    public class Tag
    {
        public Tag With(Action<Tag> action) 
        {
            action(this);
            return this;
        }

        public TagType Type { get; set; }

        public string Value { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Tag t) 
            {
                return Type.Equals(t.Type) &&
                       Value.Equals(t.Value) &&
                       Line.Equals(t.Line) &&
                       Column.Equals(t.Column);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

