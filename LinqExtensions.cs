using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Snippets
{
	public static class LinqExtensions
	{
		public static IEnumerable<IEnumerable<T>> EachCons<T>(this IEnumerable<T> source, int size)
		{
			int i = 0;
			IList<T> list = new List<T>();
			foreach (var arg in source)
			{
				list.Add(arg);
				++i;
				if (i != size) continue;
				i = 0;
				yield return list;
				list = new List<T>();
			}
		}
	}

	public class LinqExtensionsTest
	{
		[Fact]
		public void Test()
		{
			var eachConsResult = Enumerable.Range(1, 10).EachCons(2).ToList();

			Assert.Equal(5, eachConsResult.Count());
			Assert.Equal(1, eachConsResult.First().First());
			Assert.Equal(10, eachConsResult.Last().Last());
		}
	}
}
