using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TinyLang.IDE.Utils
{
    public class ControlWriter : TextWriter
    {
        private TextBoxBase textbox;

        public ControlWriter(TextBoxBase textbox) => this.textbox = textbox;

        public override void Write(char value) => textbox.AppendText(value.ToString());

        public override void Write(string value) => textbox.AppendText(value);

        public override Encoding Encoding => Encoding.ASCII;
    }
}
