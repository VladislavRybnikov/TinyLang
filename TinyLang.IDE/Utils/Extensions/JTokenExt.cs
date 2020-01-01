using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace TinyLang.IDE.Utils.Extensions
{
    public static class JTokenExt
    {
        public static TreeViewItem ToTreeViewItem(this JToken root, string rootName = "")
        {
            var parent = new TreeViewItem { Header = rootName };

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
            new TreeViewItem { Header = jp.Name + ": " + jp.Value };

        public static TreeViewItem AsTreeItem(JProperty jp)
            => jp.Value.Type switch
            {
                JTokenType.Object => JsonObjectToTree(jp.Value as JObject, jp.Name),
                JTokenType.Array => JsonArrayToTree(jp.Value as JArray, jp.Name),
                _ => JsonPropertyToTree(jp)
            };


    }
}
