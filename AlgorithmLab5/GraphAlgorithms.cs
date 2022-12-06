using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmLab5
{
    public class GraphAlgorithms
    {
        private Graph graph;
		private WriteLog WriteLog;
        public GraphAlgorithms()
        {

        }

        public GraphAlgorithms(Graph graph, WriteLog writeLog)
        {
            this.graph = graph;
			WriteLog = writeLog;
        }

        //Рекурсивный метод
        private void DFTRecursive(string node, Dictionary<string, bool> visited)
        {
			//Отмечаем полученный узел
			WriteLog($"Отмечаем узел {node}\n");
			visited[node] = true;

            //Выполняем подобное для каждого соседнего узла
            List<string> neighbours = graph.Nodes[node].Neighbours;
			WriteLog($"Соседи узла {node}:\n");
			foreach (var e in neighbours)
			{
				WriteLog($" {e}");
			}
			WriteLog("\n------\n");
            foreach (var n in neighbours)
            {
                if (!visited[n]) DFTRecursive(n, visited);
            }
        }

		//Обход в глубину
		public void DFT(string node)
        {
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = new();
            foreach (var e in graph.Nodes)
            {
                visitedNodes.Add(e.Key, false);
            }

			//Вызов рекурсивного метода
			WriteLog($"Обход в глубину\n" +
				$"Начинаем с узла {node}\n");
            DFTRecursive(node, visitedNodes);
        }

        //Обход в ширину
        public void BFT(string node)
        {
			if (!graph.Nodes.ContainsKey(node)) throw new Exception("Error");
            //Список посещенных узлов
            Dictionary<string, bool> visitedNodes = new();
            foreach (var e in graph.Nodes)
            {
                visitedNodes.Add(e.Key, false);
            }
			
            //Очередь узлов для посещения
            LinkedList<string> queue = new();

			WriteLog($"Обход в ширину\n" +
				$"Начинаем с узла {node}\n");
			//Отмечаем первый узел и добавляем в очередь
			WriteLog($"Отмечаем узел {node}\n");
			visitedNodes[node] = true;
			WriteLog($"Добавляем узел {node} в очередь\n");
			queue.AddLast(node);
			WriteLog("Очередь:");
			foreach (var e in queue)
			{
				WriteLog($" {e}");
			}
			WriteLog("\n-----\n");

			while (queue.Any())
            {
				//Убираем первый узел в очереди
				node = queue.First();
				WriteLog($"Берём первый узел из очереди: {node}\n");
				queue.RemoveFirst();

                //Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
                List<string> neighbours = graph.Nodes[node].Neighbours;
				WriteLog($"Соседи узла {node}:");
				foreach (var e in neighbours)
				{
					WriteLog($" {e}");
				}
				WriteLog("\n");

				foreach (var val in neighbours)
                {
                    if (!visitedNodes[val])
                    {
                        visitedNodes[val] = true;
                        queue.AddLast(val);
                    }
                }

				WriteLog("Очередь:");
				foreach (var e in queue)
				{
					WriteLog($" {e}");
				}
				WriteLog("\n------\n");
			}
        }



		//Максимальный поток
		public int FordFulkerson(Graph graph, string s, string t)
		{
			string u, v;

			//Граф остаточного потока
			Dictionary<string, int> rGraph = new();
			foreach(var l in graph.Links)
			{
				rGraph.Add(l.ToString(), l.Weight);
			}

			//Путь к стоку, заполняемый методом обхода в ширину
			Dictionary<string, string> path = new();

			//Обнуляем максимальный поток
			int maxFlow = 0;

			WriteLog("Начинаем поиск максимального потока\n");
			WriteLog($"Из узла {s} в {t}\n");

			//Пока путь есть
			while (BFS(rGraph, s, t, path))
			{
				WriteLog($"Текущий максимальный поток: {maxFlow}\n");
				WriteLog("Текущий путь:");

				//Ищем минимальный поток у данного пути
				int pathFlow = int.MaxValue;
				for (v = t; v != s; v = path[v])
				{
					u = path[v];
					pathFlow = Math.Min(pathFlow, rGraph[ToLink(u, v)]);
					WriteLog(" " + ToLink(u, v) + "[" + rGraph[ToLink(u, v)] + "]");
				}
				WriteLog($"\nМинимальный поток у этого пути: {pathFlow}\n");

				//Уменьшаем пропускную способность
				for (v = t; v != s; v = path[v])
				{
					u = path[v];
					rGraph[ToLink(u, v)] -= pathFlow;
				}

				//Увеличиваем максимальный поток на поток отдельного пути
				maxFlow += pathFlow;

				WriteLog("-----\n");
			}

			WriteLog("Максимальный поток равен "
							  + maxFlow + "\n");

			return maxFlow;
		}

		//Поиск обходом в ширину
		//Проверяет есть ли путь. Если да, то заполняет его
		private bool BFS(Dictionary<string, int> rGraph, string s, string t, Dictionary<string, string> path)
		{
			Dictionary<string, bool> visitedNodes = new();
			foreach (var e in graph.Nodes)
			{
				visitedNodes.Add(e.Key, false);
			}

			List<string> queue = new();
			queue.Add(s);
			visitedNodes[s] = true;
			path[s] = null;

			while (queue.Count != 0)
			{
				string u = queue[0];
				queue.RemoveAt(0);

				List<string> list = graph.Nodes[u].Neighbours;

				foreach (var v in list)
				{
					if (visitedNodes[v] == false
						&& rGraph[ToLink(u, v)] > 0)
					{
						if (v == t)
						{
							path[v] = u;
							return true;
						}
						queue.Add(v);
						path[v] = u;
						visitedNodes[v] = true;
					}
				}
			}

			return false;
		}

		private static string ToLink(string s, string t)
		{
			return s.ToString() + "-" + t.ToString();
		}
	}
}
