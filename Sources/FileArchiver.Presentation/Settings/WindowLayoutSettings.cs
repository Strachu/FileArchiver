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
using System.Drawing;

namespace FileArchiver.Presentation.Settings
{
	[Serializable]
	public class WindowLayoutSettings
	{
		public Point WindowLocation { get; set; }

		public Size WindowSize { get; set; }

		public bool Maximized { get; set; }

		public FileListPanelLayoutSettings FileListPanelSettings { get; set; }
	}

	[Serializable]
	public class FileListPanelLayoutSettings
	{
		public int FileColumnWidth { get; set; }

		public int SizeColumnWidth { get; set; }

		public int ModificationDateColumnWidth { get; set; }

		public int FilesInDirectoryCountWidth { get; set; }

		public int FileColumnDisplayIndex { get; set; }

		public int SizeColumnDisplayIndex { get; set; }

		public int ModificationDateColumnDisplayIndex { get; set; }

		public int FilesInDirectoryCountDisplayIndex { get; set; }		
	}
}
