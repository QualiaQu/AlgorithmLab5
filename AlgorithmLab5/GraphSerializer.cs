using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorithmLab5
{
	public static class GraphSerializer
	{
        public enum GraphStorageType
		{
            MyType,
            AdjacencyMatrix
        }

        public static Graph Deserialize(string[] graphData, GraphStorageType type)
        {
            Graph graph = new();

            List<string[]> list = new();
            foreach (string s in graphData)
            {
                list.Add(s.Split(';'));
            }

            string[][] gA = list.ToArray();

            if (type == GraphStorageType.MyType)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < gA[0].Length; j++)
                    {
                        if (!graph.Nodes.ContainsKey(gA[i][j]))
                        {
                            graph.AddNode(gA[i][j]);
                        }
                        if (i == 1)
                        {
                            graph.AddLink(gA[0][j], gA[1][j], Convert.ToInt32(gA[2][j]));
                        }
                    }
                }
            }
            else if (type == GraphStorageType.AdjacencyMatrix)
            {
                int[,] tempArr = new int[gA[0].Length, gA[0].Length];
                for (int i = 0; i < gA[0].Length; i++)
                {
                    for (int j = 0; j < gA[0].Length; j++)
                    {
                        tempArr[i, j] = Convert.ToInt32(gA[i][j]);
                    }
                }

                graph = ToGraph(tempArr);
			}
			else
			{
				throw new NotImplementedException();
			}

            graph.IsUndirected = IsUndirected(graph);
			return graph;
        }

        private static bool IsUndirected(Graph graph)
        {
            var count = graph.Links
                .Select(link => link.Target + "-" + link.Source)
                .Count(tempLink => graph.Links
                .Any(current => tempLink == (current.Source + "-" + current.Target)));

            return count == graph.Links.Count;
        }
        private static Graph ToGraph(int[,] arr)
		{
            Graph graph = new();

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                graph.AddNode(i.ToString());
                for (int j = 0; j < arr.GetLength(0); j++)
                {
                    if (i < j && arr[i, j] > 0)
					{
                        graph.AddLink(i, j, arr[i, j]);
					}
                    if (j < i && arr[i, j] > 0)
					{
                        graph.AddLink(i, j, arr[i, j]);
					}
                }
            }

            return graph;
        }

        public static string Serialize(Graph graph)
        {
            StringBuilder[] sB = new StringBuilder[graph.Nodes.Count];

            for (int i = 0; i < graph.Nodes.Count; i++)
            {
                sB[i] = new StringBuilder();
                var node = graph.Nodes[i.ToString()];
                for(int j = 0; j < graph.Nodes.Count; j++)
                {
                    bool next = false;
                    foreach (var neighbour in node.Neighbours)
                    {
                        if (j.ToString() == neighbour)
                        {
                            foreach (var link in graph.Links.Where(link => link.Source == i.ToString() && link.Target == j.ToString()))
                            {
                                sB[i].Append(link.Weight + ";");
                                next = true;
                                break;
                            }
                        }
                        if (next) break;
                    }
                    if(!next) sB[i].Append("0;");
                }
                sB[i].Remove(sB[i].Length - 1, 1);
                sB[i].Append('\n');
            }
            
            return sB.Aggregate<StringBuilder, string>(null, (current, builder) => current + builder);
        }
    }
}
