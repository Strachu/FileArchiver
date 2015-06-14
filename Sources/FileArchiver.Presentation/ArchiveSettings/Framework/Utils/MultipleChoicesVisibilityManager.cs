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
using System.Linq;
using System.Linq.Expressions;
using FileArchiver.Core.Utils;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Utils
{
	/// <summary>
	/// A helper which automatically manages the visibility of bound control depending on the list of available values.
	/// </summary>
	/// <typeparam name="TValue">
	/// The type of the values in a control.
	/// </typeparam>
	/// <remarks>
	/// The rules are:
	/// - When there is at least 1 choice available the control is visible,
	/// - When there isn't any choice available, the control is hidden.
	/// </remarks>
	internal class MultipleChoicesVisibilityManager<TValue> : NotifyPropertyChangedHelper
	{
		private readonly PropertyReference<IEnumerable<TValue>> mAvailableChoicesProperty;

		internal MultipleChoicesVisibilityManager(Expression<Func<IEnumerable<TValue>>> availableChoicesProperty)
		{
			mAvailableChoicesProperty = PropertyReference.To(availableChoicesProperty);

			mAvailableChoicesProperty.PropertyChanged += (sender, e) =>
			{
				OnPropertyChanged(PropertyName.Of(() => Visible));
			};
		}

		public bool Visible
		{
			get { return mAvailableChoicesProperty.Value.Any(); }
		}
	}

	/// <summary>
	/// Factory methods for <see cref="MultipleChoicesVisibilityManager{T}"/> with automatic type deduction.
	/// </summary>
	internal static class MultipleChoicesVisibilityManager
	{
		public static MultipleChoicesVisibilityManager<TValue> For<TValue>(Expression<Func<IEnumerable<TValue>>> availableChoicesProperty)
		{
			return new MultipleChoicesVisibilityManager<TValue>(availableChoicesProperty);
		}
	}
}
