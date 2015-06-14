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
using System.Linq.Expressions;
using FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms
{
	internal class SettingsControlFactory : ISettingsControlsFactory
	{
		public ISettingsControl CreateGroup(string title,
		                                    Expression<Func<IEnumerable<ISettingsControl>>> controlsProperty,
		                                    Expression<Func<bool>> visibleProperty)
		{
			return new GroupBox(title, controlsProperty, visibleProperty);
		}

		public ISettingsControl CreateTextField<TValue>(string title,
		                                                Expression<Func<TValue>> valueProperty,
																		Expression<Func<bool>> enabledProperty,
																		Expression<Func<bool>> visibleProperty)
		{
			return new TextField<TValue>(title, valueProperty, enabledProperty, visibleProperty);
		}

		public ISettingsControl CreateChoiceBox<TValue>(string title,
		                                                Expression<Func<TValue>> valueProperty,
																		Expression<Func<IEnumerable<TValue>>> availableValuesProperty,
																		Expression<Func<bool>> enabledProperty,
																		Expression<Func<bool>> visibleProperty)
		{
			return new ChoiceBox<TValue>(title, valueProperty, availableValuesProperty, enabledProperty, visibleProperty);
		}

		public ISettingsControl CreateDestinationPathBrowser<TValue>(string title,
		                                                             Expression<Func<TValue>> controlsProperty,
																						 Expression<Func<bool>> enabledProperty,
																						 Expression<Func<bool>> visibleProperty)
		{
			return new DestinationPathBrowser<TValue>(title, controlsProperty, enabledProperty, visibleProperty);
		}

		public ISettingsControl CreateSourcePathBrowser<TValue>(string title,
		                                                        Expression<Func<TValue>> controlsProperty,
																				  Expression<Func<bool>> enabledProperty,
																				  Expression<Func<bool>> visibleProperty)
		{
			return new SourcePathBrowser<TValue>(title, controlsProperty, enabledProperty, visibleProperty);
		}
	}
}
