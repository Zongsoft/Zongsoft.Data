using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Data.Benchmark
{
	class Program
	{
		private const int COUNT = 1000;

		static void Main(string[] args)
		{
			//Tests.CreateObjectTest.Test(COUNT);
			//Tests.CreateObjectTest.Test(COUNT);
			//Tests.CreateObjectTest.Test(COUNT);
			//Tests.CreateObjectTest.Test(COUNT);
			//Tests.CreateObjectTest.Test(COUNT);

			DataPopulatorTest.Test(COUNT);

			Console.ReadLine();
		}
	}
}
