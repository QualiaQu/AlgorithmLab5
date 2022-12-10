using System;
using System.IO;

namespace AlgorithmLab5
{
	public delegate void WriteLog(string text);

	static class Program
	{
		

		static void Main()
		{
			WriteLog writeLog = WriteInConsole;
			//TestDFT(writeLog);
			//TestBFT(writeLog);
			TestMaxFlow2(writeLog);
		}

		private static void WriteInConsole(string text)
		{
			Console.Write(text);
		}

		private static void TestMaxFlow2(WriteLog writeLog)
		{
			string[] graphData = File.ReadAllLines("input.csv");
			var graph = GraphSerializer.Deserialize(graphData, GraphSerializer.GraphStorageType.MyType);

			GraphAlgorithms algorithms = new(graph, writeLog);

			Console.WriteLine("Максимальный поток равен "
							  + algorithms.FordFulkerson(graph, "0", "4"));
		}
	}
}
