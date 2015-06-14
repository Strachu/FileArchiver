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
using System.Linq.Expressions;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Utils
{
	/// <summary>
	/// A version of <see cref="PropertyReference{T}"/> which allows to change the value of the property.
	/// </summary>
	/// <typeparam name="T">
	/// The type of property's value.
	/// </typeparam>
	/// <remarks>
	/// This class can be used only with properties having both getter and setter,
	/// if you need only getter, use the <see cref="PropertyReference{T}"/> class.
	/// </remarks>
	/// <seealso cref="MutablePropertyReference"/>
	internal class MutablePropertyReference<T> : PropertyReference<T>
	{
		private readonly Action<T> mSetter;

		internal MutablePropertyReference(Expression<Func<T>> propertyAccessExpression) : base(propertyAccessExpression)
		{
			var assignmentParameter  = Expression.Parameter(typeof(T));
			var assignmentExpression = Expression.Assign(propertyAccessExpression.Body, assignmentParameter);

			mSetter = Expression.Lambda<Action<T>>(assignmentExpression, assignmentParameter).Compile();
		}

		public new T Value
		{
			get { return base.Value; }
			set { mSetter(value); }
		}
	}

	/// <summary>
	/// A class containing factory methods for <see cref="MutablePropertyReference{T}"/> with automatic type deduction.
	/// </summary>
	internal static class MutablePropertyReference
	{
		/// <summary>
		/// Creates a new instance of the <see cref="MutablePropertyReference{T}"/> class.
		/// </summary>
		/// <param name="propertyAccessExpression">
		/// Lambda in the form of <c>() => obj.Property</c> with the property which should be wrapped.
		/// </param>
		public static MutablePropertyReference<T> To<T>(Expression<Func<T>> propertyAccessExpression)
		{
			return new MutablePropertyReference<T>(propertyAccessExpression);
		}
	}
}
