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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using FileArchiver.Core.Utils;

using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;

namespace FileArchiver.Presentation.Utils
{
	/// <summary>
	/// A base class for ViewModels reducing the amount of code needed to implement property change notification.
	/// </summary>
	public abstract class NotifyPropertyChangedHelper : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Called when the value of any property has been changed.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property.
		/// </param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var ev = PropertyChanged;
			if(ev != null)
			{
				ev(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Set the value of the field raising the property change notification if its value was different than new one.
		/// </summary>
		/// <typeparam name="T">
		/// Value type of field.
		/// </typeparam>
		/// <param name="field">
		/// A reference to the field.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		/// <param name="fieldName">
		/// The name to broadcast in the notification. It is automatically populated if you call this method from inside of property.
		/// </param>
		protected void SetFieldWithNotification<T>(ref T field, T newValue, [CallerMemberName]string fieldName = "")
		{
			if(Object.Equals(field, newValue))
				return;

			field = newValue;

			OnPropertyChanged(fieldName);
		}

		/// <summary>
		/// Sets the value of the property raising the property change notification if its value was different than new one.
		/// </summary>
		/// <typeparam name="T">
		/// Value type of the property.
		/// </typeparam>
		/// <param name="property">
		/// A lambda in the form of <c>() => obj.Property</c> with the property whose value should be changed.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		/// <remarks>
		/// This method allows to set the value of auto implemented property outside its body while still providing notification about changes.
		/// </remarks>
		protected void SetPropertyWithNotification<T>(Expression<Func<T>> property, T newValue)
		{
			var propertyRef = MutablePropertyReference.To(property);

			if(Object.Equals(propertyRef.Value, newValue))
				return;

			propertyRef.Value = newValue;

			OnPropertyChanged(PropertyName.Of(property));
		}
	}
}
