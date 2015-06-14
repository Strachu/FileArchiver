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
using System.Linq;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.FileListView
{
	public class FileEntryViewModel : NotifyPropertyChangedHelper
	{
		private readonly FileEntry mWrappedEntry;

		public FileEntryViewModel(FileEntry entry, IFileIconProvider iconProvider)
		{
			Contract.Requires(entry != null);
			Contract.Requires(iconProvider != null);

			mWrappedEntry = entry;

			Icon          = IsDirectory ? iconProvider.GetDirectoryIcon()
			                            : iconProvider.GetIconForFile(mWrappedEntry.Path);
			Name          = entry.Name;

			if(entry.IsDirectory)
			{
				FileCount = entry.EnumerateAllFilesRecursively().Count();
			}
		}

		public Icon Icon
		{
			get;
			private set;
		}

		public FileName Name
		{
			get;
			set;
		}

		public bool IsDirectory
		{
			get { return mWrappedEntry.IsDirectory; }
		}

		public DateTime? LastModificationTime
		{
			get { return mWrappedEntry.LastModificationTime; }
		}

		public long Size
		{
			get { return mWrappedEntry.Size; }
		}

		public int? FileCount
		{
			get;
			private set;
		}

		public bool Modified
		{
			get { return mWrappedEntry.State != FileState.Unchanged; }
		}

		private bool _mSelected = false;
		public bool Selected
		{
			get { return _mSelected; }
			set { base.SetFieldWithNotification(ref _mSelected, value); }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
