using System;
using System.Collections.Generic;
using System.Text;

namespace Task5
{
    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
        public abstract string InnerHTML { get; }
    }

    public class LightTextNode : LightNode
    {
        private string _text;

        public LightTextNode(string text)
        {
            _text = text;
        }

        public override string OuterHTML => _text;
        public override string InnerHTML => _text;
    }

    public class LightElementNode : LightNode
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }
        public List<string> CssClasses { get; } = new List<string>();
        public List<LightNode> Children { get; } = new List<LightNode>();

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
            if (cssClasses != null) CssClasses = cssClasses;
        }

        public void AddChild(LightNode node)
        {
            Children.Add(node);
        }

        public override string InnerHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var child in Children)
                {
                    sb.Append(child.OuterHTML);
                }
                return sb.ToString();
            }
        }

        public override string OuterHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"<{TagName}");
                if (CssClasses.Count > 0)
                {
                    sb.Append($" class=\"{string.Join(" ", CssClasses)}\"");
                }
                sb.Append(">");
                
                if (ClosingType != "single")
                {
                    sb.Append(InnerHTML);
                    sb.Append($"</{TagName}>");
                }

                return sb.ToString();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Компонувальник\n");
            
            var div = new LightElementNode("div", "block", "double", new List<string> { "container" });
            
            var h1 = new LightElementNode("h1", "block", "double");
            h1.AddChild(new LightTextNode("Привiт, Свiт!"));

            var ul = new LightElementNode("ul", "block", "double");
            
            var li1 = new LightElementNode("li", "block", "double");
            li1.AddChild(new LightTextNode("Item 1"));
            
            var li2 = new LightElementNode("li", "block", "double");
            li2.AddChild(new LightTextNode("Item 2"));

            ul.AddChild(li1);
            ul.AddChild(li2);

            div.AddChild(h1);
            div.AddChild(ul);

            Console.WriteLine("Згенерований HTML:");
            Console.WriteLine(div.OuterHTML);

            Console.ReadKey();
        }
    }
}