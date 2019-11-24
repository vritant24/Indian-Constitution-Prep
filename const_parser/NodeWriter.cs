using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace const_parser
{
    class NodeWriter
    {
        public static void WriteNode(string filePath, Node node)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                WriteNode(writer, node);

                using (StreamWriter w = File.CreateText(filePath))
                {
                    w.Write(sb.ToString());
                }
            }
        }

        private static void WriteNode(JsonWriter jw, Node node)
        {
            jw.WriteStartObject();

            jw.WritePropertyName("kind");
            jw.WriteValue(node.Kind);

            jw.WritePropertyName("number");
            jw.WriteValue(node.Number);

            jw.WritePropertyName("text");
            jw.WriteValue(node.Text);

            jw.WritePropertyName("descriptions");
            jw.WriteStartArray();
            foreach(var d in node.Descriptions)
            {
                jw.WriteValue(d);
            }
            jw.WriteEndArray();

            jw.WritePropertyName("children");
            jw.WriteStartArray();
            foreach (var child in node.Children)
            {
                WriteNode(jw, child);
            }
            jw.WriteEndArray();

            jw.WriteEndObject();
        }
    }
}
