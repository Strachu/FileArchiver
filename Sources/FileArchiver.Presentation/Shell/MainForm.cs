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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using FileArchiver.Core.Utils;

using FileArchiver.Presentation.Commands;
using FileArchiver.Presentation.FileListView.Windows.Forms;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Utils.Windows.Forms;

namespace FileArchiver.Presentation.Shell
{
	internal partial class MainForm : FormBase, IMainView
	{
		private readonly FileListPanel mFileListPanel = DependencyInjectionContainer.Instance.Get<FileListPanel>();

		public MainForm()
		{
			InitializeComponent();

			base.Shown       += (sender, e) => ViewShown.SafeRaise(this, e);
			base.FormClosing += (sender, e) => ViewClosing.SafeRaise(this, e);
		}

		protected override void OnLoad(EventArgs e)
		{
			mFileListRegion.ActivePanel = mFileListPanel;

			// TODO I think it's good idea to make the entries be created automatically at runtime just from ICommand implementations.
			//      It would also give the plugins possibility to add their own entries to a menu.
			//      In this case it will a new fields such as EntryName and Shortcut will be need to be added to ICommand
			//      (or alternatively an attribute can be created for it). 
			// The only problem would be how to determine the order of entries, especially if adding top level menus would also be allowed.

			mNewArchiveMenuEntry.Command      = DependencyInjectionContainer.Instance.Get<NewArchiveCommand>();
			mOpenArchiveMenuEntry.Command     = DependencyInjectionContainer.Instance.Get<OpenArchiveCommand>();
			mSaveArchiveMenuEntry.Command     = DependencyInjectionContainer.Instance.Get<SaveArchiveCommand>();

			mEnglishLanguageMenuEntry.Command = new SetLanguageCommand(this, "en-US");
			mPolishLanguageMenuEntry.Command  = new SetLanguageCommand(this, "pl-PL");

			mNewToolbarButton.Command         = DependencyInjectionContainer.Instance.Get<NewArchiveCommand>(); 
			mOpenToolbarButton.Command        = DependencyInjectionContainer.Instance.Get<OpenArchiveCommand>();
			mSaveToolbarButton.Command        = DependencyInjectionContainer.Instance.Get<SaveArchiveCommand>();
			mAddToolbarButton.Command         = DependencyInjectionContainer.Instance.Get<AddFilesCommand>();
			mExtractToolbarButton.Command     = DependencyInjectionContainer.Instance.Get<ExtractFilesCommand>();
			mRemoveToolbarButton.Command      = DependencyInjectionContainer.Instance.Get<RemoveFilesCommand>();

			base.OnLoad(e);
		}

		public string Title
		{
			set { base.Text = value; }
		}

		public string StatusText
		{
			set { mStatusBarLabel.Text = value; }
		}

		public string Language
		{
			set
			{
				foreach(MenuItem languageMenu in mLanguageMenu.MenuItems)
				{
					languageMenu.Checked = false;
				}

				var chosenLanguageMenuEntry = mLanguageMenu.MenuItems.Cast<MenuItem>()
																		 .SingleOrDefault(menu => menu.Tag as string == value) ?? mEnglishLanguageMenuEntry;
				chosenLanguageMenuEntry.Checked = true;

				this.RefreshResources();
			}
		}

		public WindowLayoutSettings LayoutSettings
		{
			get
			{
				return new WindowLayoutSettings
				{
					WindowLocation        = (base.WindowState == FormWindowState.Normal) ? base.Location   : base.RestoreBounds.Location,
					WindowSize            = (base.WindowState == FormWindowState.Normal) ? base.ClientSize : base.RestoreBounds.Size - base.SizeFromClientSize(new Size()),
					Maximized             = (base.WindowState == FormWindowState.Maximized),

					FileListPanelSettings = mFileListPanel.LayoutSettings
				};
			}

			set
			{
				base.StartPosition            = FormStartPosition.Manual;
				base.Location                 = value.WindowLocation;
				base.ClientSize               = value.WindowSize;
				base.WindowState              = value.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;

				mFileListPanel.LayoutSettings = value.FileListPanelSettings;
			}
		}

		private void SelectAll_Click(object sender, EventArgs e)
		{
			mFileListPanel.SelectAll();
		}

		private void About_Click(object sender, EventArgs e)
		{
			var aboutDialog = new AboutForm();
			aboutDialog.ShowDialog();
		}

		public event EventHandler                  ViewShown;
		public event EventHandler<CancelEventArgs> ViewClosing;
	}
}
