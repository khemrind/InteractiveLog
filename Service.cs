using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Interactive
{
    public static class Service
    {
        private static ScriptState State { get; set; }
        private static ScriptOptions Options = ScriptOptions.Default;

        public readonly static List<string> Scripts = new();
        public readonly static List<string> History = new();

        public static async void Initialize()
        {
            // create initial state
            State = await CSharpScript.RunAsync("int _ = 0;", options: Options);

            // configure default assemblies
            AddReference(typeof(Service));
        }

        public static async void AddReference(Type target)
        {
            var reference = MetadataReference.CreateFromFile(target.Assembly.Location);
            Options = Options.AddReferences(reference);
            State = await State.ContinueWithAsync($"using {target.Namespace};", options: Options);
        }

        public static async Task<object> Parse(string line)
        {
            var time = Stopwatch.StartNew();
            // attempt to parse and execute
            try
            {
                State = await State.ContinueWithAsync(line);
                return State.ReturnValue;
            }

            catch (Exception ex) 
            {
                return ex.Message; 
            }

            finally
            {
                Debug.WriteLine(time.ElapsedMilliseconds + "ms");
            }
        }

        public static string GetHistory(int index)
        {
            if (index >= 0) return History[History.Count - 1 - index];
            else return string.Empty;
        }

        //public static void GetVariables()
        //{
        //    foreach (var item in State.Variables)
        //        Output($"{item.Name}: {item.Value} ({item.Type})");
        //}

        //public static void GetReferences()
        //{
        //    foreach (var item in Options.MetadataReferences)
        //    {
        //        string display = item.Display ?? "";
        //        Output(display);
        //    }
        //}
    }
}
