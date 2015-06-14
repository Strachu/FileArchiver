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
using System.Windows.Forms;

using FileArchiver.Core;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	internal class PerFileErrorDialogLauncher : IPerFileErrorView
	{
		public RetryAction AskForRetryAction(string errorMessage)
		{
			var chosenAction = MessageBox.Show(errorMessage, Lang.ErrorTitle, MessageBoxButtons.AbortRetryIgnore,
			                                   MessageBoxIcon.Error);

			switch(chosenAction)
			{
				case DialogResult.Retry:
					return RetryAction.Retry;

				case DialogResult.Ignore:
					return RetryAction.Ignore;

				case DialogResult.Abort:
					return RetryAction.Abort;

				default:
					throw new InvalidOperationException("Invalid enum value");
			}
		}

		public FileExistsAction AskForFileExistsAction(Path filePath, Path renameCandidate, out bool applyForAll)
		{
			var form = new FileExistsForm();

			form.Show(filePath, renameCandidate);

			applyForAll = form.ApplyToAll;
			return form.ChosenAction;
		}
	}
}
