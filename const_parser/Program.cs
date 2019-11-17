using System;

namespace const_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\Administrator\Desktop\constitution.html";

            var parser = new Parser(path);
            parser.RunParser();
        }
    }
}
