﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AlgorithmLab5
{
    public class GraphAlgorithms
    {
        private readonly Graph _graph;
		private readonly WriteLog _writeLog;
        public GraphAlgorithms()
        {

        }

        public GraphAlgorithms(Graph graph, WriteLog writeLog)
        {
            _graph = graph;
			_writeLog = writeLog;
        }

        //Рекурсивный метод
        private void DftRecursive(string node, Dictionary<string, bool> visited)
        {
			//Отмечаем полученный узел
			_writeLog($"Отмечаем узел {node}\n");
			visited[node] = true;

            //Выполняем подобное для каждого соседнего узла
            List<string> neighbours = _graph.Nodes[node].Neighbours;
			_writeLog($"Соседи узла {node}:\n");
			foreach (var e in neighbours)
			{
				_writeLog($" {e}");
			}
			_writeLog("\n------\n");
            foreach (var n in neighbours)
            {
                if (!visited[n]) DftRecursive(n, visited);
            }
        }

		//Обход в глубину
		public void Dft(string node)
        {
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = new();
            foreach (var e in _graph.Nodes)
            {
                visitedNodes.Add(e.Key, false);
            }

			//Вызов рекурсивного метода
			_writeLog($"Обход в глубину\n" +
				$"Начинаем с узла {node}\n");
            DftRecursive(node, visitedNodes);
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



		//Максимальный поток
		public int FordFulkerson(Graph graph, string s, string t)
		{
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

			_writeLog("Начинаем поиск максимального потока\n");
			_writeLog($"Из узла {s} в {t}\n");

			//Пока путь есть
			while (Bfs(rGraph, s, t, path))
			{
				_writeLog($"Текущий максимальный поток: {maxFlow}\n");
				_writeLog("Текущий путь:");

				//Ищем минимальный поток у данного пути
				int pathFlow = int.MaxValue;
				string u;
				string v;
				for (v = t; v != s; v = path[v])
				{
					u = path[v];
					pathFlow = Math.Min(pathFlow, rGraph[ToLink(u, v)]);
					_writeLog(" " + ToLink(u, v) + "[" + rGraph[ToLink(u, v)] + "]");
				}
				_writeLog($"\nМинимальный поток у этого пути: {pathFlow}\n");

				//Уменьшаем пропускную способность
				for (v = t; v != s; v = path[v])
				{
					u = path[v];
					rGraph[ToLink(u, v)] -= pathFlow;
				}

				//Увеличиваем максимальный поток на поток отдельного пути
				maxFlow += pathFlow;

				_writeLog("-----\n");
			}

			_writeLog("Максимальный поток равен "
							  + maxFlow + "\n");

			return maxFlow;
		}

		//Поиск обходом в ширину
		//Проверяет есть ли путь. Если да, то заполняет его
		private bool Bfs(Dictionary<string, int> rGraph, string s, string t, Dictionary<string, string> path)
		{
			Dictionary<string, bool> visitedNodes = new();
			foreach (var e in _graph.Nodes)
			{
				visitedNodes.Add(e.Key, false);
			}

			List<string> queue = new() {s};
			visitedNodes[s] = true;
			path[s] = null;

			while (queue.Count != 0)
			{
				string u = queue[0];
				queue.RemoveAt(0);

				List<string> list = _graph.Nodes[u].Neighbours;

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
			return s + "-" + t;
		}

		public void FindWay(string start, string finish)
		{
			if (!_graph.Nodes.ContainsKey(start)
			    || !_graph.Nodes.ContainsKey(finish))
				throw new Exception("Error");
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = new();
			foreach (var e in _graph.Nodes)
			{
				visitedNodes.Add(e.Key, false);
			}
			
			//Очередь узлов для посещения
			LinkedList<string> queue = new();

			_writeLog($"Обход в ширину\n" +
			          $"Начинаем с узла {start}\n");
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
			
			int length = 0;
			bool exit = false;
			bool copied = true;
			var lvlList = new List<string>();
			int countSteps = 0;
			string st = start;
			string neighbour = null;
			while(queue.Any())
			{
				start = queue.First();
				_writeLog($"Берём первый узел из очереди: {start}\n");
				queue.RemoveFirst();
				
				if (!lvlList.Contains(start))
				{
					lvlList = new List<string>();
					length++;
				}
				
				//Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
				List<string> neighbours = _graph.Nodes[start].Neighbours;
				_writeLog($"Соседи узла {start}:");
				foreach (var e in neighbours)
				{
					foreach (var node in lvlList)
					{
						if (e == node)
						{
							copied = false;
							break;
						}
					}
					if (copied)
					{
						lvlList = neighbours;
					}
					if (e == finish && countSteps == 0)
					{
						exit = true;
						_writeLog($" {e}");
						break;
					}
					if(e == finish)
					{
						length++;
						exit = true;
						_writeLog($" {e}");
						break;
					}
					
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
				_writeLog("\n");
				if (exit)
				{
					_writeLog($"Длина кратчайшего пути равна {length}\n");
					_writeLog($"Путь {GetWay(st, finish, length, start)}");
					break;
				}
				_writeLog("\n------\n");
				countSteps++;
			}
		}

		private string GetWay(string st, string finish, int length, string prev)
		{
			var start = st[0];
			string way = " ";
			_graph.Links.Reverse();
			var target = finish;
			int count = 0;
			var skipList = new List<string>();
			if (length == 1)
			{
				way += finish + " >- " + prev;
			}
			else
			{
				while(way[^1] != start)
				{
					way = " ";
					foreach (var link in _graph.Links)
					{
						bool skip = false;
						foreach (var e in skipList)
						{
							if (target == e)
							{
								target = finish;
							}
							if (link.Source == e)
							{
								skip = true;
							}
						}

						if (skip)
						{
							continue;
						}
						if (link.Target == target)
						{
							count++;
							way += link.Target + " >- ";
							target = link.Source;
						}

						if (target == start.ToString()) 
						{
							way += start;
							break;
						}
					}
					if (count != length)
					{
						skipList.Add(target);
					}
				}
			}
			
			_graph.Links.Reverse();
			return way.Reverse().Aggregate<char, string>(null, (current, c) => current + c);
		}
    }
}
