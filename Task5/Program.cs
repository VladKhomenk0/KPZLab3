using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Task5
{
    // КОМАНДА
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class AddChildCommand : ICommand
    {
        private LightElementNode _parent;
        private LightNode _child;

        public AddChildCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child = child;
        }

        public void Execute() => _parent.Children.Add(_child);
        public void Undo() => _parent.Children.Remove(_child);
    }

    public class HtmlEditor
    {
        private Stack<ICommand> _history = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _history.Push(command);
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                Console.WriteLine("[Undo] Скасування останньої дії...");
                _history.Pop().Undo();
            }
        }
    }

    public abstract class LightNode
    {
        public abstract string InnerHTML { get; }
        public string Render()
        {
            OnCreated();
            string html = GetOuterHtmlCore();
            OnRendered();
            return html;
        }
        protected virtual void OnCreated() { }
        protected virtual void OnRendered() { }
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

        public LightElementNode(string tagName, string displayType, string closingType, List<string> cssClasses = null)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
            if (cssClasses != null) CssClasses = cssClasses;
        }

        protected override string GetOuterHtmlCore()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<{TagName}");
            if (CssClasses.Count > 0) sb.Append($" class=\"{string.Join(" ", CssClasses)}\"");
            sb.Append(">");
            if (ClosingType != "single")
            {
                sb.Append(InnerHTML);
                sb.Append($"</{TagName}>");
            }
            return sb.ToString();
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
            Console.WriteLine("Команда\n");
            
            var editor = new HtmlEditor();
            var div = new LightElementNode("div", "block", "double");
            var h1 = new LightElementNode("h1", "block", "double");
            var p = new LightElementNode("p", "block", "double");
            
            p.Children.Add(new LightTextNode("Помилковий текст"));
            
            Console.WriteLine("Виконуємо команди:");
            editor.ExecuteCommand(new AddChildCommand(div, h1));
            editor.ExecuteCommand(new AddChildCommand(div, p));
            
            Console.WriteLine("\nHTML до скасування:");
            Console.WriteLine(div.OuterHTML);

            Console.WriteLine("\nСкасовуємо останню дію (додавання <p>):");
            editor.Undo();

            Console.WriteLine("\nHTML після скасування:");
            Console.WriteLine(div.OuterHTML);
        }
    }
}