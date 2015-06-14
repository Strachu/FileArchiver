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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using FileArchiver.Core.Utils;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Utils
{
	/// <summary>
	/// A class representing a "reference" to a property.
	/// </summary>
	/// <typeparam name="T">
	/// The type of property's value.
	/// </typeparam>
	/// <remarks>
	/// The class can be used to store or pass a reference to a property for later use or
	/// to access the value of property representing by an LINQ Expression.
	/// </remarks>
	/// <seealso cref="MutablePropertyReference{T}"/>
	/// <seealso cref="PropertyReference"/>
	internal class PropertyReference<T> : NotifyPropertyChangedHelper
	{
		private readonly string  mPropertyName;
		private readonly Func<T> mPropertyGetter;

		internal PropertyReference(Expression<Func<T>> propertyAccessExpression)
		{
			Contract.Requires(propertyAccessExpression != null);
			Contract.Requires(propertyAccessExpression.Body is MemberExpression, "Excepted a lambda in the form of \"() => obj.Property\".");

			mPropertyName   = PropertyName.Of(propertyAccessExpression);
			mPropertyGetter = propertyAccessExpression.Compile();

			RegisterForPropertyNotification(propertyAccessExpression);
		}

		private void RegisterForPropertyNotification(Expression<Func<T>> propertyAccessExpression)
		{
			var memberExpression       = (MemberExpression)propertyAccessExpression.Body;
			var objectAccessExpression = memberExpression.Expression;
			var objectAccessLamdba     = Expression.Lambda<Func<object>>(objectAccessExpression).Compile();

			var propertyNotifier = objectAccessLamdba() as INotifyPropertyChanged;
			if(propertyNotifier != null)
			{
				propertyNotifier.PropertyChanged += WrappedObjectPropertyChanged;
			}
		}

		private void WrappedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == mPropertyName)
			{
				OnPropertyChanged(PropertyName.Of(() => Value));
			}
		}

		public T Value
		{
			get { return mPropertyGetter(); }
		}
	}

	/// <summary>
	/// A class containing factory methods for <see cref="PropertyReference{T}"/> with automatic type deduction.
	/// </summary>
	internal static class PropertyReference
	{
		/// <summary>
		/// Creates a new instance of the <see cref="PropertyReference{T}"/> class.
		/// </summary>
		/// <param name="propertyAccessExpression">
		/// Lambda in the form of <c>() => obj.Property</c> with the property which should be wrapped.
		/// </param>
		public static PropertyReference<T> To<T>(Expression<Func<T>> propertyAccessExpression)
		{
			return new PropertyReference<T>(propertyAccessExpression);
		}
	}
}
