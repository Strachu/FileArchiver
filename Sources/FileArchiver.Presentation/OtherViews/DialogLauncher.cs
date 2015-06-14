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
using System.Windows.Forms;

using FileArchiver.Core.Loaders;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Properties;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.OtherViews
{
	internal class DialogLauncher : IDialogLauncher
	{
		public Path AskForArchiveOpenPath(IEnumerable<ArchiveFormatInfo> supportedArchives)
		{
			var openDialog = new OpenFileDialog
			{
				CheckFileExists              = true,
				CheckPathExists              = true,
				DereferenceLinks             = true,
				Multiselect                  = false,
				SupportMultiDottedExtensions = true,
				ValidateNames                = true,

				Filter                       = ArchiveFileFilterBuilder.BuildFilter(supportedArchives.ToList())
			};

			return (openDialog.ShowDialog() == DialogResult.OK) ? new Path(openDialog.FileName) : null;
		}

		public Path AskForDestinationDirectoryForExtraction()
		{
			var browseFolderDialog = new FolderBrowserDialog
			{
				Description         = Resources.ExtractFileBrowseDirectoryDescription,
				ShowNewFolderButton = true
			};

			return (browseFolderDialog.ShowDialog() == DialogResult.OK) ? new Path(browseFolderDialog.SelectedPath) : null;
		}

		public IEnumerable<Path> AskForFilesToAdd()
		{
			var openDialog = new OpenFileDialog
			{
				CheckFileExists  = true,
				CheckPathExists  = true,
				DereferenceLinks = false,
				Multiselect      = true,
				ReadOnlyChecked  = true,
				ValidateNames    = true,

				Filter           = String.Format("{0} (*.*)|*.*", Resources.AllFiles)
			};

			return (openDialog.ShowDialog() == DialogResult.OK) ? openDialog.FileNames.Select(path => new Path(path)) : new Path[] { };
		}

		public SaveChangesAction AskForSaveChangesAction(FileName archiveName)
		{
			var message = String.Format(Resources.SaveChangesQuestion, archiveName);

			var chosenAction = MessageBox.Show(message, Resources.SaveChangesQuestionTitle, MessageBoxButtons.YesNoCancel,
			                                   MessageBoxIcon.Question);
			switch(chosenAction)
			{
				case DialogResult.Yes:
					return SaveChangesAction.Save;
				case DialogResult.No:
					return SaveChangesAction.Discard;
				default:
					return SaveChangesAction.Cancel;
			}
		}

		public void DisplayError(string message)
		{
			MessageBox.Show(message, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
