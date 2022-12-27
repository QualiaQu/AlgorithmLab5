using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AlgorithmLab5;

namespace Graph.WPF
{
	public class Field
	{
		readonly Dictionary<string, Point> _points;
		public AlgorithmLab5.Graph Graph;
		readonly GraphAlgorithms _algorithms;
		readonly int _radius = 30;
		readonly Canvas _canvas;
		readonly WriteLog _writeLog;
		
		private readonly int _strokeThickness = 2;
		private readonly SolidColorBrush _defaultColor = Brushes.Black;

		public Field(Canvas canvas, WriteLog writeLog, AlgorithmLab5.Graph graph)
		{
			_canvas = canvas;
			Graph = graph;
			_writeLog = writeLog;
			_algorithms = new(graph, writeLog);
			_points = new();
		}
		

		public void AddNode(string name)
		{
			Graph.AddNode(name);
		}

		public void AddLink(string link, int weight)
		{
			Graph.AddLink(link, weight);
		}

		public void RemoveNode(string name)
		{
			Graph.RemoveNode(name);
			_points.Remove(name);
		}

		public void RemoveLink(string link)
		{
			Graph.RemoveLink(link);
		}

		public void Bft(string node)
		{
			try
			{
				var input = node.Split(" ");
				DoBft(input[0], Convert.ToInt32(input[1]));
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}

		private async void DoBft(string node, int time)
		{
			if (!Graph.Nodes.ContainsKey(node)) throw new Exception("Error");
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = new();
			List<string> visited = new List<string>();

			foreach (var e in Graph.Nodes)
			{
				visitedNodes.Add(e.Key, false);
			}
			
			//Очередь узлов для посещения
			LinkedList<string> queue = new();

			_writeLog($"Обход в ширину\n" +
			          $"Начинаем с узла {node}\n");
			//Отмечаем первый узел и добавляем в очередь
			_writeLog($"Отмечаем узел {node}\n");
			await Task.Delay(time);
			visitedNodes[node] = true;
			visited.Add(node);
			DrawSpecialNodes(Graph, _points, visited, queue);
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
				visitedNodes[node] = true;
				visited.Add(node);
				queue.RemoveFirst();
				//Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
				List<string> neighbours = Graph.Nodes[node].Neighbours;
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
				
				DrawSpecialNodes(Graph, _points, visited, queue);
				await Task.Delay(time);
			}
		}

		public void Dft(string node)
		{
			try
			{
				var input = node.Split(" ");
				DoDft(input[0], Convert.ToInt32(input[1]));
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}
		

		//Обход в глубину
		private async void DoDft(string node, int time)
		{
			if (!Graph.Nodes.ContainsKey(node)) throw new Exception("Error");
			//Список посещенных узлов
			Dictionary<string, bool> visitedNodes = new();
			List<string> visited = new List<string>();

			foreach (var e in Graph.Nodes)
			{
				visitedNodes.Add(e.Key, false);
			}
			
			//Очередь узлов для посещения
			Stack<string> stack = new();

			_writeLog($"Обход в глубину\n" + $"Начинаем с узла {node}\n");
			//Отмечаем первый узел и добавляем в очередь
			_writeLog($"Отмечаем узел {node}\n");
			await Task.Delay(time);
			visitedNodes[node] = true;
			visited.Add(node);
			DrawSpecialNodes(Graph, _points, visited, stack);
			_writeLog($"Добавляем узел {node} в стек\n");
			stack.Push(node);
			_writeLog("Стек:");
			foreach (var e in stack)
			{
				_writeLog($" {e}");
			}
			_writeLog("\n-----\n");
			
			while (stack.Any())
			{
				
				//Убираем первый узел в очереди
				node = stack.Pop();
				_writeLog($"Берём первый узел из стека: {node}\n");
				visitedNodes[node] = true;
				visited.Add(node);
				//Получаем список соседних узлов
				//Отмечаем их и добавляем в очередь
				List<string> neighbours = Graph.Nodes[node].Neighbours;
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
						stack.Push(val);
					}
				}
				
				_writeLog("Стек:");
				foreach (var e in stack)
				{
					_writeLog($" {e}");
				}
				_writeLog("\n------\n");
				
				DrawSpecialNodes(Graph, _points, visited, stack);
				await Task.Delay(time);
			}
		}

