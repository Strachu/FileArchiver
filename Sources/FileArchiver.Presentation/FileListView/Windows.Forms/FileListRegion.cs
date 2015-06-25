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
using System.ComponentModel;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Utils.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	/// <summary>
	/// A region with FileListPanel loaded with sample data during design time.
	/// </summary>
	internal class FileListRegion : Presentation.Utils.Windows.Forms.Region
	{
		public FileListRegion()
		{
			if(this.IsInDesignMode())
			{
				base.ActivePanel = new FileListPanel(new DesignTimeFileListViewModel(), new DialogLauncher());
			}
		}

		private class DesignTimeFileListViewModel : IFileListViewModel
		{
			public DesignTimeFileListViewModel()
			{
				var directoryBuilder    = new FileEntry.Builder();
				var modifiedFileBuilder = new FileEntry.Builder();
				var fileBuilder         = new FileEntry.Builder();

				directoryBuilder.WithName(new FileName("Directory"))
					             .AsDirectory()
					             .WithSize(95464)
				                .ModifiedOn(DateTime.Now);
				directoryBuilder.WithNewFile(new FileEntry.Builder().WithName(new FileName("File1")).Build());
				directoryBuilder.WithNewFile(new FileEntry.Builder().WithName(new FileName("File2")).Build());
				directoryBuilder.WithNewFile(new FileEntry.Builder().WithName(new FileName("File3")).Build());
				directoryBuilder.WithNewFile(new FileEntry.Builder().WithName(new FileName("File4")).Build());
				directoryBuilder.WithNewFile(new FileEntry.Builder().WithName(new FileName("File5")).Build());

				modifiedFileBuilder.WithName(new FileName("Modified File"))
					                .WithSize(965)
				                   .AsModified()
				                   .ModifiedOn(DateTime.Now.Subtract(TimeSpan.FromTicks(325647546464)));

				fileBuilder.WithName(new FileName("File"))
				           .WithSize(43545754)
				           .ModifiedOn(DateTime.Now.Subtract(TimeSpan.FromTicks(2343543868634)));

				FilesInCurrentDirectory = new BindingList<FileEntryViewModel>
				{
					new FileEntryViewModel(directoryBuilder.Build(),    new GenericFileIconProvider()),
					new FileEntryViewModel(modifiedFileBuilder.Build(), new GenericFileIconProvider()),
					new FileEntryViewModel(fileBuilder.Build(),         new GenericFileIconProvider())
				};
			}

			public IArchive Archive
			{
				get { return new NullArchive(); }
			}

			public Path ArchivePath
			{
				get { return new Path("C:\\SampleArchive.zip"); }
			}

			public BindingList<FileEntryViewModel> FilesInCurrentDirectory
			{
				get;
				private set;
			}

			public Path CurrentDirectory
			{
				get { return new Path("Directory/Subdirectory"); }
			}

			public Path CurrentDirectoryFullAddress
			{
				get { return new Path("C:\\SampleArchive.zip\\Directory\\Subdirectory"); }
			}

			public bool NavigateToParentDirectoryEnabled
			{
				get { return true; }
			}

			public bool AddFilesEnabled
			{
				get { return true; }
			}
			
			public int FirstDisplayedFileIndex
			{
				get;
				set;
			}

			public void SetArchive(IArchive archive, Path archivePath)
			{
				throw new NotImplementedException();
			}

			public void Open(FileName fileName)
			{
				throw new NotImplementedException();
			}

			public void NavigateToParentDirectory()
			{
				throw new NotImplementedException();
			}

			public void AddFiles(IReadOnlyList<Path> files)
			{
				throw new NotImplementedException();
			}

		#pragma warning disable 67
			public event EventHandler<FileOpenRequestEventArgs> FileOpeningRequested;
			public event PropertyChangedEventHandler            PropertyChanged;
			public event EventHandler<ErrorEventArgs>           ErrorOccured;
		#pragma warning restore 67
		}
	}
}
