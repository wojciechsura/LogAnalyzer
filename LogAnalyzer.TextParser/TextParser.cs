using LogAnalyzer.Models.Services.TextParser;
using LogAnalyzer.Services.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LogAnalyzer.TextParser
{
    public class TextParser : ITextParser
    {
        private (bool jsonParseResult, BaseTextPart part, int newIndex) TryParseJson(string text, int current)
        {
            int start = current;

            // Find end of JSON string

            if (text[current] != '{')
                throw new ArgumentException("Invalid JSON start char!");

            Stack<char> braces = new Stack<char>();
            braces.Push('{');
            bool inQuotes = false;
            bool escape = false;

            while (current < text.Length && braces.Count > 0)
            {
                current++;

                if (escape)
                {
                    escape = false;
                    continue;
                }

                if (current < text.Length)
                {
                    if (inQuotes)
                    {
                        if (text[current] == '"' && !escape)
                        {
                            inQuotes = false;
                        }
                        else if (text[current] == '\\')
                        {
                            escape = true;
                        }                        
                    }
                    else
                    {
                        if (text[current] == '{')
                        {
                            braces.Push('{');
                        }
                        else if (text[current] == '[')
                        {
                            braces.Push('[');
                        }
                        else if (text[current] == '}')
                        {
                            if (braces.Peek() == '{')
                                braces.Pop();
                            else
                                return (false, null, 0);
                        }
                        else if (text[current] == ']')
                        {
                            if (braces.Peek() == '[')
                                braces.Pop();
                            else
                                return (false, null, 0);
                        }
                        else if (text[current] == '"')
                        {
                            inQuotes = true;
                        }
                    }
                }
            }

            // Try to process JSON if possible

            if (current < text.Length && braces.Count == 0)
            {
                try
                {
                    JObject obj = JObject.Parse(text.Substring(start, current - start));

                    return (true, new JsonTextPart(obj), current);
                }
                catch 
                {
                    return (false, null, 0);
                }
            }
            else
            {
                return (false, null, 0);
            }
        }

        private (bool xmlParseResult, BaseTextPart part, int newIndex) TryParseXml(string text, int current)
        {
            return (false, null, 0);
        }

        public List<BaseTextPart> Parse(string text)
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
                else if (text[current] == '<')
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
                else
                {
                    buffer.Append(text[current]);
                    current++;
                    continue;
                }
            }

            if (buffer.Length > 0)
            {
                result.Add(new SimpleTextPart(buffer.ToString()));
                buffer.Clear();
            }

            return result;
        }
    }
}
