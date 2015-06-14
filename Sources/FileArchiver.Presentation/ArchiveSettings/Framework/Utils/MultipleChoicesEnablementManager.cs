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
	/// A helper which automatically manages the enablement of bound control depending on the list of available values.
	/// </summary>
	/// <typeparam name="TValue">
	/// The type of the values in a control.
	/// </typeparam>
	/// <remarks>
	/// The rules are:
	/// - When there is more than 1 choice available the control is enabled,
	/// - When there is one or less choices available the control is disabled.
	/// </remarks>
	/// <seealso cref="MultipleChoicesEnablementManager"/>
	internal class MultipleChoicesEnablementManager<TValue> : NotifyPropertyChangedHelper
	{
		private readonly PropertyReference<IEnumerable<TValue>> mAvailableChoicesProperty;

		internal MultipleChoicesEnablementManager(Expression<Func<IEnumerable<TValue>>> availableChoicesProperty)
		{
			mAvailableChoicesProperty = PropertyReference.To(availableChoicesProperty);

			mAvailableChoicesProperty.PropertyChanged += (sender, e) =>
			{
				OnPropertyChanged(PropertyName.Of(() => Enabled));
			};
		}

		public bool Enabled
		{
			get { return mAvailableChoicesProperty.Value.Count() > 1; }
		}
	}

	/// <summary>
	/// Factory methods for <see cref="MultipleChoicesEnablementManager{T}"/> with automatic type deduction.
	/// </summary>
	internal static class MultipleChoicesEnablementManager
	{
		public static MultipleChoicesEnablementManager<TValue> For<TValue>(Expression<Func<IEnumerable<TValue>>> availableChoicesProperty)
		{
			return new MultipleChoicesEnablementManager<TValue>(availableChoicesProperty);
		}
	}
}
