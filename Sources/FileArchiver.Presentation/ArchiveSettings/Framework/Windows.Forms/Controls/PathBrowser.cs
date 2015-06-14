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
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal abstract class PathBrowser<TValue> : SettingsControlBase, ISettingsControlWithTitle
	{
		protected PathBrowser(string title,
		                      Expression<Func<TValue>> valueProperty,
		                      Expression<Func<bool>> enabledProperty,
		                      Expression<Func<bool>> visibleProperty) : base(visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			PathTextBox = new TextBox
			{
				Dock   = DockStyle.Fill,
				Margin = new Padding(3, 0, 3, 0)
			};

			var browseButton = new Button
			{
				Size                    = new Size(24, 23),
				Dock                    = DockStyle.Fill,
				Margin                  = new Padding(3, 0, 3, 0),
				Text                    = "...",
				UseVisualStyleBackColor = true
			};

			browseButton.Click += (sender, e) => OnBrowseButtonClick();

			var layoutPanel = new TableLayoutPanel
			{
				AutoSize    = true,
				Dock        = DockStyle.Fill,
				Margin      = new Padding(0),
				ColumnCount = 2,
				RowCount    = 1
			};

			layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			layoutPanel.ColumnStyles.Add(new ColumnStyle());

			layoutPanel.Controls.Add(PathTextBox, 0, 0);
			layoutPanel.Controls.Add(browseButton, 1, 0);

			PathTextBox.DataBindings.AddFromExpressionIfSpecified("Text", valueProperty, readOnly: false, formattingEnabled: false);
			PathTextBox.DataBindings.AddFromExpressionIfSpecified("Enabled", enabledProperty, readOnly: true);

			browseButton.DataBindings.AddFromExpressionIfSpecified("Enabled", enabledProperty, readOnly: true);

			Title   = title;
			Control = layoutPanel;
		}

		protected TextBox PathTextBox
		{
			get;
			private set;
		}

		public string Title
		{
			get;
			private set;
		}

		protected abstract void OnBrowseButtonClick();
	}
}