		private void DrawSpecialNodes(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, List<string> vList, Stack<string> stack)
		{
			foreach(var k in pc.Keys)
			{
				Ellipse ellipse = new()
				{
					Width = _radius,
					Height = _radius,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness,
					RenderTransform = new TranslateTransform(pc[k].X, pc[k].Y)
				};
				if (vList.Contains(k))
				{
					ellipse.Fill = Brushes.Blue;
				}
				else if(stack.Contains(k))
				{
					ellipse.Fill = Brushes.Aqua;
				}
				else
				{
					ellipse.Fill = Brushes.White;
				}
				_canvas.Children.Add(ellipse);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = graph.Nodes[k].Name,
					RenderTransform = new TranslateTransform(pc[k].X + 5, pc[k].Y + 4)
				};
				_canvas.Children.Add(text);
			}
		}

		public void MaxFlow(string pair)
		{
			try
			{
				string[] arr = pair.Split('-');
				FordFulkerson(arr[0], arr[1].Split(" ")[0], Convert.ToInt32(arr[1].Split(" ")[1]));
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}
		//Максимальный поток
		private async void FordFulkerson(string s, string t, int time)
		{
			//Граф остаточного потока
			Dictionary<string, int> rGraph = new();
			foreach(var l in Graph.Links)
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
				await Task.Delay(time);
				//Ищем минимальный поток у данного пути
				int pathFlow = int.MaxValue;
				string u;
				string v;
				var links = new List<string>();
				for (v = t; v != s; v = path[v])
				{
					u = path[v];
					pathFlow = Math.Min(pathFlow, rGraph[ToLink(u, v)]);
					var link = ToLink(u, v);
					links.Add(link);
					_writeLog(" " + link + "[" + rGraph[link] + "]");
					_canvas.Children.Clear();
					Draw(true, links, rGraph);
					await Task.Delay(time);
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
				_canvas.Children.Clear();
				Draw(true, null, rGraph);
				await Task.Delay(time);
			}
			
			_writeLog("Максимальный поток равен "
			          + maxFlow + "\n");
		}
		//Поиск обходом в ширину
		//Проверяет есть ли путь. Если да, то заполняет его
		private bool Bfs(Dictionary<string, int> rGraph, string s, string t, Dictionary<string, string> path)
		{
			Dictionary<string, bool> visitedNodes = new();
			foreach (var e in Graph.Nodes)
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

				List<string> list = Graph.Nodes[u].Neighbours;

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

		private void DrawSpecialNodes(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, 
			List<string> vList,
			LinkedList<string> queue)
		{
			foreach(var k in pc.Keys)
			{
				Ellipse ellipse = new()
				{
					Width = _radius,
					Height = _radius,
					//Fill = Brushes.White,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness,
					RenderTransform = new TranslateTransform(pc[k].X, pc[k].Y)
				};
				if (vList.Contains(k))
				{
					ellipse.Fill = Brushes.Blue;
				}
				else if(queue.Contains(k))
				{
					ellipse.Fill = Brushes.Aqua;
				}
				else
				{
					ellipse.Fill = Brushes.White;
				}
				_canvas.Children.Add(ellipse);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = graph.Nodes[k].Name,
					RenderTransform = new TranslateTransform(pc[k].X + 5, pc[k].Y + 4)
				};
				_canvas.Children.Add(text);
			}
		}
		private void DrawNodes(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc)
		{
			foreach(var k in pc.Keys)
			{
				Ellipse ellipse = new()
				{
					Width = _radius,
					Height = _radius,
					Fill = Brushes.White,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness,
					RenderTransform = new TranslateTransform(pc[k].X, pc[k].Y)
				};
				_canvas.Children.Add(ellipse);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = graph.Nodes[k].Name,
					RenderTransform = new TranslateTransform(pc[k].X + 5, pc[k].Y + 4)
				};
				_canvas.Children.Add(text);
			}
		}

		private void DrawLinks(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, bool arrow, List<string> links)
		{
			foreach (var l in graph.Links)
			{
				Line line = new()
				{
					X1 = pc[l.Source].X + _radius / 2,
					Y1 = pc[l.Source].Y + _radius / 2,
					X2 = pc[l.Target].X + _radius / 2,
					Y2 = pc[l.Target].Y + _radius / 2,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness
				};
				if (links != null && links.Contains(l.Source + "-" + l.Target))
				{
					line.Stroke = Brushes.Red;
				}
				_canvas.Children.Add(line);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = l.Weight.ToString(),
					RenderTransform = new TranslateTransform((pc[l.Target].X + pc[l.Source].X + _radius) / 2,
						(pc[l.Target].Y + pc[l.Source].Y + _radius) / 2)
				};
				_canvas.Children.Add(text);
				if (arrow)
				{
					DrawArrow(_points[l.Source].X + _radius / 2, _points[l.Source].Y + _radius / 2,
						_points[l.Target].X + _radius / 2, _points[l.Target].Y + _radius / 2);
				}
			}
		}
		private void DrawLinksWithFlow(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, bool arrow, List<string> links, Dictionary<string, int> rGraph)
		{
			foreach (var l in graph.Links)
			{
				Line line = new()
				{
					X1 = pc[l.Source].X + _radius / 2,
					Y1 = pc[l.Source].Y + _radius / 2,
					X2 = pc[l.Target].X + _radius / 2,
					Y2 = pc[l.Target].Y + _radius / 2,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness
				};
				if (links != null && links.Contains(l.Source + "-" + l.Target))
				{
					line.Stroke = Brushes.Red;
				}
				_canvas.Children.Add(line);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = $"{rGraph[l.Source + "-" + l.Target]}/" + l.Weight.ToString(),
					RenderTransform = new TranslateTransform((pc[l.Target].X + pc[l.Source].X + _radius) / 2,
						(pc[l.Target].Y + pc[l.Source].Y + _radius) / 2)
				};
				_canvas.Children.Add(text);
				if (arrow)
				{
					DrawArrow(_points[l.Source].X + _radius / 2, _points[l.Source].Y + _radius / 2,
						_points[l.Target].X + _radius / 2, _points[l.Target].Y + _radius / 2);
				}
			}
		}

		private void DrawBlackLink(AlgorithmLab5.Graph graph, Link op)
		{
			foreach (var l in graph.Links)
			{
				if (l.Source == op.Source && l.Target == op.Target)
				{
					Line line = new()
					{
						X1 = _points[l.Source].X + _radius / 2,
						Y1 = _points[l.Source].Y + _radius / 2,
						X2 = _points[l.Target].X + _radius / 2,
						Y2 = _points[l.Target].Y + _radius / 2,
						Stroke = _defaultColor,
						StrokeThickness = _strokeThickness
					};
					_canvas.Children.Add(line);
					TextBlock text = new()
					{
						FontSize = 16,
						Text = l.Weight.ToString(),
						RenderTransform = new TranslateTransform((_points[l.Target].X + _points[l.Source].X + _radius) / 2,
							(_points[l.Target].Y + _points[l.Source].Y + _radius) / 2)
					};
					_canvas.Children.Add(text);
					break;
				}
			}
		}
		private void DrawGrayLinks(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc)
		{
			foreach (var l in graph.Links)
			{
				Line line = new()
				{
					X1 = pc[l.Source].X + _radius / 2,
					Y1 = pc[l.Source].Y + _radius / 2,
					X2 = pc[l.Target].X + _radius / 2,
					Y2 = pc[l.Target].Y + _radius / 2,
					Stroke = Brushes.Gainsboro,
					StrokeThickness = _strokeThickness
				};
				_canvas.Children.Add(line);

				TextBlock text = new()
				{
					FontSize = 16,
					Text = l.Weight.ToString(),
					RenderTransform = new TranslateTransform((pc[l.Target].X + pc[l.Source].X + _radius) / 2,
						(pc[l.Target].Y + pc[l.Source].Y + _radius) / 2)
				};
				_canvas.Children.Add(text);
			}
		}

		private void DrawArrow(double x1, double y1, double x2, double y2)
		{
			double d = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

			double x = x2 - x1;
			double y = y2 - y1;

			double x3 = x2 - x / d * 25;
			double y3 = y2 - y / d * 25;

			double xp = y2 - y1;
			double yp = x1 - x2;

			double x4 = x3 + xp / d * 5;
			double y4 = y3 + yp / d * 5;
			double x5 = x3 - xp / d * 5;
			double y5 = y3 - yp / d * 5;

			var line = new Line
			{
				Stroke = Brushes.Black,
				X1 = x2 - x / d * 10,
				Y1 = y2 - y / d * 10,
				X2 = x4,
				Y2 = y4,
				StrokeThickness = _strokeThickness
			};
			_canvas.Children.Add(line);

			line = new Line
			{
				Stroke = Brushes.Black,
				X1 = x2 - x / d * 10,
				Y1 = y2 - y / d * 10,
				X2 = x5,
				Y2 = y5,
				StrokeThickness = _strokeThickness
		};
			_canvas.Children.Add(line);
		}

		public void Draw(bool arrow, List<string> links)
		{
			DrawLinks(Graph, _points, arrow, links);
			DrawNodes(Graph, _points);
		}

		private void Draw(bool arrow, List<string> links, Dictionary<string, int> rGraph)
		{
			DrawLinksWithFlow(Graph, _points, arrow, links, rGraph);
			DrawNodes(Graph, _points);
		}

		private void DrawGray()
		{
			DrawGrayLinks(Graph, _points);
			DrawNodes(Graph, _points);
		}
		
		public void ArrangeInCircle(double width, double height)
		{
			int i = 0;
			_points.Clear();

			foreach(var e in Graph.Nodes)
			{
				if(!_points.ContainsKey(e.Key)) _points.Add(e.Key, new(0.4 * width * Math.Cos(i * 2 * Math.PI / Graph.Nodes.Count) + width * 0.475,
					0.4 * height * Math.Sin(i * 2 * Math.PI / Graph.Nodes.Count) + height * 0.475));
				i++;
			}
		}
		public void ArrangeInHexagon()
		{
			var initX = 400;
			var initY = 50;

			var stepX = 250;
			var stepY = 150;
			
			var source = Graph.Nodes.First().Key;
			var sourcePoint = new Point(initX, initY);
			
			var stock = Graph.Nodes.Last().Key;
			var commonNodes = GetCountCommonNodes();
			Point stockPoint;
			stockPoint = commonNodes % 2 == 0 ? new Point(initX, initY + stepY * (commonNodes - 1)) : new Point(initX, initY + stepY * (commonNodes));
			
			_points.Clear();
			var i = 1;
			foreach(var node in Graph.Nodes.Keys)
			{
				if (node == source)
				{
					_points.Add(node, sourcePoint);
					continue;
				}
				if (node == stock)
				{
					_points.Add(node, stockPoint);
					continue;
				}
				if (Convert.ToInt32(node) % 2 != 0)
				{
					_points.Add(node, new Point(initX - stepX, initY + stepY * i));
				}
				else
				{
					_points.Add(node, new Point(initX + stepX, initY + stepY * i));
					i++;
				}
			}
		}
		

		private int GetCountCommonNodes()
		{
			int count = 0;
			foreach (var node in Graph.Nodes.Keys)
			{
				if (node != Graph.Nodes.Keys.First() && node != Graph.Nodes.Keys.Last())
				{
					count++;
				}
			}
			return count;
		}
		private int GetSumWeights(AlgorithmLab5.Graph graph)
		{
			return graph.Links.Sum(link => link.Weight);
		}

		private async void DrawOperations(List<Link> operations, int time)
		{
			int sum = 0;
			_writeLog($"Начинаем со случайно выбранной вершины {operations[0].Source}\n");
			var added = new List<string> {operations[0].Source};
			foreach (var link in Graph.Links)
			{
				if (added.Contains(link.Source))
				{
					_writeLog(
						$"Присоединяем самое лёгкое из рёбер , соединяющих вершину из построенного дерева - {link.Source}, " +
						$"и вершину не из дерева - {link.Target}, вес ребра = {link.Weight}\n");
					_writeLog($"Добавляем вершину {link.Target} в остов и отмечаем как пройденную\n\n");
					added.Add(link.Target);
				}
				else
				{
					_writeLog(
						$"Присоединяем самое лёгкое из рёбер , соединяющих вершину из построенного дерева - {link.Target}, " +
						$"и вершину не из дерева - {link.Source}, вес ребра = {link.Weight}\n");
					_writeLog($"Добавляем вершину {link.Source} в остов и отмечаем как пройденную\n\n");
					added.Add(link.Source);
				}
				sum += link.Weight; 
				Line line = new()
				{
					X1 = _points[link.Source].X + _radius / 2,
					Y1 = _points[link.Source].Y + _radius / 2,
					X2 = _points[link.Target].X + _radius / 2,
					Y2 = _points[link.Target].Y + _radius / 2,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness
				};
				_canvas.Children.Add(line);
				TextBlock text = new()
				{
					FontSize = 16,
					Text = link.Weight.ToString(),
					RenderTransform = new TranslateTransform((_points[link.Target].X + _points[link.Source].X + _radius) / 2,
						(_points[link.Target].Y + _points[link.Source].Y + _radius) / 2)
				};
				_canvas.Children.Add(text);
				DrawNodes(Graph, _points);
				await Task.Delay(time);
			}
			_writeLog("Остовное дерево построено!\n");
			_writeLog("Суммарный вес рёбер = " + sum);
		}
		public void SpanningTree(string input)
		{
			try
			{
				switch (Graph.IsUndirected)
				{
					case false when !Graph.IsMst:
						DoUndirected();
						Draw(false, null);
						_writeLog("Теперь граф неориентированный");
						break;
					case true when !Graph.IsMst:
					{
						DrawGray();
						var result = SpanningTreeByPrim();
						Graph = result.Item1;
						DrawOperations(result.Item2, Convert.ToInt32(input));
						
						break;
					}
					default:
						Draw(false, null);
						break;
				}
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
			
		}

		private (AlgorithmLab5.Graph, List<Link>) SpanningTreeByPrim()
		{
			var mst = new List<Link>();
			//неиспользованные ребра
			var notUsedE  = Graph.Links;
			//использованные вершины
			var usedV  = new List<int>();
			//неиспользованные вершины
			var notUsedV  = new List<int>();
			for (int i = 0; i < Graph.Nodes.Count; i++)
				notUsedV.Add(i);
			Random rand = new Random();
			var start = rand.Next(0, Graph.Nodes.Count);
			usedV.Add(start);
			notUsedV.RemoveAt(usedV[0]);
			while (notUsedV.Count > 0)
			{
				int minE = -1; //номер наименьшего ребра
				//поиск наименьшего ребра
				for (int i = 0; i < notUsedE.Count; i++)
				{
					if ((usedV.IndexOf(Convert.ToInt32(notUsedE[i].Source)) == -1 ||
					     notUsedV.IndexOf(Convert.ToInt32(notUsedE[i].Target)) == -1) &&
					    (usedV.IndexOf(Convert.ToInt32(notUsedE[i].Target)) == -1 ||
					     notUsedV.IndexOf(Convert.ToInt32(notUsedE[i].Source)) == -1)) continue;
					if (minE != -1)
					{
						if (notUsedE[i].Weight < notUsedE[minE].Weight)
							minE = i;	
					}
					else
						minE = i;
				}

				//заносим новую вершину в список использованных и удаляем ее из списка неиспользованных
				if (usedV.IndexOf(Convert.ToInt32(notUsedE[minE].Source)) != -1)
				{
					usedV.Add(Convert.ToInt32(notUsedE[minE].Target));
					notUsedV.Remove(Convert.ToInt32(notUsedE[minE].Target));
				}
				else
				{
					usedV.Add(Convert.ToInt32(notUsedE[minE].Source));
					notUsedV.Remove(Convert.ToInt32(notUsedE[minE].Source));
				}
				//заносим новое ребро в дерево и удаляем его из списка неиспользованных

				mst.Add(notUsedE[minE]);
				notUsedE.RemoveAt(minE);
			}
			
			var graph = new AlgorithmLab5.Graph();
			foreach (var link in mst)
			{
				graph.AddLink(link.Source + "-" + link.Target, link.Weight);
			}

			graph.IsMst = true;
			Graph.IsMst = true;

			return (graph, mst);
		}
		private void Print(string text)
		{
			_writeLog(text);
		}
		private void DoUndirected()
		{
			var newLinks = new Dictionary<string, int>();
			foreach (var link in Graph.Links) newLinks.Add(link.Target + "-" + link.Source, link.Weight);
			foreach (var link in newLinks)
			{
				Graph.AddLink(link.Key, link.Value);
			}

			Graph.IsUndirected = true;
		}

		public void FindWay(string input)
		{
			try
			{
				string[] arr = input.Split('-');
				DoFindWay(arr[0], arr[1].Split(" ")[0], Convert.ToInt32(arr[1].Split(" ")[1]));
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}

		private async void DoFindWay(string start, string finish, int time)
		{
			if (!Graph.Nodes.ContainsKey(start)
			    || !Graph.Nodes.ContainsKey(finish))
				throw new Exception();
			
			Dictionary<string, bool> visitedNodes = Graph.Nodes.ToDictionary(e => e.Key, _ => false);
			Dictionary<string, int> distanceToNodes = Graph.Nodes.ToDictionary(e => e.Key, _ => 0);
			LinkedList<string> queue = new();
			
			visitedNodes[start] = true;
			
			queue.AddLast(start);
			
			while(queue.Any())
			{
				start = queue.First();
				queue.RemoveFirst();
				List<string> neighbours = Graph.Nodes[start].Neighbours;
				
				foreach (var e in neighbours.Where(e => distanceToNodes[e] == 0 && e != start))
				{
					distanceToNodes[e] = distanceToNodes[start] + 1;
				}
				foreach (var val in neighbours.Where(val => !visitedNodes[val]))
				{
					visitedNodes[val] = true;
					queue.AddLast(val);
				}
				_writeLog($"Отмечаем узел {start}\n");
				_writeLog($"Длина пути до него равна {distanceToNodes[start]}\n");
				_writeLog("------\n");
				DrawNodesWithDistance(Graph, _points, distanceToNodes);
				await Task.Delay(time);
			}
			_writeLog(GetWay(distanceToNodes, finish));
		}

		private string GetWay(Dictionary<string, int> distanceToNodes, string finish)
		{
			var distance = distanceToNodes[finish];
			var way = finish + " >- ";
			var length = distanceToNodes[finish] - 1;
			var prevNodes = new List<string> {finish};
			var exit = false;
			while (!exit)
			{
				foreach (var prevNode in distanceToNodes.Where(node => node.Value == length).SelectMany(_ => distanceToNodes))
				{
					if (!prevNodes.Any(prev =>
						    prevNode.Value == length && Graph.Nodes[prevNode.Key].Neighbours.Contains(prev))) continue;
					
					length--;
					way += prevNode.Key + " >- ";
					prevNodes.Add(prevNode.Key);
				}

				if(length == -1)
				{
					exit = true;
				}
			}
			
			way = way.Remove(way.Length - 3);
			
			return $"Длина пути равна {distance}, путь -" + way.Reverse().Aggregate<char, string>(null, (current, c) => current + c);
		}
		private void DrawNodesWithDistance(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, Dictionary<string, int> distance)
		{
			foreach(var k in pc.Keys)
			{
				Ellipse ellipse = new()
				{
					Width = _radius,
					Height = _radius,
					Fill = Brushes.White,
					Stroke = _defaultColor,
					StrokeThickness = _strokeThickness,
					RenderTransform = new TranslateTransform(pc[k].X, pc[k].Y)
				};
				_canvas.Children.Add(ellipse);

				TextBlock text = new()
				{
					FontSize = 15,
					Text = graph.Nodes[k].Name + $"({distance[k]})",
					RenderTransform = new TranslateTransform(pc[k].X + 5, pc[k].Y + 4)
				};
				_canvas.Children.Add(text);
			}
		}
	}
}

