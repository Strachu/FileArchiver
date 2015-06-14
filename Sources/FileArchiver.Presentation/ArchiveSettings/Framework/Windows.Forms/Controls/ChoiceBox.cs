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
using System.Windows.Forms;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal class ChoiceBox<TValue> : SettingsControlBase, ISettingsControlWithTitle
	{
		public ChoiceBox(string title, Expression<Func<TValue>> valueProperty, Expression<Func<IEnumerable<TValue>>> availableValuesProperty,
		                               Expression<Func<bool>> enabledProperty, Expression<Func<bool>> visibleProperty)
			: base(visibleProperty ?? (() => MultipleChoicesVisibilityManager.For(availableValuesProperty).Visible))
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);
			Contract.Requires(availableValuesProperty != null);

			enabledProperty = enabledProperty ?? (() => MultipleChoicesEnablementManager.For(availableValuesProperty).Enabled);

			var comboxBox = new ComboBox()
			{
				FormattingEnabled = true,
				DropDownStyle     = ComboBoxStyle.DropDownList
			};

			// The readOnly binding for valueProperty is intended. In write mode the bound value was changed after the control lost focus,
			// thus causing the necessity for clicking on choice box with archive settings two times if it was clicked directly after selecting
			// archive format (because the controls were recreated just after first click on the choice box).
			comboxBox.DataBindings.AddFromExpressionIfSpecified("SelectedItem", valueProperty,   readOnly: true);
			comboxBox.DataBindings.AddFromExpressionIfSpecified("Enabled",      enabledProperty, readOnly: true);

			comboxBox.DataSource = EnumerablePropertyBindingWrapper.For(availableValuesProperty);

			comboxBox.SelectedIndexChanged += (sender, e) =>
			{
				MutablePropertyReference.To(valueProperty).Value = (TValue)comboxBox.SelectedItem;
			};

			Title   = title;
			Control = comboxBox;
		}

		public string Title
		{
			get;
			private set;
		}
	}
}
