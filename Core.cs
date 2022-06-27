using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading.Tasks;

namespace Interactive
{
    public static class Core
    {
        private static ScriptState State { get; set; }
        private static ScriptOptions Options = ScriptOptions.Default;

        public static async void Initialize()
        {
            // create initial state
            State = await CSharpScript.RunAsync("", options: Options);

            // configure default assemblies
            AddReference(typeof(Core));
        }

        public static async void AddReference(Type target)
        {
            var reference = MetadataReference.CreateFromFile(target.Assembly.Location);
            Options = Options.AddReferences(reference);
            await State.ContinueWithAsync("", options: Options);
        }

        public static async Task<object> Parse(string line)
        {
            try
            {
                State = await State.ContinueWithAsync(line);
                return State.ReturnValue;
            }
            catch (Exception ex) { return ex.Message; }
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
