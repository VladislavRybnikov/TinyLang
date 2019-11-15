﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing
{
    public static class ReservedNames
    {
        public const string If = "if";
        public const string Else = "else";
        public const string Elif = "elif";
        public const string While = "while";
        public const string Do = "do";
        public const string Record = "record";
        public const string Func = "func";
        public const string Return = "return";
        public const string New = "new";

        public static string[] All =
        {
            If, Else, Elif, While, Do, Record, Func, Return, New
        };
    }
}