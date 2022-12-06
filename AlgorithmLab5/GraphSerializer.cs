using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			return graph;
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
            StringBuilder[] sB = new StringBuilder[graph.Links.Count];
            for (var i = 0; i < graph.Links.Count; i++)
            {
                sB[i] = new();
                for (var target = 0; target < graph.Links.Count; target++)
                {
                    if (graph.Links[i].Target != target.ToString())
                    {
                        sB[i].Append("0;");
                        continue;
                    }
                    if (graph.Links[i].Target == target.ToString())
                    {
                        sB[i].Append(graph.Links[i].Weight + ";");
                    }
                    if (i+1 < graph.Links.Count && graph.Links[i+1].Source != null 
                                                && graph.Links[i].Source == graph.Links[i + 1].Source 
                                                && Convert.ToDouble(graph.Links[i+1].Target) - Convert.ToDouble(graph.Links[i].Target) > 1)
                    {
                        sB[i].Append("0;");
                    }
                    if (i+1 < graph.Links.Count && graph.Links[i+1].Source != null && graph.Links[i].Source == graph.Links[i+1].Source)
                    {
                        sB[i].Append(graph.Links[i+1].Weight + ";");
                        break;
                    }
                }
                //sB[i].Remove(sB[i].Length - 1, 1);
            }
            bool next = false;
            return sB.Aggregate("", (current, builder) => current + (builder.ToString() + '\n'));
        }
        // public static string Serialize(Graph graph)
		// {
  //           StringBuilder[] sB = new StringBuilder[3];
  //           for (int i = 0; i < 3; i++) sB[i] = new();
  //
  //           int linkCount = graph.Links.Count;
  //
  //           for (int i = 0; i < linkCount; i++)
		// 	{
  //               sB[0].Append(graph.Links[i].Source);
  //               sB[1].Append(graph.Links[i].Target);
  //               sB[2].Append(graph.Links[i].Weight);
  //
  //               if(i != linkCount - 1)
		// 		{
  //                   foreach(var e in sB)
		// 			{
  //                       e.Append(';');
		// 			}
		// 		}
		// 	}
  //
  //           string result =
  //               sB[0].ToString() + '\n' +
  //               sB[1].ToString() + '\n' +
  //               sB[2].ToString();
  //
  //           return result;
  //       }
    }
}
