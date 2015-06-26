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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace FileArchiver.Presentation.ArchiveSettings.Framework
{
	public static class ISettingsControlsFactoryExtensions
	{
		/// <summary>
		/// Convenience overload of
		/// <see cref="ISettingsControlsFactory.CreateGroup(string, Expression{Func{IEnumerable{ISettingsControl}}}, Expression{Func{bool}})">CreateGroup()</see>
		/// to allow inline creation of array of controls when data binding isn't needed.
		/// </summary>
		/// <example>
		/// This method allows method calls in the form of:
		/// <code>
		/// mFactory.CreateGroup("Name of the group", new ISettingsControl[]
		/// {
		///    mFactory.Create...,
		///    mFactory.Create...
		/// }
		/// </code>
		/// </example>
		public static ISettingsControl CreateGroup(this ISettingsControlsFactory factory, string title,
		                                           IReadOnlyCollection<ISettingsControl> controls,
		                                           Expression<Func<bool>> visibleProperty = null)
		{
			Contract.Requires(factory != null);
			Contract.Requires(controls != null);
			Contract.Requires(Contract.ForAll(controls, control => control != null));

			return factory.CreateGroup(title, () => controls, visibleProperty);
		}

		/// <summary>
		/// An overload of
		/// <see cref="ISettingsControlsFactory.CreateChoiceBox{TValue}(string, Expression{Func{TValue}}, Expression{Func{IEnumerable{TValue}}}, Expression{Func{bool}}, Expression{Func{bool}})">CreateChoiceBox()</see>
		/// which automatically adds all possible values of an enum to the choice box.
		/// </summary>
		public static ISettingsControl CreateChoiceBox<TEnum>(this ISettingsControlsFactory factory, string title,
		                                                     Expression<Func<TEnum>> valueProperty,
		                                                     Expression<Func<bool>> enabledProperty = null,
		                                                     Expression<Func<bool>> visibleProperty = null)
			where TEnum : struct
		{
			Contract.Requires(factory != null);
			Contract.Requires(typeof(TEnum).IsEnum);

			var allPossibleEnumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();

			return factory.CreateChoiceBox(title, valueProperty, () => allPossibleEnumValues, enabledProperty, visibleProperty);
		}
	}
}
