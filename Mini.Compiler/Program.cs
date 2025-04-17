using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mini.Compiler.CodeAnalysis;
using Mini.Compiler.CodeAnalysis.Binding;
using Mini.Compiler.CodeAnalysis.Syntax;
using Mini.Compiler.CodeAnalysis.Text;

namespace Mini.Compiler
{
    internal class Program
    {
        private static readonly Dictionary<string, Action<Context>> _commands = new(StringComparer.OrdinalIgnoreCase)
        {
            ["clear"] = ctx => Console.Clear(),
            ["exit"] = ctx => ctx.ExitRequested = true,
            ["#showtree"] = ctx => ctx.ShowTree = true,
            ["#unshowtree"] = ctx => ctx.ShowTree = false,
            ["#help"] = ctx => DisplayHelp()
        };

        private static void Main(string[] args)
        {
            var context = new Context();
            DisplayWelcome();

            while (!context.ExitRequested)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                    continue;

                if (TryProcessCommand(input, context))
                    continue;

                EvaluateAndDisplay(input, context);
            }
        }

        private static bool TryProcessCommand(string input, Context context)
        {
            if (_commands.TryGetValue(input, out var action))
            {
                action(context);
                return true;
            }
            return false;
        }

        private static void EvaluateAndDisplay(string text, Context context)
        {
            var sourceText = SourceText.From(text);
            var parser = new Parser(sourceText);
            var syntaxTree = parser.Parse();
            var compilation = new Compliation(syntaxTree);

            if (context.ShowTree)
                PrettyPrint(Console.Out, syntaxTree.Root);

            var result = compilation.Evalutate(context.Variables);
            if (result.Diagnostics.Any())
            {
                foreach (var diag in result.Diagnostics)
                    PrintDiagnostic(diag, sourceText);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Value);
                Console.ResetColor();
            }
        }

        private static void PrintDiagnostic(Diagnostics diagnostic, SourceText source)
        {
            var pos = diagnostic.Span.Start;
            var index = source.GetLineIndex(pos);
            var line = source.Lines[index];
            var lineNum = index + 1;
            var column = pos - line.Start + 1;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"({lineNum},{column}): {diagnostic.Message}");
            Console.ResetColor();
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";
            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken tok && tok.Value is not null)
            {
                writer.Write(' ');
                writer.Write(tok.Value);
            }

            writer.WriteLine();
            indent += isLast ? "    " : "│   ";

            var children = node.GetChildren().ToList();
            for (int i = 0; i < children.Count; i++)
            {
                PrettyPrint(writer, children[i], indent, i == children.Count - 1);
            }
        }

        private static void DisplayWelcome()
        {
            Console.WriteLine("Mini.Compiler REPL - 输入表达式进行计算，或使用以下命令：");
            Console.WriteLine("  clear       清屏");
            Console.WriteLine("  exit        退出");
            Console.WriteLine("  #showTree   显示语法树");
            Console.WriteLine("  #unshowTree 取消语法树显示");
            Console.WriteLine("  #help       显示帮助");
            Console.WriteLine();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("可用命令:");
            foreach (var cmd in _commands.Keys.OrderBy(k => k))
                Console.WriteLine($"  {cmd}");
        }
    }

    internal class Context
    {
        public bool ShowTree { get; set; } = false;
        public bool ExitRequested { get; set; } = false;
        public Dictionary<string, object> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
