using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace const_parser
{
    internal class Node
    {
        public Kinds Kind { get; private set; }
        public string Number;

        private readonly LinkedList<string> descriptions;
        public LinkedList<string> Descriptions
        {
            get => this.descriptions;
            set
            {
                foreach (var desc in value)
                {
                    this.descriptions.AddLast(desc);
                }
            }
        }

        public LinkedList<Node> Children;
        public string Text;
        public Node(Kinds kind)
        {
            this.Kind = kind;
            this.Children = new LinkedList<Node>();
            this.descriptions = new LinkedList<string>();
            this.Number = "";
        }

        public override string ToString()
        {
            return $"[Kind: {Kind}; Number: {Number}\n{Text}\n{string.Join(",\n", Children.Select(c=>c.ToString()).ToArray())}\n]";
        }
    }
}
