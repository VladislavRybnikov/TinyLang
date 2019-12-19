﻿using System;
using System.Collections.Generic;
using System.Linq;
using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using TinyLang.Fluent.Models;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.Fluent
{
    public interface ITinyLangEngine : IDisposable
    {
        Statements Statements { get; }

        ITinyLangEngine SetVariable(string name, int val);

        ITinyLangEngine SetVariable(string name, bool val);

        ITinyLangEngine SetVariable(string name, string val);

        ITinyLangEngine SetVariable(string name, IStatement val);

        ITinyLangEngine SetVariableDynamic(string name, string val);

        ITinyLangEngine DefineType(string name, Dictionary<string, string> args);

        ITinyLangEngine AddStatement(IStatement st);

        void Execute();

        void Execute(out AST ast);
    }

    public class TinyLangEngine : ITinyLangEngine
    {
        private IASTBuilder _astBuilder;
        private Statements _statements;

        IASTBuilder ASTBuilder => _astBuilder 
            ?? (_astBuilder = new ASTBuilder(new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer()));

        private TinyLangEngine() 
        {
            _astBuilder = new ASTBuilder(new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer());
            _statements = new Statements(_astBuilder);
        }

        public static ITinyLangEngine Empty
        {
            get
            {
                var e = new TinyLangEngine();
                e.ASTBuilder.Empty();
                return e;
            }
        }

        public Statements Statements => _statements ?? (_statements = new Statements(_astBuilder));

        public static ITinyLangEngine FromScript(string script)
        {
            var self = new TinyLangEngine();
            self.ASTBuilder.FromStr(script);
            return self;
        }

        public ITinyLangEngine DefineType(string name, Dictionary<string, string> args)
        {
            var typeDefinition = new RecordExpr(name, args.Select(kv => new TypedVar(kv.Key, kv.Value)));

            ASTBuilder.AddStatement(typeDefinition);

            return this;
        }

        public void Dispose() => _astBuilder = null;

        public void Execute() => Execute(out _);

        public ITinyLangEngine SetVariable(string name, int val) => SetVariable(name, new Val(val));

        public ITinyLangEngine SetVariable(string name, bool val) => SetVariable(name, new Val(val));

        public ITinyLangEngine SetVariable(string name, string val) => SetVariable(name, new Val(val));

        public ITinyLangEngine SetVariableDynamic(string name, string val)
        {
            throw new NotImplementedException();
        }

        public ITinyLangEngine SetVariable(string name, IStatement val)
        {
            var variable = new VarExpr(name);
            var assignment = new AssignExpr(variable, val.GetExpr());
            ASTBuilder.AddStatement(assignment);
            return this;
        }

        public void Execute(out AST ast)
        {
            ast = ASTBuilder.Build();

            TinyCompiler.Create(ASTBuilder,
                CodeGeneratorsFactory.Instance)
                .WithAssemblyName("engineInteractive")
                .RunFromAst(ast);
        }

        public ITinyLangEngine AddStatement(IStatement st)
        {
            ASTBuilder.AddStatement(st.GetExpr());
            return this;
        }
    }
}