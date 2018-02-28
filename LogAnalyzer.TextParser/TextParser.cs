using LogAnalyzer.Models.Services.TextParser;
using LogAnalyzer.Services.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using System.Web;
using System.Reflection;
using LogAnalyzer.Common.Tools;

namespace LogAnalyzer.TextParser
{
    public class TextParser : ITextParser
    {
        private int Move(string text, int current, int line, int position, bool zeroBasedPosition = false)
        {
            while (current < text.Length && (line > 1 || (!zeroBasedPosition && position > 1) || (zeroBasedPosition && position > 0)))
            {
                if (text[current] == '\n')
                {
                    if (line == 1)
                        return -1;

                    line--;
                }
                else if (line == 1)
                {
                    position--;
                }

                current++;
            }

            return current;
        }

        private (bool jsonParseResult, BaseTextPart part, int newIndex) TryParseJson(string text, int current)
        {
            try
            {
                string textPart = text.Substring(current);

                JObject obj = JObject.Parse(textPart);

                return (true, new JsonTextPart(obj), text.Length);
            }
            catch (JsonReaderException e)
            {
                int end = Move(text, current, e.LineNumber, e.LinePosition, true);

                try
                {
                    string textPart = text.Substring(current, end - current);

                    JObject obj = JObject.Parse(textPart);

                    return (true, new JsonTextPart(obj), end);
                }
                catch (JsonReaderException)
                {
                    return (false, null, 0);
                }
            }
        }

        private (bool xmlParseResult, BaseTextPart part, int newIndex) TryParseXml(string text, int current)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                string textPart = text.Substring(current);

                doc.Load(new StringReader(textPart));    

                return (true, new XmlTextPart(doc), text.Length);
            }
            catch (XmlException e)
            {
                int end = Move(text, current, e.LineNumber, e.LinePosition);

                try
                {
                    string textPart = text.Substring(current, end - current);

                    doc.Load(new StringReader(textPart));

                    return (true, new XmlTextPart(doc), end);
                }
                catch (XmlException)
                {
                    return (false, null, 0);
                }
            }
        }

        private List<BaseTextPart> Parse(string text)
        {
            var result = new List<BaseTextPart>();

            int current = 0;
            StringBuilder buffer = new StringBuilder();
            while (current < text.Length)
            {
                if (text[current] == '{')
                {
                    (bool jsonParseResult, BaseTextPart part, int newIndex) = TryParseJson(text, current);

                    if (jsonParseResult)
                    {
                        if (buffer.Length > 0)
                        {
                            result.Add(new SimpleTextPart(buffer.ToString()));
                            buffer.Clear();
                        }

                        result.Add(part);
                        current = newIndex;
                        continue;
                    }
                }

                if (text[current] == '<')
                {
                    (bool xmlParseResult, BaseTextPart part, int newIndex) = TryParseXml(text, current);

                    if (xmlParseResult)
                    {
                        if (buffer.Length > 0)
                        {
                            result.Add(new SimpleTextPart(buffer.ToString()));
                            buffer.Clear();
                        }

                        result.Add(part);
                        current = newIndex;
                        continue;
                    }
                }

                buffer.Append(text[current]);
                current++;
                continue;
            }

            if (buffer.Length > 0)
            {
                result.Add(new SimpleTextPart(buffer.ToString()));
                buffer.Clear();
            }

            return result;
        }

        private string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"LogAnalyzer.TextParser.Resources.{name}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string ParseMessageToHtml(string logMessage)
        {
            StringBuilder builder = new StringBuilder();

            var items = Parse(logMessage);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is SimpleTextPart simpleTextPart)
                {
                    using (HtmlTag.Open(builder, "pre"))
                        builder.Append(HttpUtility.HtmlEncode(simpleTextPart.Text));
                }
                else if (items[i] is JsonTextPart jsonTextPart)
                {
                    using (HtmlTag.Open(builder, "pre"))
                    using (HtmlTag.Open(builder, "code", "class=\"json\""))
                        builder.Append(HttpUtility.HtmlEncode(jsonTextPart.Json.ToString(Newtonsoft.Json.Formatting.Indented)));
                }
                else if (items[i] is XmlTextPart xmlTextPart)
                {
                    MemoryStream ms = new MemoryStream();
                    XmlTextWriter writer = new XmlTextWriter(ms, Encoding.Unicode)
                    {
                        Formatting = System.Xml.Formatting.Indented                        
                    };
                    xmlTextPart.Document.WriteContentTo(writer);
                    writer.Flush();
                    ms.Flush();

                    ms.Position = 0;
                    StreamReader reader = new StreamReader(ms);
                    string formattedXml = reader.ReadToEnd();

                    using (HtmlTag.Open(builder, "pre"))
                    using (HtmlTag.Open(builder, "code", "class=\"json\""))
                        builder.Append(HttpUtility.HtmlEncode(formattedXml));
                }
                else
                    throw new InvalidOperationException("Invalid text part!");
            }

            return builder.ToString();
        }

        public string ParseToHtmlPage(string logMessage)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<!DOCTYPE html>");

            using (HtmlTag.Open(builder, "html"))
            {
                using (HtmlTag.Open(builder, "head"))
                {
                    builder.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\" />");
                }

                using (HtmlTag.Open(builder, "body"))
                {
                    builder.Append(ParseMessageToHtml(logMessage));
                }
            }

            return builder.ToString();
        }
    }
}
