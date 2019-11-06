using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace TinyLang.Compiler.Core.Parsing
{
    abstract class Term { }

    class Word : Term
    {
        public readonly string Value;
        public Word(string value) => Value = value;
    }

    class Number : Term
    {
        public readonly int Value;
        public Number(int value) => Value = value;
    }

    class Symbol : Term
    {
        public readonly char Value;

        public Symbol(char value) => Value = value;
    }

    public class SingleScriptParser
    {
        public static void Parse(string script)
        {
            var parser = many1(letter);
            var spaces = many(satisfy(char.IsWhiteSpace));

            var word = from w in asString(many1(letter))
                       from s in spaces
                       select new Word(w) as Term;

            var number = from d in asString(many1(digit))
                         from s in spaces
                         select new Number(int.Parse(d)) as Term;

            var equalsOperation = from e in ch('=')
                                  from s in spaces
                                  select new Symbol(e) as Term;

            var symbol = from e in symbolchar
                         from s in spaces
                         select new Symbol(e) as Term;

            var term = either(word, number);

            //var expression = chain(word, term, symbolchar);
            //var wordOrExpr = either(term, expression)
            var variableAsign = chain(word, equalsOperation, term);

            var result = parse(variableAsign, script);
        }
    }
}
