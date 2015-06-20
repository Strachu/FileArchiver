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
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal class CheckBox : SettingsControlBase, ISettingsControlWithTitle
	{
		public CheckBox(string title,
		                Expression<Func<bool>> valueProperty,
		                Expression<Func<bool>> enabledProperty,
		                Expression<Func<bool>> visibleProperty) : base(visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			var checkBox = new System.Windows.Forms.CheckBox();

			checkBox.DataBindings.AddFromExpressionIfSpecified("Checked", valueProperty,   readOnly: false);
			checkBox.DataBindings.AddFromExpressionIfSpecified("Enabled", enabledProperty, readOnly: true);

			Title   = title;
			Control = checkBox;
		}

		public string Title
		{
			get;
			private set;
		}
	}
}
