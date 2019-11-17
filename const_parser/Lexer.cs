using System.Collections.Generic;
using System.IO;

namespace const_parser
{
    internal class Lexer
    {
        private StreamReader reader;
        private Queue<Element> buffer;

        public Lexer(string filePath)
        {
            var fileStream = File.OpenRead(filePath);
            this.reader = new StreamReader(fileStream);
            this.buffer = new Queue<Element>();
        }

        public Element GetNextElement()
        {
            if(this.buffer.Count == 0)
            {
                this.PopulateBuffer();
            }

            return this.buffer.Count == 0 ? null : this.buffer.Dequeue();
        }

        public Element PeekNextElement()
        {
            if (this.buffer.Count == 0)
            {
                this.PopulateBuffer();
            }

            return this.buffer.Count == 0 ? null : this.buffer.Peek();
        }

        private void PopulateBuffer()
        {
            var line = "";
            while (string.IsNullOrEmpty(line) && !this.reader.EndOfStream)
            {
                line = this.reader.ReadLine();
            }
            
            var elements = line.Split("</>");

            foreach (var el in elements)
            {
                el.Trim();
                if (string.IsNullOrEmpty(el))
                {
                    continue;
                }


                this.buffer.Enqueue(
                    new Element(
                        el.Substring(1, 1),
                        el.Substring(3).Trim()));
            }
        }
    }

    internal class Element
    {
        public readonly string Type;
        public readonly string Text;

        public Element(string type, string text)
        {
            this.Type = type;
            this.Text = text;
        }

        public override string ToString()
        {
            return $"{this.Type}: {this.Text}";
        }
    }
}
