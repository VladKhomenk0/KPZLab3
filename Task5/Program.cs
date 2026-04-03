using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Task5
{
    // ШАБЛОННИЙ МЕТОД
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

        // Хуки
        protected virtual void OnCreated() { Console.WriteLine($"[{this.GetType().Name}] створено."); }
        protected virtual void OnRendered() { Console.WriteLine($"[{this.GetType().Name}] відрендерено."); }
        
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

        public void AddChild(LightNode node) { Children.Add(node); }

        public override string InnerHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var child in Children) sb.Append(child.OuterHTML);
                return sb.ToString();
            }
        }

        // Реалізація генерації розмітки для шаблонного методу
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

        public IEnumerable<LightNode> GetDepthFirst()
        {
            var iterator = new DepthFirstIterator(this);
            while (iterator.MoveNext()) yield return iterator.Current;
        }
        public IEnumerable<LightNode> GetBreadthFirst()
        {
            var iterator = new BreadthFirstIterator(this);
            while (iterator.MoveNext()) yield return iterator.Current;
        }
    }

    public class DepthFirstIterator : IEnumerator<LightNode>
    {
        private Stack<LightNode> _stack = new Stack<LightNode>();
        private LightNode _current;
        public DepthFirstIterator(LightNode root) { _stack.Push(root); }
        public LightNode Current => _current;
        object IEnumerator.Current => Current;
        public bool MoveNext()
        {
            if (_stack.Count == 0) return false;
            _current = _stack.Pop();
            if (_current is LightElementNode elementNode)
                for (int i = elementNode.Children.Count - 1; i >= 0; i--) _stack.Push(elementNode.Children[i]);
            return true;
        }
        public void Reset() => throw new NotSupportedException();
        public void Dispose() { }
    }

    public class BreadthFirstIterator : IEnumerator<LightNode>
    {
        private Queue<LightNode> _queue = new Queue<LightNode>();
        private LightNode _current;
        public BreadthFirstIterator(LightNode root) { _queue.Enqueue(root); }
        public LightNode Current => _current;
        object IEnumerator.Current => Current;
        public bool MoveNext()
        {
            if (_queue.Count == 0) return false;
            _current = _queue.Dequeue();
            if (_current is LightElementNode elementNode)
                foreach (var child in elementNode.Children) _queue.Enqueue(child);
            return true;
        }
        public void Reset() => throw new NotSupportedException();
        public void Dispose() { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Шаблонний метод\n");
            
            var div = new LightElementNode("div", "block", "double");
            var h1 = new LightElementNode("h1", "block", "double");
            h1.AddChild(new LightTextNode("Hello!"));
            div.AddChild(h1);

            Console.WriteLine("\nВиклик рендеру (з хуками)");
            Console.WriteLine(div.OuterHTML);
        }
    }
}