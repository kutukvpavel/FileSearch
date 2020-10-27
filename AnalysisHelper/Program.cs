using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AnalysisHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunCopier(); // TODO: CLI

        }

        static void RunCopier()
        {
            FileCopier.Destination = @"E:\Doc\Паша\Электроника\обрабатывающий центр\комп\cnc diamant software\cncwint (copied from cab)";
            FileCopier.ComparisonFolder = @"H:\cncwint";
            FileCopier.Source = @"E:\Doc\Паша\Электроника\обрабатывающий центр\комп\cnc diamant software\cnc soft cab";
            FileCopier.Main();
        }

        static void RunBinaryComparer()
        {

        }
    }
}
