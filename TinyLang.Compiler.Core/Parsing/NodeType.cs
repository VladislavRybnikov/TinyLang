using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.Parsing
{
    public class ForTypeAttribute : Attribute
    {
        public ForTypeAttribute(Type type) {
            Type = type;
        }

        public Type Type { get; }
        public string Value => Type.Name.Replace("Expr", string.Empty);
    }

    public enum NodeType
    {
        [ForType(typeof(FuncInvocationExpr))]
        FuncInvocation,
        [ForType(typeof(AssignExpr))]
        VariableAssign,
        TypeDefinition,
        TypeCreation,
        [ForType(typeof(IfElseExpr))]
        ConditionExpression
    }

    public static class NodeTypeExt
    {
        public static NodeType? AsNodeType(this string val) 
        {
            var fields = typeof(NodeType).GetFields();

            var member = fields.FirstOrDefault(m => m.GetCustomAttributes(true).Cast<ForTypeAttribute>()
                  .Any(attr => attr.Value == val))?.Name;

            return Enum.TryParse<NodeType>(member, out var e) ? e : (NodeType?)null;
        }
    }
}
