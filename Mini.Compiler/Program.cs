using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mini.Compiler.CodeAnalysis;
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

        static void Main(string[] args)
        {
            var context = new Context();
            Compliation previous = null;

            DisplayWelcome();

            while (!context.ExitRequested)
            {
                // 1) 读取用户一段“完整”的脚本（可能是多行块）
                var script = ReadCompleteScript();

                // 2) 如果是命令行，先处理命令
                var trimmed = script.Trim();
                if (_commands.TryGetValue(trimmed, out var cmd))
                {
                    cmd(context);
                    continue;
                }

                // 3) 解析 → 绑定 → 执行
                var syntaxTree = SyntaxTree.Parse(script);
                var compilation = previous == null
                    ? new Compliation(syntaxTree)
                    : previous.CompliationWith(syntaxTree);

                if (context.ShowTree)
                    PrettyPrint(Console.Out, syntaxTree.Root);

                var result = compilation.Evalutate(context.Variables);

                // 4) 输出诊断或结果
                if (result.Diagnostics.Any())
                {
                    foreach (var d in result.Diagnostics)
                        PrintDiagnostic(d, syntaxTree.Text);
                    // 即使报错，也保留上一状态，方便用户修正
                    previous = compilation;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                    previous = compilation;
                }
            }
        }

        /// <summary>
        /// 从控制台读取一段“完整”的脚本：要么单行，要么多行 {} 块
        /// </summary>
        private static string ReadCompleteScript()
        {
            var sb = new StringBuilder();

            while (true)
            {
                // 首行用 "> "，后续行用 "* "
                //Console.Write(sb.Length == 0 ? "> " : " ");
                var line = Console.ReadLine();
                if (line == null)
                    break;

                sb.AppendLine(line);

                // 用 Parser 试着解析，检查是否还缺一个 "}"
                var text = sb.ToString();
                var parser = new Parser(SourceText.From(text));
                _ = parser.CompilationUnit(); // 触发解析并填充 Diagnostics

                var needsMore = parser.Diagnostics.Any(d =>
                    d.Message.Contains("expected CloseBraceToken", StringComparison.OrdinalIgnoreCase) &&
                    d.Span.Start >= text.Length - 1);

                if (!needsMore)
                    break;
            }

            return sb.ToString();
        }

        private static void PrintDiagnostic(Diagnostics diagnostic, SourceText source)
        {
            var pos = diagnostic.Span.Start;
            var lineIdx = source.GetLineIndex(pos);
            var line = source.Lines[lineIdx];
            var lineNum = lineIdx + 1;
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
                PrettyPrint(writer, children[i], indent, i == children.Count - 1);
        }

        private static void DisplayWelcome()
        {
            Console.WriteLine("Mini.Compiler REPL - 输入表达式进行计算，或使用以下命令：");
            Console.WriteLine("  clear        清屏");
            Console.WriteLine("  exit         退出");
            Console.WriteLine("  #showTree    显示语法树");
            Console.WriteLine("  #unshowTree  取消语法树显示");
            Console.WriteLine("  #help        显示帮助");
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
        public bool ShowTree { get; set; }
        public bool ExitRequested { get; set; }
        public Dictionary<string, object> Variables { get; } =
            new(StringComparer.OrdinalIgnoreCase);
    }
}
