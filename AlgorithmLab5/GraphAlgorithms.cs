using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLab5
{
    public class GraphAlgorithms
    {
        private readonly Graph _graph;
		private readonly WriteLog _writeLog;

		public GraphAlgorithms(Graph graph, WriteLog writeLog)
        {
            _graph = graph;
			_writeLog = writeLog;
        }
		
        //Обход в ширину
        public void Bft(string node)
        {
			if (!_graph.Nodes.ContainsKey(node)) throw new Exception("Error");
            //Список посещенных узлов
            Dictionary<string, bool> visitedNodes = new();
            foreach (var e in _graph.Nodes)
            {
                visitedNodes.Add(e.Key, false);
            }
			
            //Очередь узлов для посещения
            LinkedList<string> queue = new();

			_writeLog($"Обход в ширину\n" +
				$"Начинаем с узла {node}\n");
			//Отмечаем первый узел и добавляем в очередь
			_writeLog($"Отмечаем узел {node}\n");
			visitedNodes[node] = true;
			_writeLog($"Добавляем узел {node} в очередь\n");
			queue.AddLast(node);
			_writeLog("Очередь:");
			foreach (var e in queue)
			{
				_writeLog($" {e}");
			}
			_writeLog("\n-----\n");

			while (queue.Any())
            {
				//Убираем первый узел в очереди
				node = queue.First();
				_writeLog($"Берём первый узел из очереди: {node}\n");
				queue.RemoveFirst();

                //Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
                List<string> neighbours = _graph.Nodes[node].Neighbours;
				_writeLog($"Соседи узла {node}:");
				foreach (var e in neighbours)
				{
					_writeLog($" {e}");
				}
				_writeLog("\n");

				foreach (var val in neighbours)
                {
                    if (!visitedNodes[val])
                    {
                        visitedNodes[val] = true;
                        queue.AddLast(val);
                    }
                }

				_writeLog("Очередь:");
				foreach (var e in queue)
				{
					_writeLog($" {e}");
				}
				_writeLog("\n------\n");
            }
        }

        public void FindWay(string start, string finish)
		{
			if (!_graph.Nodes.ContainsKey(start)
			    || !_graph.Nodes.ContainsKey(finish))
				throw new Exception("Error");
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = _graph.Nodes.ToDictionary(e => e.Key, e => false);
			
			//Словарь вершин с расстоянием до них
			Dictionary<string, int> distanceToNodes = _graph.Nodes.ToDictionary(e => e.Key, e => 0);

			//Очередь узлов для посещения
			LinkedList<string> queue = new();

			_writeLog($"Обход в ширину\n" + $"Начинаем с узла {start}\n");
			//Отмечаем первый узел и добавляем в очередь
			_writeLog($"Отмечаем узел {start}\n");
			visitedNodes[start] = true;
			_writeLog($"Добавляем узел {start} в очередь\n");
			queue.AddLast(start);
			_writeLog("Очередь:");
			foreach (var e in queue)
			{
				_writeLog($" {e}");
			}
			_writeLog("\n-----\n");

			while(queue.Any())
			{
				start = queue.First();
				_writeLog($"Берём первый узел из очереди: {start}\n");
				queue.RemoveFirst();

				//Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
				List<string> neighbours = _graph.Nodes[start].Neighbours;
				_writeLog($"Соседи узла {start}:");
				foreach (var e in neighbours)
				{
					if (distanceToNodes[e] == 0)
					{
						distanceToNodes[e] = distanceToNodes[start] + 1;
					}
					_writeLog($" {e}");
				}
				_writeLog("\n");
				foreach (var val in neighbours.Where(val => !visitedNodes[val]))
				{
					visitedNodes[val] = true;
					queue.AddLast(val);
				}
				_writeLog("Очередь:");
				foreach (var e in queue)
				{
					_writeLog($" {e}");
				}
				_writeLog("\n");
				
				_writeLog("\n------\n");
			}
			_writeLog(GetWay(distanceToNodes, finish));
		}

		private string GetWay(Dictionary<string, int> distanceToNodes, string finish)
		{
			var distance = distanceToNodes[finish];
			string way = finish + " >- ";
			var length = distanceToNodes[finish];
			var temp = finish;
			bool exit = false;
			while (!exit)
			{
				foreach (var node in distanceToNodes)
				{
					if (node.Value == length )
					{
						length--;
						foreach (var prevNode in distanceToNodes)
						{
							if (prevNode.Value == length && _graph.Nodes[prevNode.Key].Neighbours.Contains(temp))
							{
								way += prevNode.Key + " >- ";
								temp = prevNode.Key;
								break;
							}
						}
					}
				}

				if (way.Split(" >- ").Length > distance && way.Split(" >- ")[distance] == "0")
				{
					exit = true;
				}
			}
			
			way = way.Remove(way.Length - 3);
			return $"Длина пути равна {distance}, путь -" + way.Reverse().Aggregate<char, string>(null, (current, c) => current + c);
		}
    }
}
