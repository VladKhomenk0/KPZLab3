using System;
using System.IO;

namespace Task1
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string message);
        void Warn(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG]: {message}");
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR]: {message}");
            Console.ResetColor();
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[WARN]: {message}");
            Console.ResetColor();
        }
    }

    public class FileWriter
    {
        public void Write(string text)
        {
            File.AppendAllText("log.txt", text);
        }

        public void WriteLine(string text)
        {
            File.AppendAllText("log.txt", text + Environment.NewLine);
        }
    }
    
    public class FileLoggerAdapter : ILogger
    {
        private readonly FileWriter _fileWriter;

        public FileLoggerAdapter(FileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public void Log(string message)
        {
            _fileWriter.WriteLine($"[LOG] {DateTime.Now}: {message}");
            Console.WriteLine($"Записано у файл: [LOG] {message}"); 
        }

        public void Error(string message)
        {
            _fileWriter.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            Console.WriteLine($"Записано у файл: [ERROR] {message}");
        }

        public void Warn(string message)
        {
            _fileWriter.WriteLine($"[WARN] {DateTime.Now}: {message}");
            Console.WriteLine($"Записано у файл: [WARN] {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Адаптер\n");
            
            ILogger consoleLogger = new Logger();
            consoleLogger.Log("Система запущена.");
            consoleLogger.Warn("Попередження.");

            Console.WriteLine("\nПеремикаємось на файловий логер\n");
            
            FileWriter fileWriter = new FileWriter();
            ILogger fileLogger = new FileLoggerAdapter(fileWriter);

            fileLogger.Log("Система запущена (файл).");
            fileLogger.Warn("Увага, запис у файл.");
            fileLogger.Error("Критична помилка у файлі.");

            Console.ReadKey();
        }
    }
}