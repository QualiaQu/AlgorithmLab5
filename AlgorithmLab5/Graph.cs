using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmLab5
{
	public class Graph
	{
		public Graph()
		{
			Nodes = new();
			Links = new();
		}

		public bool IsUndirected = false;
		public bool IsMst = false;
		public Dictionary<string, Node> Nodes { get; set; }
		public List<Link> Links { get; set; }
        
		public void AddNode(string nodeName)
		{
			if (!Nodes.ContainsKey(nodeName)) Nodes.Add(nodeName, new Node(nodeName));
		}

		public void AddLink<T>(T source, T target, int weight = 1)
		{
			string s = source.ToString();
			string t = target.ToString();

			AddNode(s);
			AddNode(t);
			Links.Add(new Link(s, t, weight));

			Nodes[s].AddNeighbour(t);
		}

		public void AddLink<T>(T link, int weight)
		{
			string[] arr = link.ToString()!.Split('-');
			AddLink(arr[0], arr[1], weight);
		}
        
		public void ChangeNode(string node, string newName)
		{
			Nodes[node].UpdateName(newName);
		}

		public void ChangeLink<T>(T source, T target, int weight)
		{
			string s = source.ToString();
			string t = target.ToString();

			for (int i = 0; i < Links.Count; i++)
			{
				if (s == Links[i].Source &&
					t == Links[i].Target) Links[i].Weight = weight;
			}
		}

		public void AddWeight<T>(T source, T target, int delta)
		{
			for(int i = 0; i < Links.Count; i++)
			{
				if (Links[i].Source == source.ToString() &&
					Links[i].Target == target.ToString() ) Links[i].Weight += delta;
			}
		}

		public void RemoveNode(string node)
		{
			Nodes.Remove(node);
			foreach(var n in Nodes.Values)
			{
				if (n.Neighbours.Contains(node)) n.Neighbours.Remove(node);
			}
			for(int i = 0; i < Links.Count;)
			{
				if (Links[i].Source == node || Links[i].Target == node)
				{
					Links.RemoveAt(i);
					i = 0;
					continue;
				}

				i++;
			}
		}

		public void RemoveLink(string link)
		{
			for(int i = 0; i < Links.Count; i++)
			{
				if (link == Links[i].ToString()) Links.RemoveAt(i);
			}
		}

		public Graph Clone()
		{
			return (Graph)MemberwiseClone();
		}
	}

	public class Node
	{
		public string Name { get; set; }
		public List<string> Neighbours { get; set; }

		public Node(string name)
		{
			Name = name;
			Neighbours = new();
		}

		public void UpdateName(string newName)
		{
			Name = newName;
		}

		public void AddNeighbour(string neighbour)
		{
			if(!Neighbours.Contains(neighbour))Neighbours.Add(neighbour);
		}
	}

	public class Link
	{
		public string Source { get; set; }
		public string Target { get; set; }
		public int Weight { get; set; }

		public Link()
		{

		}

		public Link(string source, string target, int weight)
		{
			Source = source;
			Target = target;
			Weight = weight;
		}

		public override string ToString()
		{
			return Source + "-" + Target;
		}
	}
}

