using System;
using System.IO;
using System.Linq;

namespace AnalysisHelper
{
    /// <summary>
    /// Copy files from one folder to another, but only do it for files that contain in a "comparison folder"
    /// Useful for further side-by-side analysis of installer packages (that are usually a mess of hundreds of files),
    /// that contain different versions of software.
    /// For example, once the files of interest of both versions are collected inside two folders, NirSoft HashMyFiles
    /// can be used to quickly determine which files contain changes and characterize the changes
    /// </summary>
    public static class FileCopier
    {
        /// <summary>
        /// Where to put the result
        /// </summary>
        public static string Destination { get; set; }
        /// <summary>
        /// To get the names of files to be copied from (comparison is of binary type!)
        /// </summary>
        public static string ComparisonFolder { get; set; }
        /// <summary>
        /// Copy files from here
        /// </summary>
        public static string Source { get; set; }


        private static readonly StringComparer Comparer = StringComparer.Ordinal;

        public static void Main()
        {
            Console.WriteLine("Obtaining target names...");
            string[] names = Directory.EnumerateFiles(ComparisonFolder).Select(x => Path.GetFileName(x)).ToArray();
            Console.WriteLine("Searching for targets...");
            string[] toBeCopied = Directory.EnumerateFiles(Source)
                .Where(x => names.Contains(Path.GetFileName(x), Comparer)).ToArray();
            Console.WriteLine("Copying...");
            foreach (var item in toBeCopied)
            {
                string d = Path.Combine(Destination, Path.GetFileName(item));
                File.Copy(item, d, true);
            }
            Console.WriteLine("Not Found:");
            names = names.Except(toBeCopied.Select(x => Path.GetFileName(x))).ToArray();
            foreach (var item in names)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
