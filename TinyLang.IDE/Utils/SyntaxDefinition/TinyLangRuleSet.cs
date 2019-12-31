using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyLang.IDE.Utils.SyntaxDefinition
{
    public class TinyLangRuleSet
    {
        private static IEnumerable<(ColorType color, string pattern)> _rules = new List<(ColorType color, string pattern)>
        {
            (ColorType.FuncKeyword, "func "),
            (ColorType.FuncName, @"\b[\d\w_]+(?=\s*\()"),
            (ColorType.StringLiteral, "\"((\\.)|[^\\\\\"])*\""),
            (ColorType.TypeDecalration, @":\s?([\w]+)"),
            (ColorType.TypeKeyword, "type "),
            (ColorType.ReturnKeyword, "return "),
            (ColorType.NewKeyword, "new ")
        };

        public static HighlightingRuleSet GetHighlightingRuleSet(string name, IDictionary<ColorType, HighlightingColor> colorsCollection) 
        {
            var ruleSet = new HighlightingRuleSet()
            {
                Name = name
            };

            _rules.Select(x => new HighlightingRule
            {
                Color = colorsCollection[x.color],
                Regex = new Regex(x.pattern)
            }).ToList().ForEach(ruleSet.Rules.Add);

            return ruleSet;
        }
    }
}
