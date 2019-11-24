using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace const_parser
{
    class Parser
    {
        private Lexer lexer;
        private DescriptionProvider descriptionProvider;

        public Parser(string filePath)
        {
            this.lexer = new Lexer(filePath);
            this.descriptionProvider = new DescriptionProvider();
        }

        public void RunParser()
        {
            var i = 0;
            const string path = @"C:\Users\Administrator\source\repos\const_parser\const_parser\parts\";
            while (i++ < 27)
            {
                var part = this.ParsePart();
                if(part!=null) NodeWriter.WriteNode(path + $"{part.Number}.json", part);
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
                node.Descriptions = this.descriptionProvider.GetDescList();
                var nextEl = this.lexer.GetNextElement();

                node.Text += " " + nextEl.Text;

                ParseDescription();
                nextType = this.lexer.PeekNextElement().Type;
            }

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind != Kinds.Article && nextKind != Kinds.Part && nextKind != Kinds.Schedule && nextKind != Kinds.Category)
            {
                Node nodeToAdd = null;
                var descs = this.descriptionProvider.GetDescList();
                switch (nextKind)
                {
                    case Kinds.Text:
                        nodeToAdd = ParseText(true);
                        break;
                    case Kinds.Clause:
                        nodeToAdd = ParseClause();
                        break;
                    case Kinds.Sub_Clause:
                        nodeToAdd = ParseSubClause();
                        break;
                    case Kinds.Explanation:
                        nodeToAdd = ParseExplanation();
                        break;
                    default:
                        throw new Exception($"{el} | expected category or article");
                }
                nodeToAdd.Descriptions = descs;
                node.Children.AddLast(nodeToAdd);
                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;

        }

        private void ParseDescription()
        {
            var list = new LinkedList<string>();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Description)
            {
                var nextEl = this.lexer.GetNextElement();

                this.descriptionProvider.AddDesc(nextEl.Text);

                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }
        }

        private Node ParseClause()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Clause);

            var node = new Node(k)
            {
                Number = el.Text
            };

            ParseDescription();
            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            if(nextKind == Kinds.Text)
            {
                node.Text = ParseText(false).Text;
            }
            node.Descriptions = this.descriptionProvider.GetDescList();

            ParseDescription();
            nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Sub_Clause)
            {
                var descs = this.descriptionProvider.GetDescList();
                var nodeToAdd = ParseSubClause();
                nodeToAdd.Descriptions = descs;
                node.Children.AddLast(nodeToAdd);

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

            var node = new Node(k)
            {
                Number = el.Text
            };

            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            if(nextKind == Kinds.Text)
            {
                node.Text = ParseText(false).Text;
                node.Descriptions = this.descriptionProvider.GetDescList();
            } else if (nextKind == Kinds.Sub_Clause)
            {
                var descs = this.descriptionProvider.GetDescList();
                node.Children.AddLast(ParseSubClause());
                node.Descriptions = descs;
            } else
            {
                throw new Exception($"{nextKind} not supported after clause");
            }

            return node;
        }

        private Node ParseExplanation()
        {
            var el = this.lexer.GetNextElement();
            var k = this.GetKind(el);
            AssertEqual(k, Kinds.Explanation);

            var node = new Node(k)
            {
                Number = el.Text
            };

            var nextKind = this.GetKind(this.lexer.PeekNextElement());
            while (nextKind == Kinds.Text)
            {
                node.Text = ParseText(false).Text;

                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            node.Descriptions = this.descriptionProvider.GetDescList();

            return node;
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
                var descs = this.descriptionProvider.GetDescList();
                var nodeToAdd = ParseArticle();
                nodeToAdd.Descriptions = descs;
                node.Children.AddLast(nodeToAdd);

                ParseDescription();
                nextKind = this.GetKind(this.lexer.PeekNextElement());
            }

            return node;
        }

        private Node ParseText(bool storeDesc)
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
            if(storeDesc)
                node.Descriptions = this.descriptionProvider.GetDescList();
            return node;
        }

        private Node ParsePart()
        {
            var el = this.lexer.GetNextElement();
            if (this.GetKind(el) == Kinds.Schedule) return null;
            AssertEqual(this.GetKind(el), Kinds.Part);

            var index = el.Text.IndexOf(":");
            var node = new Node(this.GetKind(el))
            {
                Text = el.Text.Substring(index + 1),
                Number = el.Text.Substring(0, index)
            };

            ParseDescription();
            var nextType = this.lexer.PeekNextElement().Type;
            while (string.Equals(nextType, "9"))
            {
                node.Text += " " + this.lexer.GetNextElement().Text;
                node.Descriptions = this.descriptionProvider.GetDescList();
                ParseDescription();
                nextType = this.lexer.PeekNextElement().Type;
            }

            ParseDescription();
            var nextEl = this.lexer.PeekNextElement();
            while (this.GetKind(nextEl) != Kinds.Part && this.GetKind(nextEl) != Kinds.Schedule)
            {
                var descs = this.descriptionProvider.GetDescList();
                Node nodeToAdd;
                switch (this.GetKind(nextEl))
                {
                    case Kinds.Category:
                        nodeToAdd = ParseCategory();
                        break;
                    case Kinds.Article:
                        nodeToAdd = ParseArticle();
                        break;
                    case Kinds.Text:
                        nodeToAdd = ParseText(true);
                        break;
                    default:
                        throw new Exception($"{nextEl} | expected category or article");
                }
                nodeToAdd.Descriptions = descs;
                node.Children.AddLast(nodeToAdd);
                ParseDescription();
                nextEl = this.lexer.PeekNextElement();
            }

            return node;
        }

        private Kinds GetKind(Element el)
        {
            var numberRegEx = new Regex("^([0-9])");
            var alphRegEx = new Regex("^([a-z])");
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
