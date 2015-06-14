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
using System.Linq.Expressions;
using System.Windows.Forms;
using FileArchiver.Presentation.Properties;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal class DestinationPathBrowser<TValue> : PathBrowser<TValue>
	{
		public DestinationPathBrowser(string title,
		                              Expression<Func<TValue>> valueProperty,
		                              Expression<Func<bool>> enabledProperty,
		                              Expression<Func<bool>> visibleProperty)
			: base(title, valueProperty, enabledProperty, visibleProperty)
		{
		}

		protected override void OnBrowseButtonClick()
		{
			var saveDialog = new SaveFileDialog
			{
				OverwritePrompt = true,
				ValidateNames   = true,
				FileName        = base.PathTextBox.Text,

				Filter          = String.Format("{0}|*.*", Resources.AllFiles)
			};

			if(saveDialog.ShowDialog() == DialogResult.OK)
			{
				base.PathTextBox.Text = saveDialog.FileName;
			}
		}
	}
}
