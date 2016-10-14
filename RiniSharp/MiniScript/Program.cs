using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using Newtonsoft.Json;

namespace MiniScript
{
    class Node
    {
        [JsonIgnore]
        public virtual int maxChildren { get; }

        [JsonIgnore]
        public Node parent;
        public List<Node> children;

        public Node()
        {
            children = new List<Node>();
        }
        public bool Push(Node node)
        {
            Console.WriteLine($"PUSH {this} , {node}");
            children.Add(node);
            return children.Count == maxChildren;
        }

        public virtual object Exec()
        {
            return null;
        }
    }
    class BlockNode : Node
    {
    }

    class BinaryExpression : Node
    {
        public override int maxChildren => 2;

        public Node left
        {
            get
            {
                return children[0];
            }
        }
        public Node right
        {
            get
            {
                return children[1];
            }
        }

        public string op;

        public override object Exec()
        {
            if (op == "+")
                return Op.Add(left.Exec(), right.Exec());

            return null;
        }
    }
    class Ident : Node
    {
        public string id;

        public override object Exec()
        {
            return 5;
        }
    }
    class Literal : Node
    {
        public object value;

        public override object Exec()
        {
            return value;
        }
    }

    class Op
    {
        public static object Add(object a, object b)
        {
            Console.WriteLine($"OpADD {a} + {b}");
            if (a is int)
                return (int)a + Convert.ToInt32(b);

            return null;
        }
    }

    class Program
    {

        static void Dive(SyntaxNode node, Node parent)
        {
            Console.WriteLine(node + " / " + node.GetType().ToString());

            Node current = null;

            if (node is LiteralExpressionSyntax)
            {
                var literal = new Literal() { parent = parent };
                literal.value = ((LiteralExpressionSyntax)node).Token.Value;

                current = literal;
            }
            if (node is IdentifierNameSyntax)
            {
                var ident = new Ident() { parent = parent };
                ident.id = (string)((IdentifierNameSyntax)node).Identifier.Value;

                current = ident;
            }
            if (node is BinaryExpressionSyntax)
            {
                var bin = new BinaryExpression() { parent = parent };
                bin.op = ((BinaryExpressionSyntax)node).OperatorToken.ValueText;
                
                current = bin;
            }

            if (parent is BinaryExpression)
            {
                
            }

            if (current != null && parent != null)
            {
                if ((parent).Push(current))
                {
                    Console.WriteLine("Up");
                    current = parent.parent;
                    parent = current;
                }
                else
                {
                    //current.parent = parent;
                    parent = current;
                }
            }

            foreach (var child in node.ChildNodes())
            {
                Dive(child, parent);
            }
        }

        static void Execute(Node node)
        {

        }

        static void Main(string[] args)
        {

            var src = @"
11 + a + 33;
";

            var options =
                new CSharpParseOptions(
                    LanguageVersion.CSharp6, DocumentationMode.Diagnose, SourceCodeKind.Script);
            var references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            };
            var syntaxTree = CSharpSyntaxTree.ParseText(src, options);


            Console.WriteLine(syntaxTree.GetRoot().GetType());

            var root = new BlockNode();
            Dive(syntaxTree.GetRoot(), root);



            Console.WriteLine(root.children[0].Exec());

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.Formatting = Formatting.Indented;
            setting.TypeNameHandling = TypeNameHandling.Objects;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(root, setting);

            Console.WriteLine(json);
        }
    }
}
