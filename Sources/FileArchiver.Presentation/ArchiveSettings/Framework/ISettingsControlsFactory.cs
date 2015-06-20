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
using System.Linq.Expressions;

namespace FileArchiver.Presentation.ArchiveSettings.Framework
{
	/// <summary>
	/// A factory for built in controls which can be returned from <see cref="IArchiveSettingsViewModel.SettingControls"/>.
	/// </summary>
	/// <remarks>
	/// For all properties passed to any expression a INotifyPropertyChanged interface notifications can be used to
	/// automatically reflect the change of value of this property in a control.
	/// </remarks>
	[ContractClass(typeof(ISettingsControlsFactoryContractClass))]
	public interface ISettingsControlsFactory
	{
		/// <summary>
		/// Creates a control representing a group of controls.
		/// </summary>
		/// <param name="title">
		/// The title of group.
		/// </param>
		/// <param name="controlsProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound to the group as the actual list of controls in the group.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the group is always visible unless when all of its controls are hidden.
		/// </param>
		ISettingsControl CreateGroup(string title,
										     Expression<Func<IEnumerable<ISettingsControl>>> controlsProperty,
		                             Expression<Func<bool>> visibleProperty = null);

		/// <summary>
		/// Creates a text field.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="title">
		/// The title of setting represented by this control.
		/// </param>
		/// <param name="valueProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound as the actual value of the settings.
		/// </param>
		/// <param name="enabledProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling whether the control is enabled,
		/// that is, whether it is accepting any input. If not specified, the control is always enabled.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the control is always visible.
		/// </param>
		ISettingsControl CreateTextField<TValue>(string title,
		                                         Expression<Func<TValue>> valueProperty,
		                                         Expression<Func<bool>> enabledProperty = null,
		                                         Expression<Func<bool>> visibleProperty = null);

		/// <summary>
		/// Creates a control giving the user a possibility to choose one from predefined list of values.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="title">
		/// The title of setting represented by this control.
		/// </param>
		/// <param name="valueProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound as the actually chosen value.
		/// </param>
		/// <param name="availableValuesProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the list of actually available values
		/// from which the user can choose.
		/// </param>
		/// <param name="enabledProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling whether the control is enabled,
		/// that is, whether the user can change selected value.
		/// If not specified, the control is always enabled as long as there is more than 1 choice available.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the control is always visible as long as there is any choice available.
		/// </param>
		ISettingsControl CreateChoiceBox<TValue>(string title,
		                                         Expression<Func<TValue>> valueProperty,
		                                         Expression<Func<IEnumerable<TValue>>> availableValuesProperty,
			                                      Expression<Func<bool>> enabledProperty = null,
		                                         Expression<Func<bool>> visibleProperty = null);

		/// <summary>
		/// Creates a check box.
		/// </summary>
		/// <param name="title">
		/// The title of setting represented by this control.
		/// </param>
		/// <param name="valueProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound as the actual value of the settings.
		/// </param>
		/// <param name="enabledProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling whether the control is enabled,
		/// that is, whether it's value can be changed by a user. If not specified, the control is always enabled.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the control is always visible.
		/// </param>
		ISettingsControl CreateCheckBox(string title,
		                                Expression<Func<bool>> valueProperty,
		                                Expression<Func<bool>> enabledProperty = null,
		                                Expression<Func<bool>> visibleProperty = null);

		/// <summary>
		/// Creates a control giving the user a possibility to browse for or enter a destination path.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="title">
		/// The title of setting represented by this control.
		/// </param>
		/// <param name="valueProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound as the actual value.
		/// </param>
		/// <param name="enabledProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling whether the control is enabled,
		/// that is, whether it accepts any input.
		/// If not specified, the control is always enabled.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the control is always visible.
		/// </param>
		ISettingsControl CreateDestinationPathBrowser<TValue>(string title,
																				Expression<Func<TValue>> valueProperty,
		                                                      Expression<Func<bool>> enabledProperty = null,
		                                                      Expression<Func<bool>> visibleProperty = null);

		/// <summary>
		/// Creates a control giving the user a possibility to browse for or enter a path to an existing file.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type of the value. Should be deduced automatically.
		/// </typeparam>
		/// <param name="title">
		/// The title of setting represented by this control.
		/// </param>
		/// <param name="valueProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property to be bound as the actual value.
		/// </param>
		/// <param name="enabledProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling whether the control is enabled,
		/// that is, whether it accepts any input.
		/// If not specified, the control is always enabled.
		/// </param>
		/// <param name="visibleProperty">
		/// An expression in the form of <c>() => obj.Property</c> with a property controlling the visibility of the group.
		/// If not specified, the control is always visible.
		/// </param>
		ISettingsControl CreateSourcePathBrowser<TValue>(string title,
																		 Expression<Func<TValue>> valueProperty,
		                                                 Expression<Func<bool>> enabledProperty = null,
		                                                 Expression<Func<bool>> visibleProperty = null);
	}
}
