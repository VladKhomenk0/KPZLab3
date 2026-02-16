using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Task4
{
    public interface ITextReader
    {
        char[][] ReadFile(string filePath);
    }

    public class SmartTextReader : ITextReader
    {
        public char[][] ReadFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            char[][] result = new char[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].ToCharArray();
            }

            return result;
        }
    }

    public class SmartTextChecker : ITextReader
    {
        private SmartTextReader _reader;

        public SmartTextChecker(SmartTextReader reader)
        {
            _reader = reader;
        }

        public char[][] ReadFile(string filePath)
        {
            Console.WriteLine($"[Logger] Opening file: {filePath}");
            
            char[][] result = _reader.ReadFile(filePath);
            
            Console.WriteLine($"[Logger] Read successfully. Closing file.");
            Console.WriteLine($"[Logger] Stats: {result.Length} lines, {CountChars(result)} chars total.");
            
            return result;
        }

        private int CountChars(char[][] content)
        {
            int count = 0;
            foreach (var line in content) count += line.Length;
            return count;
        }
    }

    public class SmartTextReaderLocker : ITextReader
    {
        private ITextReader _reader;
        private Regex _restrictedPattern;

        public SmartTextReaderLocker(ITextReader reader, string pattern)
        {
            _reader = reader;
            _restrictedPattern = new Regex(pattern);
        }

        public char[][] ReadFile(string filePath)
        {
            if (_restrictedPattern.IsMatch(filePath))
            {
                Console.WriteLine("Access denied!");
                return new char[0][];
            }

            return _reader.ReadFile(filePath);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Проксі\n");

            File.WriteAllText("test.txt", "Hello World\nThis is a pattern test.");
            File.WriteAllText("admin_secret.txt", "TOP SECRET DATA");

            SmartTextReader realReader = new SmartTextReader();

            Console.WriteLine("Тест Логера");
            ITextReader loggerProxy = new SmartTextChecker(realReader);
            loggerProxy.ReadFile("test.txt");
            
            Console.WriteLine("\nТест Локера");
            ITextReader secureProxy = new SmartTextReaderLocker(realReader, @"^admin.*");

            Console.WriteLine("Спроба прочитати 'test.txt':");
            var res1 = secureProxy.ReadFile("test.txt");
            if (res1.Length > 0) Console.WriteLine("Успіх!");

            Console.WriteLine("\nСпроба прочитати 'admin_secret.txt':");
            secureProxy.ReadFile("admin_secret.txt");

            Console.ReadKey();
        }
    }
}