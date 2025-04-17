using Mini.Compiler.CodeAnalysis;
using Mini.Compiler.CodeAnalysis.Syntax;
using System.Reflection;
using Mini.Compiler.CodeAnalysis.Binding;
using Binder = Mini.Compiler.CodeAnalysis.Binding.Binder;
namespace Mini.Compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;
            var variables = new Dictionary<string, object>();
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == "Clear")
                {
                    Console.Clear();
                    continue;
                }
                if (line == "Exit")
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var parser = new Parser(line);
                var syntaxTree = parser.Parse();
                var compliation = new Compliation(syntaxTree);
                
                //var boundExpression=binder.BindExpression(syntaxTree.Root);
                //var boundExpression = binder.BindExpression(syntaxTree.Root);
                var result=compliation.Evalutate(variables);
                var diagnostics = result.Diagnostics;
                if (line == "#showTree")
                {
                    showTree = true;
                    continue;
                }
                if (line == "#unshowTree")
                {
                    showTree = false;
                    continue;
                }
                if (showTree)
                {
                    PrettyPrint(syntaxTree.Root);
                }
                if (diagnostics.Any())
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(diagnostic.Message); // ✅ 打印错误信息
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.Value); // 打印结果
                    Console.ResetColor();
                }
            }
        }
        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);
            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }
            Console.WriteLine();
            indent += isLast ? "    " : "│   ";
            var lastChild = node.GetChildren().LastOrDefault();
            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }
}
