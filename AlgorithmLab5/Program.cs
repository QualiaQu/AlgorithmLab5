using System;

namespace AlgorithmLab5
{
	public delegate void WriteLog(string text);

	static class Program
	{
		

		static void Main(string[] args)
		{
			WriteLog writeLog = WriteInConsole;
			//TestDFT(writeLog);
			//TestBFT(writeLog);
			TestMaxFlow2(writeLog);
		}

		public static void WriteInConsole(string text)
		{
			Console.Write(text);
		}

		public static void TestDFT(WriteLog writeLog)
		{
			Graph graph = new();
			GraphAlgorithms algorithms = new(graph, writeLog);

			graph.AddLink(1, 2);
			graph.AddLink(1, 7);
			graph.AddLink(1, 8);
			graph.AddLink(2, 3);
			graph.AddLink(2, 6);
			graph.AddLink(3, 4);
			graph.AddLink(3, 5);
			graph.AddLink(8, 9);
			graph.AddLink(8, 12);
			graph.AddLink(9, 10);
			graph.AddLink(9, 11);

			algorithms.DFT("1");
		}

		public static void TestBFT(WriteLog writeLog)
		{
			Graph graph = new();
			GraphAlgorithms algorithms = new(graph, writeLog);

			graph.AddLink(1, 2);
			graph.AddLink(1, 3);
			graph.AddLink(1, 4);
			graph.AddLink(2, 5);
			graph.AddLink(3, 6);
			graph.AddLink(3, 7);
			graph.AddLink(4, 8);
			graph.AddLink(5, 9);
			graph.AddLink(6, 10);

			algorithms.BFT("1");
		}

		public static void TestMaxFlow(WriteLog writeLog)
		{
			string[] graphData = FileWorker.ReadFile("AdjacencyMaxFlow.csv");
			Graph graph = new();
			graph = GraphSerializer.Deserialize(graphData, GraphSerializer.GraphStorageType.AdjacencyMatrix);
			GraphAlgorithms algorithms = new(graph, writeLog);

			Console.WriteLine("Максимальный поток равен "
							  + algorithms.FordFulkerson(graph, "0", "7"));
		}

		private static void TestMaxFlow2(WriteLog writeLog)
		{
			
			string[] graphData = FileWorker.ReadFile("input.csv");
			var graph = GraphSerializer.Deserialize(graphData, GraphSerializer.GraphStorageType.MyType);

			GraphAlgorithms algorithms = new(graph, writeLog);

			Console.WriteLine("Максимальный поток равен "
							  + algorithms.FordFulkerson(graph, "0", "4"));
		}
	}
}
