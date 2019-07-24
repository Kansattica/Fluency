// adapted from https://stackoverflow.com/a/12475608/5587653
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Fluency.Interpreter.Parser.Entities.FunctionGraph;

namespace Fluency.CLI
{
    public class GraphWriter
    {
        public struct Graph
        {
            public Node[] Nodes;
            public Link[] Links;
        }

        public struct Node
        {
            [XmlAttribute]
            public string Id;
            [XmlAttribute]
            public string Label;

            public Node(string id, string label)
            {
                this.Id = id;
                this.Label = label;
            }
        }

        public struct Link
        {
            [XmlAttribute]
            public string Source;
            [XmlAttribute]
            public string Target;
            [XmlAttribute]
            public string Label;

            public Link(string source, string target, string label)
            {
                this.Source = source;
                this.Target = target;
                this.Label = label;
            }
        }

        public List<Node> Nodes { get; protected set; }
        public List<Link> Links { get; protected set; }

        public GraphWriter()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
        }

        public void AddNode(Node n)
        {
            this.Nodes.Add(n);
        }

        public void AddLink(Link l)
        {
            this.Links.Add(l);
        }


        private HashSet<int> _seen = new HashSet<int>();
        public void WalkFunctionGraph(FunctionNode head)
        {
            if (!_seen.Contains(head.Id))
            {
                AddNode(head);
                _seen.Add(head.Id);
            }
            else
            {
                return;
            }

            if (head.TopOut != null)
            {
                WalkFunctionGraph(head.TopOut);
                AddLink(head, head.TopOut, "top");
            }

            if (head.BottomOut != null)
            {
                WalkFunctionGraph(head.BottomOut);
                AddLink(head, head.BottomOut, "bottom");
            }

            if (head.TopOut == null && head.BottomOut == null)
            {
                Node returnNode = new Node() { Label = "Return", Id = "return" + head.Id };
                AddNode(returnNode);
                AddLink(new Link { Source = head.Id.ToString(), Target = returnNode.Id, Label = "return" });
            }
        }

        private void AddNode(FunctionNode n)
        {
            AddNode(new Node { Id = n.Id.ToString(), Label = n.ToString() });
        }

        private void AddLink(FunctionNode a, FunctionNode b, string type)
        {
            AddLink(new Link { Source = a.Id.ToString(), Target = b.Id.ToString(), Label = type });
        }

        public void Serialize(string xmlpath)
        {
            Graph g = new Graph();
            g.Nodes = this.Nodes.ToArray();
            g.Links = this.Links.ToArray();

            XmlRootAttribute root = new XmlRootAttribute("DirectedGraph");
            root.Namespace = "http://schemas.microsoft.com/vs/2009/dgml";
            XmlSerializer serializer = new XmlSerializer(typeof(Graph), root);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(xmlpath, settings))
                serializer.Serialize(xmlWriter, g);
        }
    }
}