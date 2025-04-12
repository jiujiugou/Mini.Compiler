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
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);
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
                if (syntaxTree.Diagnostics.Any())
                {
                    foreach (var diagnostic in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                    continue;
                }

                try
                {
                    var evaluator = new Evaluator(boundExpression);
                    var result = evaluator.Evaluate();
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Runtime Error: {ex.Message}");
                }

                //var lexer = new Lexer(line);
                //while (true)
                //{
                //    var token = lexer.Lex();
                //    if (token.Kind == SyntaxKind.EndOfFileToken)
                //    {
                //        break;
                //    }
                //    Console.WriteLine($"{token.Kind}:'{token.Text}'");
                //    if (token.Value != null)
                //    {
                //        Console.WriteLine($"{token.Value}");
                //    }
                //}
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
