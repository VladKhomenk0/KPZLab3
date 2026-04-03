using System;
using System.Collections.Generic;
using System.Text;

namespace Task5
{
    // СТЕЙТ
    public interface INodeState
    {
        string GetHtml(LightElementNode node, string originalHtml);
    }

    public class VisibleState : INodeState
    {
        public string GetHtml(LightElementNode node, string originalHtml) => originalHtml;
    }

    public class HiddenState : INodeState
    {
        public string GetHtml(LightElementNode node, string originalHtml) 
            => $"";
    }

    public abstract class LightNode
    {
        public abstract string InnerHTML { get; }
        public string Render()
        {
            return GetOuterHtmlCore();
        }
        protected abstract string GetOuterHtmlCore();
        public string OuterHTML => Render(); 
    }

    public class LightTextNode : LightNode
    {
        private string _text;
        public LightTextNode(string text) { _text = text; }
        protected override string GetOuterHtmlCore() => _text;
        public override string InnerHTML => _text;
    }

    public class LightElementNode : LightNode
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }
        public List<string> CssClasses { get; } = new List<string>();
        public List<LightNode> Children { get; } = new List<LightNode>();
        
        // Змінна стану
        public INodeState State { get; set; } = new VisibleState();

        public LightElementNode(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }

        protected override string GetOuterHtmlCore()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<{TagName}>");
            if (ClosingType != "single")
            {
                sb.Append(InnerHTML);
                sb.Append($"</{TagName}>");
            }
            return State.GetHtml(this, sb.ToString());
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Стейт\n");
            
            var div = new LightElementNode("div", "block", "double");

            var h1 = new LightElementNode("h1", "block", "double");
            h1.Children.Add(new LightTextNode("Я видимий заголовок"));
            
            var secret = new LightElementNode("span", "inline", "double");
            secret.Children.Add(new LightTextNode("Секретний текст"));
            secret.State = new HiddenState(); // Зміна стану
            
            div.Children.Add(h1);
            div.Children.Add(secret);

            Console.WriteLine("Генерація HTML:");
            Console.WriteLine(div.OuterHTML);
        }
    }
}