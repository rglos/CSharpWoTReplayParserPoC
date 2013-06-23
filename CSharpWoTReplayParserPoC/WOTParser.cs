using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CSharpWoTReplayParserPoC
{
    public class BytesArrayBinaryReader : BinaryReader
    {
        public BytesArrayBinaryReader(byte[] input)
            : base(new MemoryStream(input))
        {
        }
    }

    class WOTParser
    {
        private ScriptEngine pyEngine = null;
        private ScriptRuntime pyRuntime = null;
        private ScriptScope pyScope = null;

        private string extractDetailsBlob(string data)
        {
            Func<string, string> unpickle = pyScope.GetVariable<Func<string, string>>("load_details_blob");
            return unpickle(data);
        }

        private void initPython()
        {
            if (pyEngine == null)
            {
                pyEngine = Python.CreateEngine();
                pyScope = pyEngine.CreateScope();

                ScriptSource src = pyEngine.CreateScriptSourceFromFile("scripts.py");

                src.Execute(pyScope);
            }
        }

        private dynamic runPythonCode(String code)
        {
            ScriptSource source = pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();
            return compiled.Execute(pyScope);
        }

        public WOTParser()
        {
            initPython();
        }

        
        public bool processReplayFile(string file)
        {
            byte[] ret = null;
            try
            {
                FileStream fs = File.OpenRead(file);
                BinaryReader br = new BinaryReader(fs);

                fs.Seek(4, SeekOrigin.Begin); // Skip magic number
                int blocks = br.ReadInt32();

                if (blocks == 3)
                {
                    Console.WriteLine("[+] Loading chunks, {0} blocks found", blocks);

                    blocks = br.ReadInt32();
                    Console.WriteLine("[+] Players chunk length {0}", blocks);
                    byte[] data = br.ReadBytes(blocks);
                    
                    // XXX: Player data
                    // - get the bytes into a string
                    var dataString = Encoding.UTF8.GetString(data, 0, data.Length);
                    // - parse the string into a JSON object
                    var jobject = JObject.Parse(dataString);
                    // - TODO: Now what?  We can use the jobject to populate a Entity Framework object(s) and persist that to the database
                    //      - so we keep seperation of concerns here... we keep the parsing seperate from the entities in case there is drift in either objects
                    //      - i think this will work well
                    //      - have a look at some of the more recent answers to this question - we might be able to package this nicely - http://stackoverflow.com/questions/2246694/how-to-convert-json-object-to-custom-c-sharp-object
                    // - TODO: Test this on a partial replay - i.e. I leave the battle 1/2 through it to play another tank.  What's in the replay file?
                    //      - with the battle results file, we get the full battle results if we view it afterwards - what about the replay?
                    Console.WriteLine(dataString);

                    blocks = br.ReadInt32();
                    Console.WriteLine("[+] Frags chunk length {0}", blocks);
                    data = br.ReadBytes(blocks);

                    // XXX: Frag data
                    Console.WriteLine(Encoding.UTF8.GetString(data, 0, data.Length));

                    blocks = br.ReadInt32();
                    Console.WriteLine("[+] Details pickle length {0}", blocks);
                    data = br.ReadBytes(blocks);

                    // XXX: Details data
                    Console.WriteLine(extractDetailsBlob(System.Text.Encoding.Default.GetString(data)));

                }
                else
                {
                    Console.WriteLine("[-] Insufficient block count");
                }

                br.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
