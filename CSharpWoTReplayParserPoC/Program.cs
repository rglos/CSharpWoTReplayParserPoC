using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSharpWoTReplayParserPoC
{
    class Program
    {
        static void Main(string[] args)
        {
            WOTParser wp = new WOTParser();

            if (args.Length > 0)
            {
                wp.processReplayFile(args[0]);
            }
        }
    }
}
