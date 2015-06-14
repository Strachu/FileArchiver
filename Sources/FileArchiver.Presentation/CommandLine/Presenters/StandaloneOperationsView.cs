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
using System.IO;
using System.Windows.Forms;

using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Properties;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.CommandLine.Presenters
{
	/// <summary>
	/// An implementation of views for archive operations invoked when the main window has not been created.
	/// </summary>
	/// <remarks>
	/// The main purpose of this class is to add elements to windows which aren't normally shown
	/// during operation from GUI, for example a button for the window in the task bar,
	/// application icon, center the window on desktop etc.
	/// </remarks>
	internal class StandaloneOperationsView : IEntireArchivesExtractionView, IPackIntoArchiveView
	{
		IProgressView IEntireArchivesExtractionView.ShowProgressForNextExtraction(string archivePath)
		{
			var operationDescription = String.Format(Lang.ExtractEntireArchiveProgressForm_Description, Path.GetFileName(archivePath));

			return ShowStandaloneProgressForm(Lang.ExtractProgressForm_Title, operationDescription);
		}

		IProgressView IPackIntoArchiveView.ShowProgress(string archivePath)
		{
			var operationDescription = String.Format(Lang.PackingProgressForm_Description, Path.GetFileName(archivePath));

			return ShowStandaloneProgressForm(Lang.PackingProgressForm_Title, operationDescription);
		}

		private IProgressView ShowStandaloneProgressForm(string title, string description)
		{
			var form = new ProgressForm(title, description);

			form.ShowInTaskbar = true;
			form.ShowIcon      = true;
			form.Icon          = Resources.ApplicationIcon;
			form.StartPosition = FormStartPosition.CenterScreen;

			form.Show();

			return form;
		}

		public void DisplayError(string message)
		{
			MessageBox.Show(message, Lang.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		void IPackIntoArchiveView.DisplayError(string message)
		{
			DisplayError(message);
		}
	}
}
