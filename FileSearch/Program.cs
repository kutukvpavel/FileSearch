using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //{ Encoding.UTF8 },
                { Encoding.BigEndianUnicode }
            };
            NotFound = Enumerable.Repeat(-1, Encodings.Count).ToArray();
            Errors = new List<Exception>();
        }

        static readonly List<Encoding> Encodings;
        static readonly int[] NotFound;
        static List<Exception> Errors;

        static void Main(string[] args)
        {
            if (args == null) return;
            if (args.Length < 2) return;
            if (args[0].Length == 0 || args[1].Length == 0) return;
            Console.OutputEncoding = Encoding.Unicode;
            bool fileExists = File.Exists(args[1]);
            if (!fileExists && !Directory.Exists(args[1])) return;
            byte[][] targets = Encodings.Select(x => x.GetBytes(args[0])).ToArray();
            KnuthMorrisPratt[] algos = targets.Select(x => new KnuthMorrisPratt(x)).ToArray();
            string[] files;
            if (fileExists)
            {
                files = new string[] { args[1] };
            }
            else
            {
                files = Directory.GetFiles(args[1], "*", SearchOption.AllDirectories);
            }
            int[][] results = new int[files.Length][];
            Console.WriteLine("Searching files...");
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    results[i] = ProcessFile(files[i], /*targets,*/ algos);
                }
                catch (Exception ex)
                {
                    results[i] = NotFound;
                    Errors.Add(ex);
                }
            }
            Console.WriteLine("Finished.");
            var output = results.Select((x, i) =>
                x.Any(z => z > -1)
                ? string.Join(" === ", new string[] { files[i] }.Concat( 
                    x.Select((y, j) => (y > -1) ? Encodings[j].EncodingName : null).Where(y => y != null)))
                : null)
                .Where(x => x != null);
            foreach (var item in output)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(Environment.NewLine + "Errors:");
            foreach (var item in Errors)
            {
                Console.WriteLine(item.ToString());
            }
            Console.ReadKey();
        }

        static int[] ProcessFile(string path, /*byte[][] targets,*/ KnuthMorrisPratt[] algos)
        {
            var file = File.ReadAllBytes(path);
            int[] res = new int[algos.Length];
            for (int i = 0; i < algos.Length; i++)
            {
                res[i] = algos[i].Search(file);
                /*int len = file.Length - targets[i].Length; //Simple shift->compare algorithm (for testing)
                if (len < 0)
                {
                    res[i] = -1;
                    continue;
                }
                int j = 0;
                for (; j <= len; j++)
                {
                    int k = 0;
                    for (; k < targets[i].Length; k++)
                    {
                        if (targets[i][k] != file[j + k]) break;
                    }
                    if (k == targets[i].Length) break;
                }
                res[i] = j > len ? -1 : j;*/
            }
            return res;
        }
    }
}
