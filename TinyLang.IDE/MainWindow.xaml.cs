using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using TinyLang.Fluent;

namespace TinyLang.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ControlWriter : TextWriter
    {
    private TextBox textbox;
    public ControlWriter(TextBox textbox)
    {
        this.textbox = textbox;
    }

    public override void Write(char value)
    {
        textbox.Text += value;
    }

    public override void Write(string value)
    {
        textbox.Text += value;
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
    }
public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.SetOut(new ControlWriter(txtBx2));
            btn1.Click += (s, e) =>
            {
                tv1.Items.Clear();

                using var engine = TinyLangEngine
                    .FromScript(txtBx1.Text);

                engine.Execute(out var ast);
                var astJson = JToken.Parse(ast.ToString());
                tv1.Items.Add(JsonToTree(astJson, "AST"));
            };
        }

        public static TreeViewItem JsonToTree(JToken root, string rootName = "")
        {
            var parent = new TreeViewItem{Header = rootName};

            ((root as JObject)?.Properties() ?? Enumerable.Empty<JProperty>())
                .Select(AsTreeItem)
                .ToList().ForEach(x => parent.Items.Add(x));

            return parent;
        }

        public static TreeViewItem JsonObjectToTree(JObject obj, string key)
        {
            var parent = new TreeViewItem { Header = key };

            foreach (var prop in obj?.Properties() ?? Enumerable.Empty<JProperty>())
            {
                var name = prop.Name;
                parent.Items.Add(AsTreeItem(prop));
            }

            return parent;
        }

        public static TreeViewItem JsonArrayToTree(JArray arr, string key)
        {
            var parent = new TreeViewItem { Header = key };

            foreach (var (jToken, i1) in arr.Select((v, i) => (v, i)))
            {
                parent.Items.Add(AsTreeItem(new JProperty(i1.ToString(), jToken)));
            }

            return parent;
        }

        public static TreeViewItem JsonPropertyToTree(JProperty jp) =>
            new TreeViewItem {Header = jp.Name + ": " + jp.Value};

        public static TreeViewItem AsTreeItem(JProperty jp)
            => jp.Value.Type switch
            {
                JTokenType.Object => JsonObjectToTree(jp.Value as JObject, jp.Name),
                JTokenType.Array => JsonArrayToTree(jp.Value as JArray, jp.Name),
                _ => JsonPropertyToTree(jp)
            };
    }
}
