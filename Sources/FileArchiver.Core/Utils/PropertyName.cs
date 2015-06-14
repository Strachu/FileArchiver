#region Copyright
/*
 * Copyright (C) 2015 Patryk Strach
 * 
 * This file is part of FileArchiver.
 * 
 * FileArchiver is free software: you can redistribute it and/or modify it under the terms of
 * the GNU Lesser General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * FileArchiver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with FileArchiver.
 * If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace FileArchiver.Core.Utils
{
	/// <summary>
	/// A class containing methods allowing to retrieve a property name in a strongly typed fashion
	/// with compile time check support.
	/// </summary>
	public static class PropertyName
	{
		/// <summary>
		/// Retrieves the property name from an LINQ expression.
		/// </summary>
		/// <typeparam name="TMemberReturn">
		/// The type of the property value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="propertyAccessExpression">
		/// The property access expression in the form of <c>() => obj.Property</c>.
		/// </param>
		public static string Of<TMemberReturn>(Expression<Func<TMemberReturn>> propertyAccessExpression)
		{
			Contract.Requires(propertyAccessExpression != null);
			Contract.Requires(propertyAccessExpression.Body is MemberExpression, "Excepted a lambda in the form of \"() => obj.Property\".");
			Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));

			var memberExpression = (MemberExpression)propertyAccessExpression.Body;

			return memberExpression.Member.Name;
		}

		/// <summary>
		/// Retrieves the property name from an LINQ expression when the object with the property is not available.
		/// </summary>
		/// <typeparam name="TObject">
		/// The type of the class whose name of property should be retrieved.
		/// </typeparam>
		/// <param name="propertyAccessExpression">
		/// The property access expression in the form of <c>x => x.Property</c>.
		/// </param>
		/// <example>
		/// To get a name of "TestProperty" property of class TestClass use:
		/// <code>
		/// var propertyName = PropertyName.Of{TestClass}(x => x.TestProperty);
		/// </code>
		/// </example>
		public static string Of<TObject>(Expression<Func<TObject, object>> propertyAccessExpression)
		{
			// The delegate return value is object instead of generic parameter as C# doesn't allow to specify one parameter while inferring the one.
			// If the return value were TMemberReturn, the method would need to specify BOTH TObject and TMemberReturn.

			Contract.Requires(propertyAccessExpression != null);
			Contract.Requires(propertyAccessExpression.Body is MemberExpression, "Excepted a lambda in the form of \"x => x.Property\".");
			Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<string>()));

			var memberExpression = (MemberExpression)propertyAccessExpression.Body;

			return memberExpression.Member.Name;
		}
	}
}
