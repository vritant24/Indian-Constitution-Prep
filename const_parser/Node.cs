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
        public LinkedList<string> Descriptions;
        public LinkedList<Node> Children;
        public string Text;
        public Node(Kinds kind)
        {
            this.Kind = kind;
            this.Children = new LinkedList<Node>();
            this.Descriptions = new LinkedList<string>();
            this.Number = "";
        }

        public override string ToString()
        {
            return $"[Kind: {Kind}; Number: {Number}\n{Text}\n{string.Join(",\n", Children.Select(c=>c.ToString()).ToArray())}\n]";
        }
    }
}
