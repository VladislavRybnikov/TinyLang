using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.IDE.Core;

namespace TinyLang.IDE.Services.ScriptRunning
{
    public class TreeViewIcon
    {
        public Image Icon { get; set; }
        public string Description { get; set; }

        public TreeViewIcon(string name, string description)
        {
            Icon = GetIcon(name);
            Description = description;
        }

        private Image GetIcon(string name)
        {
            var path = new Uri(Path.Combine(Resources.IconFolder, name), UriKind.RelativeOrAbsolute);

            var icon = new PngBitmapDecoder(path, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            return new Image
            {
                Height = 21,
                Width = 21,
                Source = icon.Frames[0]
            };
        }
    }

    public class TreeViewIconEnricher : Enricher<TreeViewItem, TreeViewIcon>
    {
        public override void Enrich(TreeViewItem value)
        {
            base.Enrich(value);

            foreach (var ch in value.Items.Cast<TreeViewItem>().Where(x => x != null))
            {
                Enrich(ch);
            }
        }

        protected override void Enrich(TreeViewItem value, TreeViewIcon enrichment)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            panel.Children.Add(enrichment.Icon);
            panel.Children.Add(new TextBlock(new Run(" " + value.Header + $" ({enrichment.Description})")));

            value.Header = panel;

            foreach (var ch in value.Items.Cast<TreeViewItem>().Where(x => x != null))
            {
                Enrich(ch);
            }
        }

        protected override TreeViewIcon SelectEnrichment(TreeViewItem value)
        {
            var nodeType = value.Items.Cast<TreeViewItem>().Select(GetNodeType).FirstOrDefault(x => x != null);

            return nodeType switch
            {
                NodeType.FuncInvocation => new TreeViewIcon("Function-PNG-Image.png", "Invoke"),
                NodeType.VariableAssign => new TreeViewIcon("brackets-27345_960_720.png", "Assign"),
                NodeType.ConditionExpression => new TreeViewIcon("condition.png", "Condition"),
                _ => null
            };
        }

        private NodeType? GetNodeType(TreeViewItem item)
        {
            if (item.Header is string s)
            {
                var regex = new Regex(@"nodeType:\s*(\w+)");
                var match = regex.Match(s);

                return match.Success ? match.Groups[1].Value.AsNodeType() : null;
            }
            return null;
        }


    }
}
