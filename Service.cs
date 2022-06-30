﻿using Microsoft.CodeAnalysis;
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

        public readonly static List<string> History = new();

        public static async void Initialize()
        {
            // create initial state
            State = await CSharpScript.RunAsync("int moo = 3;", options: Options);

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
            // save line
            if (History.Count != 0)
            {
                var last = History[^1];
                if (line != last) History.Add(line);
            }
            else History.Add(line);

            // attempt to parse and execute
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
