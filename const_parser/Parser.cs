using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace const_parser
{
    class Parser
    {
        private Lexer lexer;

        public Parser(string filePath)
        {
            this.lexer = new Lexer(filePath);
        }

        public void RunParser()
        {
            var i = 0;
            while (i++ < 1)
            {
                var part = this.ParsePart();
                Console.WriteLine(part);
            }
        }

        private void AssertEqual(Kinds actual, Kinds expected)
        {
            if (actual != expected)
            {
                throw new Exception($"Found {actual} | Expected {expected}");
            }
        }

        private Node ParseArticle()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Article);

            var index = el.Text.IndexOf(" ");
            var node = new Node(k)
            {
                Text = el.Text.Substring(index + 1),
                Number = el.Text.Substring(0, index)
            };

            ParseDescription();
            var nextType = this.lexer.PeekNextElement().Type;
            while (string.Equals(nextType, "7"))
            {
                var nextEl = this.lexer.GetNextElement();

                node.Text += " " + nextEl.Text;

                ParseDescription();
                nextType = this.lexer.PeekNextElement().Type;
            }

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind != Kinds.Article && nextKind != Kinds.Part && nextKind != Kinds.Schedule)
            {
                switch (nextKind)
                {
                    case Kinds.Text:
                        node.Children.AddLast(ParseText());
                        break;
                    case Kinds.Clause:
                        node.Children.AddLast(ParseClause());
                        break;
                    case Kinds.Sub_Clause:
                        this.lexer.GetNextElement();
                        break;
                    default:
                        /*throw new Exception($"{el} | expected category or article");*/
                        this.lexer.GetNextElement();
                        break;
                }
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;

        }

        private void ParseDescription()
        {
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Description)
            {
                var nextEl = this.lexer.GetNextElement();

                //node.Text += " " + nextEl.Text;

                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }
        }

        private Node ParseClause()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Clause);

            var textEl = ParseText();

            var node = new Node(k)
            {
                Text = textEl.Text,
                Number = el.Text
            };

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Sub_Clause)
            {
                node.Children.AddLast(ParseSubClause());
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;
        }

        private Node ParseSubClause()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Sub_Clause);

            var textEl = ParseText();

            var node = new Node(k)
            {
                Text = textEl.Text,
                Number = el.Text
            };

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Sub_Sub_Clause)
            {
                //node.Children.AddLast(parseSubClause());
                this.lexer.GetNextElement();
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;
        }

        private Node parseSubClause()
        {
            return null;
        }

        private Node ParseCategory()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Category);

            var node = new Node(k)
            {
                Text = el.Text
            };

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Article)
            {
                node.Children.AddLast(ParseArticle());
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }



            return node;
        }

        private Node ParseText()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Text);

            var node = new Node(k)
            {
                Text = el.Text
            };

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Text)
            {
                var nextEl = this.lexer.GetNextElement();

                node.Text += " " + nextEl.Text;

                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;
        }

        private Node ParsePart()
        {
            var el = this.lexer.GetNextElement();
            AssertEqual(this.GetKind(el), Kinds.Part);
            var node = new Node(this.GetKind(el))
            {
                Text = el.Text
            };

            ParseDescription();
            var nextType = this.lexer.PeekNextElement().Type;
            while (string.Equals(nextType, "9"))
            {
                var nextEl = this.lexer.GetNextElement();

                node.Text += " " + nextEl.Text;

                ParseDescription();
                nextType = this.lexer.PeekNextElement().Type;
            }

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind != Kinds.Part && nextKind != Kinds.Schedule)
            {
                switch (nextKind)
                {
                    case Kinds.Category:
                        node.Children.AddLast(ParseCategory());
                        break;
                    case Kinds.Article:
                        node.Children.AddLast(ParseArticle());
                        break;
                    case Kinds.Description:
                        this.lexer.GetNextElement();
                        break;
                    default:
                        /*throw new Exception($"{el} | expected category or article");*/
                        this.lexer.GetNextElement();
                        break;
                }
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;
        }

        private Kinds GetKind(Element el)
        {
            var numberRegEx = new Regex("^([0-9])");
            switch (el.Type)
            {
                case "9":
                    return el.Text.ToLower().StartsWith("part") 
                            ? Kinds.Part
                            : Kinds.Schedule;
                case "7":
                    return el.Text.ToLower().StartsWith("explanation")
                            ? Kinds.Explanation
                            : numberRegEx.IsMatch(el.Text.Substring(0,1))
                                ? Kinds.Article
                                : Kinds.Category;
                case "6":
                    return numberRegEx.IsMatch(el.Text.Substring(0, 1))
                        ? Kinds.Clause
                        : Kinds.Sub_Clause;
                case "5":
                    return Kinds.Text;
                case "4":
                    return Kinds.Description;
                default:
                    throw new Exception($"unsupported type {el.Text}");
            }
        }
    }
}
