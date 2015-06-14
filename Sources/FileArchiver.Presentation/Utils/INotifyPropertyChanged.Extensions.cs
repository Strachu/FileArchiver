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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Core.Utils;

namespace FileArchiver.Presentation.Utils
{
	internal static class INotifyPropertyChangedExtensions
	{
		/// <summary>
		/// Subscribes to single property changed event.
		/// </summary>
		/// <typeparam name="TPropertyReturn">
		/// The type of the property return value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="notifier">
		/// The object in whose property changes we are interested in.
		/// </param>
		/// <param name="propertyAccessor">
		/// An expression in the form of () => obj.Property which points to the property
		/// for which the property change events should be raised.
		/// </param>
		/// <param name="eventHandler">
		/// The event handler which will be notified about property changes.
		/// </param>
		public static void SubscribeToPropertyChanged<TPropertyReturn>(this INotifyPropertyChanged notifier,
		                                                               Expression<Func<TPropertyReturn>> propertyAccessor,
		                                                               EventHandler eventHandler)
		{
			Contract.Requires(notifier != null);
			Contract.Requires(propertyAccessor != null);
			Contract.Requires(eventHandler != null);

			notifier.PropertyChanged += (sender, e) =>
			{
				if(e.PropertyName == PropertyName.Of(propertyAccessor))
				{
					eventHandler.SafeRaise(notifier, EventArgs.Empty);
				}
			};
		}
	}
}
