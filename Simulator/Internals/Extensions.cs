using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace Simulator.Internals
{
	public static class Extensions
	{
        /// <summary>
        /// Generates a 'list-of-lists', splits the enumerable into sub-enumerables of specified length
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="sublistLength">The desired sublist length</param>
        /// <returns></returns>
		public static IEnumerable<IEnumerable<T>> Sublist<T>(this IEnumerable<T> source, int sublistLength)
		{
			var list = new List<T>();
			var counter = 0;
			foreach(var item in source)
			{
				list.Add(item);
				counter++;
				if(counter == sublistLength)
				{
					yield return list;
					list = new List<T>();
					counter = 0;
				}
			}

			if (list.Count > 0)
				yield return list;
		}

        public static T DeepCopy<T>(T obj)
        {
            using(var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream,obj);
                memoryStream.Position = 0;

                return (T) formatter.Deserialize(memoryStream);
            }
        }

        public static System.Windows.Point[] ToWindowsPointArray(this IEnumerable<Point> points)
        {
            return points.Select(point => new System.Windows.Point(point.X.Value, point.Y.Value)).ToArray();
        }
	}
}
