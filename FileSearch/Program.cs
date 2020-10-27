using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;

namespace FileSearch
{
    public enum ExitCodes
    {
        EncoderError = -3,
        IOError = -2,
        BadArguments = -1,
        OK = 0
    }

    public static class Program
    {
        static List<Encoding> Encodings;

        static int Main(string[] args)
        {
            //Parse CLI
            if (args == null) return (int)ExitCodes.BadArguments;
            if (args.Length < 2) return (int)ExitCodes.BadArguments;
            if (args[0].Length == 0 || args[1].Length == 0) return (int)ExitCodes.BadArguments;
            Console.OutputEncoding = Encoding.UTF8;
            args[1] = args[1].Trim('"');

            //Get directory listings
            bool fileExists = File.Exists(args[1]);
            if (!fileExists && !Directory.Exists(args[1])) return (int)ExitCodes.IOError;
            string[] files;
            if (fileExists)
            {
                files = new string[] { args[1] };
            }
            else
            {
                try
                {
                    Console.WriteLine("Obtaining directory listings...");
                    files = Directory.GetFiles(args[1], args.Length > 2 ? args[2] : "*", SearchOption.AllDirectories);
                }
                catch (IOException)
                {
                    Console.WriteLine("IO error!");
                    return (int)ExitCodes.IOError;
                }
            }

            //Create search targets
            SearchTargetEqualityComparer comparer;
            SearchTarget[] targets;
            if (args.Contains("-b")) //Raw byte search
            {
                Encodings = new List<Encoding>()
                {
                    RawEncoding.Instance,
                    RawReversedEncoding.Instance
                };
                args[0] = args[0].Trim('"');
            }
            else
            {
                Encodings = new List<Encoding>()
                {
                    Encoding.ASCII,
                    Encoding.Unicode,
                    Encoding.UTF8,
                    Encoding.BigEndianUnicode,
                    Encoding.UTF32,
                    Encoding.UTF7
                };
            }
            try
            {
                comparer = new SearchTargetEqualityComparer(args[0]);
                targets = Encodings.Select(x => new SearchTarget(args[0], x)).Distinct(comparer).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encoder error:");
                Console.WriteLine(ex.ToString());
                return (int)ExitCodes.EncoderError;
            }

            //Preparations over,
            //following sections do not terminate on errors

            //Search
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

            //Output
            if (ErrorListener.Instance.Any()) Console.WriteLine(Environment.NewLine + "Errors:");
            foreach (var item in ErrorListener.Instance)
            {
                Console.WriteLine(item.ToString());
            }
            if (args.Contains("-k")) Console.ReadKey();
            return (int)ExitCodes.OK;
        }
    }

    public static class ErrorListener
    {
        private static readonly List<Exception> _instance = new List<Exception>();

        public static List<Exception> Instance { get { return _instance; } }
    }
}
