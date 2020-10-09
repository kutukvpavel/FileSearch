using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileSearch
{
    static class Program
    {
        static Program()
        {
            Encodings = new List<Encoding>()
            {
                { Encoding.ASCII },
                { Encoding.Unicode },
                { Encoding.UTF8 },
                { Encoding.BigEndianUnicode },
                { Encoding.UTF32 },
                { Encoding.UTF7 }
            };
        }

        static readonly List<Encoding> Encodings;

        static void Main(string[] args)
        {
            if (args == null) return;
            if (args.Length < 2) return;
            if (args[0].Length == 0 || args[1].Length == 0) return;
            Console.OutputEncoding = Encoding.UTF8;
            args[1] = args[1].Trim('"');
            bool fileExists = File.Exists(args[1]);
            if (!fileExists && !Directory.Exists(args[1])) return;
            string[] files;
            if (fileExists)
            {
                files = new string[] { args[1] };
            }
            else
            {
                files = Directory.GetFiles(args[1], args.Length > 2 ? args[2] : "*", SearchOption.AllDirectories);
            }
            SearchTargetEqualityComparer comparer = new SearchTargetEqualityComparer(args[0]);
            SearchTarget[] targets = Encodings.Select(x => new SearchTarget(args[0], x)).Distinct(comparer).ToArray();
            Console.WriteLine("Distinct encodings:");
            foreach (var item in targets)
            {
                try
                {
                    item.CreateAlgorithm();
                    Console.WriteLine(item.ToString());
                }
                catch (Exception e)
                {
                    ErrorListener.Instance.Add(e);
                }
            }
            SearchFile[] results = files.Select(x => new SearchFile(x)).ToArray(); 
            Console.WriteLine(Environment.NewLine + "Searching files...");
            foreach (var item in results)
            {
                try
                {
                    item.Search(targets);
                    if (item.Results.Length > 0) Console.WriteLine(item.ToString());
                }
                catch (Exception e)
                {
                    ErrorListener.Instance.Add(e);
                }
            }
            Console.WriteLine("Finished!");
            if (ErrorListener.Instance.Any()) Console.WriteLine(Environment.NewLine + "Errors:");
            foreach (var item in ErrorListener.Instance)
            {
                Console.WriteLine(item.ToString());
            }
            Console.ReadKey();
        }
    }

    public static class ErrorListener
    {
        private static readonly List<Exception> _instance = new List<Exception>();

        public static List<Exception> Instance { get { return _instance; } }
    }
}
