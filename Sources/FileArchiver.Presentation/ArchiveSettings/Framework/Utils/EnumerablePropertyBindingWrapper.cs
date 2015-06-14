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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

using FileArchiver.Core.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Utils
{
	/// <summary>
	/// A class to enable data binding to a <b>property</b> with a collection instead of binding to just collection.
	/// </summary>
	/// <typeparam name="T">
	/// The type of values stored in a bound collection.
	/// </typeparam>
	/// <remarks>
	/// The difference between binding to a property and to a collection is that in the case of binding to a property you
	/// can assign entire new collection and raise a property notification and the binding will work while in the case
	/// of binding to a raw collection you cannot use assignment to change the values.
	/// </remarks>
	internal class EnumerablePropertyBindingWrapper<T> : BindingList<T>
	{
		private readonly PropertyReference<IEnumerable<T>> mBoundPropertyReference;

		internal EnumerablePropertyBindingWrapper(Expression<Func<IEnumerable<T>>> property)
		{
			mBoundPropertyReference = PropertyReference.To(property);

			mBoundPropertyReference.PropertyChanged += (sender, e) =>
			{
				SyncCollections();

				ResetBindings();
			};

			SyncCollections();

			base.AllowEdit   = false;
			base.AllowNew    = false;
			base.AllowRemove = false;
		}

		private void SyncCollections()
		{
			base.Items.Clear();

			var collection = mBoundPropertyReference.Value;
			if(collection != null)
			{
				mBoundPropertyReference.Value.CopyTo(base.Items);
			}
		}
	}

	/// <summary>
	/// A class containing factory methods for <see cref="EnumerablePropertyBindingWrapper{T}"/> with automatic type deduction.
	/// </summary>
	internal static class EnumerablePropertyBindingWrapper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyReference"/> class.
		/// </summary>
		/// <param name="property">
		/// Lambda in the form of <c>() => obj.Property</c> with the collection property which should be wrapped.
		/// </param>
		public static EnumerablePropertyBindingWrapper<T> For<T>(Expression<Func<IEnumerable<T>>> property)
		{
			return new EnumerablePropertyBindingWrapper<T>(property);
		}
	}
}
