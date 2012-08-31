using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Miscellaneous
{
	public class Property
	{
		public static string GetName<T>(Expression<Func<T>> property)
		{
			var memberExpr = property.Body as MemberExpression;

			if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
				return memberExpr.Member.Name;

			throw new InvalidOperationException("No property reference was found");
		}
	}
}
