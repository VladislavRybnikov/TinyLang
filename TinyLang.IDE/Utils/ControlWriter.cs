using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace TinyLang.IDE.Utils
{
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;

        public ControlWriter(TextBox textbox) => this.textbox = textbox;

        public override void Write(char value) => textbox.Text += value;

        public override void Write(string value) => textbox.Text += value;

        public override Encoding Encoding => Encoding.ASCII;
    }
}
