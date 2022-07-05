using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;

namespace Interactive
{
    public static class Highlighter
    {

        private static readonly List<(string, Color)> Patterns = new()
        {
            ("(\\\".*?\\\")", View.XamlConvert<Color>("#D69D85")), // string
            ("(\\.\\w+)", View.XamlConvert<Color>("#9CDCFE")), // member
            ("^(using |var )", View.XamlConvert<Color>("#569CD6")), // keyword
            ("(new |await )", View.XamlConvert<Color>("#D8A0DF")), // context
            ("(\\.\\w+\\()", View.XamlConvert<Color>("#DCDCAA")), // function
        };

        public static string ColorTable()
        {
            var table = @"{\colortbl;\red220\green220\blue220;";

            foreach (var (_, color) in Patterns)
                table += $"\\red{color.R}\\green{color.G}\\blue{color.B};";

            return table + "}";
        }

        public static string Highlight(string text, int pattern_index)
        {
            var num = pattern_index + 2; // black, default, target, ...
            var (pattern, _) = Patterns[pattern_index];
            return Regex.Replace(text, pattern, $"{{\\cf{num}$1\\cf1}}");
        }

        public static string Process(string source)
        {
            for (int index = 0; index < Patterns.Count; index++)
                source = Highlight(source, index);

            return @"{\rtf1\ansi " + ColorTable() + "\\cf1" + source + "}";
        }

    }
}
