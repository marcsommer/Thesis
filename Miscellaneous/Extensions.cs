using System;

namespace Miscellaneous
{
	public static class Extensions
	{
		public static TOut NullSafe<TIn,TOut>(this TIn input, Func<TIn, TOut> func) where TIn  : class 
																					where TOut : class
		{
			return input == null ? null : func(input);
		}


		public static string GetName<T>(this T obj)
		{
			return Property.GetName(() => obj);
		}
	}
}
