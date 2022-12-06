using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmLab5
{
	public static class FileWorker
	{
		public static string[] ReadFile(string path)
		{
			return File.ReadAllLines(path);
		}
	}
}
