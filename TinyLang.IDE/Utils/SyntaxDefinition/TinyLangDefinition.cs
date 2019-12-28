using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace TinyLang.IDE.Utils.SyntaxDefinition
{
    public enum ColorType 
    {
        FuncKeyWord, 
        FuncName,
        TypeKeyWord,
        TypeDecalration,
        StringLiteral

    }

    public class TinyLangDefinition : IHighlightingDefinition
    {
        private Dictionary<ColorType, HighlightingColor> _colors = new Dictionary<ColorType, HighlightingColor>
        {
            [ColorType.FuncKeyWord] = BoldForeground(Colors.Blue, ColorType.FuncKeyWord.ToString()),
            [ColorType.FuncName] = Foreground(Colors.BlueViolet, ColorType.FuncName.ToString()),
            [ColorType.StringLiteral] = Foreground(Colors.Orange, ColorType.StringLiteral.ToString()),
            [ColorType.TypeKeyWord] = Foreground(Colors.Blue, ColorType.TypeKeyWord.ToString()),
            [ColorType.TypeDecalration] = Foreground(Colors.Teal, ColorType.TypeKeyWord.ToString())
        };

        public static HighlightingColor Foreground(Color color, string name)
            => new HighlightingColor() { Foreground = new SimpleHighlightingBrush(color), Name =  name};

        public static HighlightingColor BoldForeground(Color color, string name)
           => new HighlightingColor() { Foreground = new SimpleHighlightingBrush(color), Name = name, FontWeight = FontWeights.Bold };

        public string Name => "TinyLang";

        public HighlightingRuleSet MainRuleSet
        {
            get
            {
                var ruleSet = new HighlightingRuleSet()
                {
                    Name = "TinyLang"
                };

                ruleSet.Rules.Add(new HighlightingRule
                {
                    Color = _colors[ColorType.FuncKeyWord],
                    Regex = new Regex("func ")
                    
                });
                ruleSet.Rules.Add(new HighlightingRule
                {
                    Color = _colors[ColorType.FuncName],
                    Regex = new Regex(@"\b[\d\w_]+(?=\s*\()")
                });
                ruleSet.Rules.Add(new HighlightingRule
                {
                    Color = _colors[ColorType.StringLiteral],
                    Regex = new Regex("\"((\\.)|[^\\\\\"])*\"")
                });
                ruleSet.Rules.Add(new HighlightingRule
                {
                    Color = _colors[ColorType.TypeDecalration],
                    Regex = new Regex(@":\s?([\w]+)")
                });
                return ruleSet;
            }
        }

        public IEnumerable<HighlightingColor> NamedHighlightingColors => _colors.Values;

        public IDictionary<string, string> Properties => new Dictionary<string, string>();

        public HighlightingColor GetNamedColor(string name)
        {
            if (Enum.TryParse<ColorType>(name, out var enumValue))
            {
                return _colors[enumValue];
            }
            return null;
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            return null;
        }
    }
}
