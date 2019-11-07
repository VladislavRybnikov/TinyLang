using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Token;

namespace TinyLang.Compiler.Core
{
    public static class TinyLanguage
    {
        public static GenLanguageDef LanguageDef { get; }

        public static GenTokenParser TokenParser => makeTokenParser(LanguageDef);

        static TinyLanguage()
        {
            LanguageDef = Language.JavaStyle;
        }
    }
}
