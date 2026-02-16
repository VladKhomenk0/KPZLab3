using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Task6
{
    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
    }

    public class LightTextNode : LightNode
    {
        private string _text;
        public LightTextNode(string text) => _text = text;
        public override string OuterHTML => _text;
    }
    
    public class LightElementInfo
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }

        public LightElementInfo(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }
    }
    
    public static class LightElementFactory
    {
        private static Dictionary<string, LightElementInfo> _cache = new Dictionary<string, LightElementInfo>();

        public static LightElementInfo GetInfo(string tagName, string displayType, string closingType)
        {
            string key = $"{tagName}_{displayType}_{closingType}";

            if (!_cache.ContainsKey(key))
            {
                _cache[key] = new LightElementInfo(tagName, displayType, closingType);
            }
            return _cache[key];
        }
    }
    
    public class LightElementNode : LightNode
    {
        private LightElementInfo _info;
        public List<LightNode> Children { get; } = new List<LightNode>();

        public LightElementNode(string tagName, string displayType, string closingType)
        {
            _info = LightElementFactory.GetInfo(tagName, displayType, closingType);
        }

        public void AddChild(LightNode node) => Children.Add(node);

        public override string OuterHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"<{_info.TagName}>");
                foreach (var child in Children) sb.Append(child.OuterHTML);
                if (_info.ClosingType != "single") sb.Append($"</{_info.TagName}>");
                return sb.ToString();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Легковаговик\n");

            string bookText = @"Title
Shenanigans in the Park.
  The quick brown fox jumps over the lazy dog.
Another line here.
  Something strictly strictly strictly.
Short.
End.";
            string[] lines = bookText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            LightElementNode root = new LightElementNode("div", "block", "double");
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                LightElementNode node;

                if (i == 0)
                {
                    node = new LightElementNode("h1", "block", "double");
                }
                else if (line.StartsWith(" "))
                {
                    node = new LightElementNode("blockquote", "block", "double");
                }
                else if (line.Length < 20)
                {
                    node = new LightElementNode("h2", "block", "double");
                }
                else
                {
                    node = new LightElementNode("p", "block", "double");
                }

                node.AddChild(new LightTextNode(line.Trim()));
                root.AddChild(node);
            }

            Console.WriteLine("HTML дерево побудовано:");
            Console.WriteLine(root.OuterHTML);
            
            long memoryUsed = GC.GetTotalMemory(true);
            Console.WriteLine($"\nПам'ять, використана процесом: {memoryUsed} байт");
            
            Console.ReadKey();
        }
    }
}