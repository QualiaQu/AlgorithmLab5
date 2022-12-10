using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AlgorithmLab5;

namespace Graph.WPF
{
	public class Field
	{
		readonly Dictionary<string, Point> _points;
		public readonly AlgorithmLab5.Graph Graph;
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



		//private void GeneratePoints()
		//{
		//	Random random = new(1);

		//	for (int i = 0; i < 10; i++)
		//	{
		//		points.Add(new Point(random.Next(600), random.Next(600)));
		//	}
		//}

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
				_algorithms.Bft(node);
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}

		public void Dft(string node)
		{
			try
			{
				_algorithms.Dft(node);
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}

		public void MaxFlow(string pair)
		{
			try
			{
				string[] arr = pair.Split('-');

				_algorithms.FordFulkerson(Graph, arr[0], arr[1]);
			}
			catch (Exception)
			{
				_writeLog("ERROR");
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

		private void DrawLinks(AlgorithmLab5.Graph graph, Dictionary<string, Point> pc, bool arrow)
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

		private void DrawArrow(double x1, double y1, double x2, double y2)
		{
			double d = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

			double x = x2 - x1;
			double y = y2 - y1;

			double x3 = x2 - (x / d) * 25;
			double y3 = y2 - (y / d) * 25;

			double xp = y2 - y1;
			double yp = x1 - x2;

			double x4 = x3 + (xp / d) * 5;
			double y4 = y3 + (yp / d) * 5;
			double x5 = x3 - (xp / d) * 5;
			double y5 = y3 - (yp / d) * 5;

			var line = new Line
			{
				Stroke = Brushes.Black,
				X1 = x2 - (x / d) * 10,
				Y1 = y2 - (y / d) * 10,
				X2 = x4,
				Y2 = y4,
				StrokeThickness = _strokeThickness
			};
			_canvas.Children.Add(line);

			line = new Line
			{
				Stroke = Brushes.Black,
				X1 = x2 - (x / d) * 10,
				Y1 = y2 - (y / d) * 10,
				X2 = x5,
				Y2 = y5,
				StrokeThickness = _strokeThickness
		};
			_canvas.Children.Add(line);
		}

		public void Draw(bool arrow)
		{
			DrawLinks(Graph, _points, arrow);
			DrawNodes(Graph, _points);
		}

		public void ArrangeInCircle(double width, double height)
		{
			int i = 0;
			_points.Clear();

			foreach(var e in Graph.Nodes)
			{
				if(!_points.ContainsKey(e.Key))_points.Add(e.Key, new(0.4 * width * Math.Cos(i * 2 * Math.PI / Graph.Nodes.Count) + width * 0.475,
					0.4 * height * Math.Sin(i * 2 * Math.PI / Graph.Nodes.Count) + height * 0.475));
				i++;
			}
		}
		private int GetSumWeights(AlgorithmLab5.Graph graph)
		{
			
			int sum = 0;
			foreach (var link in graph.Links)
			{
				sum += link.Weight;
			}

			return sum;
		}
		

		public AlgorithmLab5.Graph SpanningTree()
		{
			switch (Graph.IsUndirected)
			{
				case false when !Graph.IsMst:
					DoUndirected();
					_writeLog("Теперь граф неориентированный");
					return Graph;
				case true when !Graph.IsMst:
				{
					var graph = _algorithms.SpanningTreeByPrim();
					Print(graph.Item2);
					Print("Суммарный вес рёбер = " + GetSumWeights(graph.Item1));
					return graph.Item1;
				}
				default:
					Print("Суммарный вес рёбер = " + GetSumWeights(Graph));
					return Graph;
			}
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
			// string[] arr = input.Split('-');
			// _algorithms.FindWay(arr[0], arr[1]);
			try
			{
				string[] arr = input.Split('-');
				_algorithms.FindWay(arr[0], arr[1]);
			}
			catch (Exception)
			{
				_writeLog("ERROR");
			}
		}

		private void Print(string text)
		{
			_writeLog(text);
		}
	}
}

