using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace TinyLang.IDE.Utils.SyntaxDefinition
{
    public class TinyLangDefinition : IHighlightingDefinition
    {
        private Dictionary<ColorType, HighlightingColor> _colors = new Dictionary<ColorType, HighlightingColor>
        {
            [ColorType.FuncKeyword] = BoldForeground(Colors.Blue, ColorType.FuncKeyword.ToString()),
            [ColorType.FuncName] = BoldForeground(Colors.BlueViolet, ColorType.FuncName.ToString()),
            [ColorType.StringLiteral] = Foreground(Colors.Orange, ColorType.StringLiteral.ToString()),
            [ColorType.TypeKeyword] = BoldForeground(Colors.Blue, ColorType.TypeKeyword.ToString()),
            [ColorType.TypeDecalration] = BoldForeground(Colors.Teal, ColorType.TypeKeyword.ToString()),
            [ColorType.ReturnKeyword] = BoldForeground(Colors.Blue, ColorType.ReturnKeyword.ToString()),
            [ColorType.NewKeyword] = BoldForeground(Colors.Blue, ColorType.NewKeyword.ToString())
        };

        public static HighlightingColor Foreground(Color color, string name)
            => new HighlightingColor() { Foreground = new SimpleHighlightingBrush(color), Name =  name};

        public static HighlightingColor BoldForeground(Color color, string name)
           => new HighlightingColor() { Foreground = new SimpleHighlightingBrush(color), Name = name, FontWeight = FontWeights.Bold };

        public string Name => "TinyLang";

        public HighlightingRuleSet MainRuleSet => TinyLangRuleSet.GetHighlightingRuleSet("TinyLang", _colors);

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
