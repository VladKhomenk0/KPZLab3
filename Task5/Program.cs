using System;
using System.Collections.Generic;
using System.Text;

namespace Task5
{
    // ВІДВІДУВАЧ
    public interface ILightNodeVisitor
    {
        void Visit(LightElementNode element);
        void Visit(LightTextNode text);
    }

    public class TextExtractorVisitor : ILightNodeVisitor
    {
        public StringBuilder ExtractedText { get; } = new StringBuilder();

        public void Visit(LightElementNode element)
        {
            foreach (var child in element.Children)
                child.Accept(this);
        }

        public void Visit(LightTextNode text)
        {
            ExtractedText.Append(text.InnerHTML + " ");
        }
    }

    public abstract class LightNode
    {
        public abstract string InnerHTML { get; }
        public abstract string OuterHTML { get; }
        
        public abstract void Accept(ILightNodeVisitor visitor);
    }

    public class LightTextNode : LightNode
    {
        private string _text;
        public LightTextNode(string text) { _text = text; }
        public override string OuterHTML => _text;
        public override string InnerHTML => _text;
        
        public override void Accept(ILightNodeVisitor visitor) => visitor.Visit(this);
    }

    public class LightElementNode : LightNode
    {
        public string TagName { get; }
        public string ClosingType { get; }
        public List<LightNode> Children { get; } = new List<LightNode>();

        public LightElementNode(string tagName, string closingType)
        {
            TagName = tagName;
            ClosingType = closingType;
        }

        public override string OuterHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"<{TagName}>");
                if (ClosingType != "single")
                {
                    sb.Append(InnerHTML);
                    sb.Append($"</{TagName}>");
                }
                return sb.ToString();
            }
        }

        public override string InnerHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var child in Children) sb.Append(child.OuterHTML);
                return sb.ToString();
            }
        }

        public override void Accept(ILightNodeVisitor visitor) => visitor.Visit(this);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Відвідувач\n");
            
            var div = new LightElementNode("div", "double");
            var h1 = new LightElementNode("h1", "double");
            h1.Children.Add(new LightTextNode("Заголовок сайту."));
            
            var p = new LightElementNode("p", "double");
            p.Children.Add(new LightTextNode("Опис компанії та"));
            
            var span = new LightElementNode("span", "double");
            span.Children.Add(new LightTextNode("важливі деталі."));
            p.Children.Add(span);

            div.Children.Add(h1);
            div.Children.Add(p);

            Console.WriteLine("Оригінальний HTML");
            Console.WriteLine(div.OuterHTML);

            Console.WriteLine("\nЕкстракція тексту (Visitor)");
            var visitor = new TextExtractorVisitor();
            div.Accept(visitor);
            
            Console.WriteLine(visitor.ExtractedText.ToString().Trim());
        }
    }
}