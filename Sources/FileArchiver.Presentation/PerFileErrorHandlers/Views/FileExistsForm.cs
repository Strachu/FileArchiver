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
using System.Diagnostics.Contracts;
using System.Drawing;

using FileArchiver.Presentation.Utils.Windows.Forms;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	/// <summary>
	/// A form displayed to the user when the collision of file names occurs
	/// giving the user possibility to choose an action.
	/// </summary>
	internal partial class FileExistsForm : FormBase
	{
		private readonly ComponentResourceManager mLang = new ComponentResourceManager(typeof(FileExistsForm));

		public FileExistsForm()
		{
			InitializeComponent();

			mErrorIconLabel.Image = new Bitmap(SystemIcons.Warning.ToBitmap(), mErrorIconLabel.Width, mErrorIconLabel.Height);
		}

		public void Show(Path filePath, Path renameCandidate)
		{
			Contract.Requires(filePath != null);
			Contract.Requires(renameCandidate != null);

			ChosenAction = FileExistsAction.Abort;

			// TODO create some helper for getting strings from form's resources using ExpressionTree?
			// - Note that you also need to retrieve variable's name - the entire string
			mMessageLabel.Text = String.Format(mLang.GetString("mMessageLabel.Text"), filePath, renameCandidate.FileName);

			base.ShowDialog();
		}

		public FileExistsAction ChosenAction
		{
			get;
			private set;
		}

		/// <summary>
		/// Indicates whether the user wants to apply chosen action for all future conflicts.
		/// </summary>
		public bool ApplyToAll
		{
			get { return mApplyToAllCheckBox.Checked; }
		}

		private void OverwriteButton_Click(object sender, EventArgs e)
		{
			ChooseAction(FileExistsAction.Overwrite);
		}

		private void RenameButton_Click(object sender, EventArgs e)
		{
			ChooseAction(FileExistsAction.Rename);
		}

		private void SkipButton_Click(object sender, EventArgs e)
		{
			ChooseAction(FileExistsAction.Skip);
		}

		private void AbortButton_Click(object sender, EventArgs e)
		{
			ChooseAction(FileExistsAction.Abort);
		}

		private void ChooseAction(FileExistsAction action)
		{
			ChosenAction = action;

			base.Close();
		}
	}
}
