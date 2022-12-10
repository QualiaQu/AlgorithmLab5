using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AlgorithmLab5;
using Microsoft.Win32;

namespace Graph.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private Field _field;
		private string _input;
		private Action _action;
		private AlgorithmLab5.Graph _graph;
		
		private OpenFileDialog _openFileDialog = new ();
		private SaveFileDialog _saveFileDialog = new ();

		enum Action
		{
			AddNode, AddLink, RemoveNode, RemoveLink, Bft, Dft, MaxFlow, MinimumSpanningTree, FindWay, OpenFile, SaveFile
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Start(string filename)
		{
			WriteLog writeLog = WriteInTextBox;
			string[] graphData = File.ReadAllLines(filename);
			_graph = GraphSerializer.Deserialize(graphData, GraphSerializer.GraphStorageType.AdjacencyMatrix);
			_field = new(Canvas, writeLog, _graph);
			_field.ArrangeInCircle(MWindow.Width * 2 / 3, MWindow.Height);
			_field.Draw(!_graph.IsUndirected);
		}

		private void DrawMst(AlgorithmLab5.Graph graph)
		{
			WriteLog writeLog = WriteInTextBox;
			_field = new(Canvas, writeLog, graph);
			_field.ArrangeInCircle(MWindow.Width * 2 / 3, MWindow.Height);
			_field.Draw(false);
		}
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Canvas.Children.Clear();
			Log.Text = "";
			_input = TextBox.Text;
			
			switch (_action)
			{
				case Action.AddNode:
					_field.AddNode(_input);
					_field.Draw(true);
					break;
				case Action.AddLink:
					WeightInput weightInput = new WeightInput();
					if(weightInput.ShowDialog() == true)
					{
						_field.AddLink(_input, weightInput.Weight);
					}
					_field.Draw(true);
					break;
				case Action.RemoveNode:
					_field.RemoveNode(_input);
					_field.Draw(true);
					break;
				case Action.RemoveLink:
					_field.RemoveLink(_input);
					_field.Draw(true);
					break;
				case Action.Bft:
					_field.Bft(_input);
					_field.Draw(true);
					break;
				case Action.Dft:
					_field.Dft(_input);
					_field.Draw(true);
					break;
				case Action.MaxFlow:
					_field.MaxFlow(_input);
					_field.Draw(true);
					break;
				case Action.MinimumSpanningTree:
					DrawMst(_field.SpanningTree());
					break;
				case Action.FindWay:
					_field.FindWay(_input);
					_field.Draw(true);
					break;
				case Action.OpenFile:
					OpenFile();
					break;
				case Action.SaveFile:
					SaveFile();
					break;
				default:
					throw new Exception();
			}

			_field.ArrangeInCircle(MWindow.Width * 2 / 3, MWindow.Height);
		}
		
		private void OpenFile()
		{
			_openFileDialog = new()
			{
				InitialDirectory = Environment.CurrentDirectory,
				DefaultExt = ".csv",
				Filter = "Files (.csv)|*.csv|All files (*.*)|*.*",
				CheckPathExists = true
			};
			if (_openFileDialog.ShowDialog().HasValue)
			{
				Start(_openFileDialog.FileName);
			}
		}

		private void SaveFile()
		{
			_saveFileDialog = new()
			{
				InitialDirectory = Environment.CurrentDirectory,
				DefaultExt = ".csv",
				Filter = "Files (.csv)|*.csv|All files (*.*)|*.*",
				CheckPathExists = true
			};
			if (_saveFileDialog.ShowDialog().HasValue)
			{
				File.WriteAllText(_saveFileDialog.FileName, GraphSerializer.Serialize(_field.Graph));
			}
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_action = (Action)ComboBox.SelectedIndex;
		}

		private void WriteInTextBox(string text)
		{
			Log.Text += text;
		}
	}
}
