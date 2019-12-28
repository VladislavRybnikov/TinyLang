using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using TinyLang.Fluent;
using TinyLang.IDE.Services.ScriptAnalyze.Models;
using TinyLang.IDE.Utils.Extensions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.IDE.Services.ScriptAnalyze
{
    public interface IScriptAnalyzer 
    {
        Task<IEnumerable<Tag>> AnalyzeForTagsAsync(string text);
    }

    public class ScriptAnalyzer : IScriptAnalyzer
    {
        public ScriptAnalyzer() 
        {
        }

        public async Task<IEnumerable<Tag>> AnalyzeForTagsAsync(string text) 
        {
            return await Task.Run(() => 
            {
                return TinyLangEngine.Empty.NewASTBuilder.FromStr(text).Build()
                    .SelectMany(GetTags);
            })
            .ConfigureAwait(false);
        }

        private IEnumerable<Tag> GetTags(Expr e) 
        {
            return e switch
            {
                FuncInvocationExpr f => GetTagsFromFuncInvocation(f),
                StrExpr s => FromExpr(s)
                    .With(t => t.Value = s.Value)
                    .With(t => t.Type = TagType.StringLiteral)
                    .AsSingle(),
                AssignExpr a => GetTags(a.Value),
                _ => Enumerable.Empty<Tag>()
            };
        }

        private IEnumerable<Tag> GetTagsFromFuncInvocation(FuncInvocationExpr f) 
        {
            yield return FromExpr(f).With(t =>
            {
                t.Value = f.Name;
                t.Type = TagType.FuncName;
            });

            foreach (var tag in f.Args.SelectMany(GetTags)) 
            {
                yield return tag;
            }
        }

        private Tag FromExpr(Expr e)
        {
            return new Tag {
                Line = e.Pos.Line,
                Column = e.Pos.Column
            };
        }
    }
}
