using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class OpNotAllowedOnScopeException : PositionedException
    {
        public CodeGenerationScope Scope { get; }
        
        public string OperationName { get; }

        public OpNotAllowedOnScopeException(CodeGenerationScope scope, string opName, Position position) 
            : base(position, ComposeMsg(scope, opName, position))
        {
            Scope = scope;
            OperationName = opName;
        }

        public static string ComposeMsg(CodeGenerationScope scope, string opName, Position position)
            => $"Operation '{opName}' is not allowed in {scope} scope at {position}."; 
    }
}
