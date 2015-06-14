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

using System.Drawing;
using FileArchiver.Presentation.Properties;

namespace FileArchiver.Presentation.FileListView
{
	/// <summary>
	/// A very simple file icon provider which just loads different icon for a directory and file
	/// from application's resources.
	/// </summary>
	internal class GenericFileIconProvider : IFileIconProvider
	{
		private readonly Icon mDirectoryIcon;
		private readonly Icon mGenericFileIcon;

		public GenericFileIconProvider()
		{
			mDirectoryIcon   = Icon.FromHandle(Resources.Icon_Directory.GetHicon());
			mGenericFileIcon = Icon.FromHandle(Resources.Icon_File.GetHicon());
		}

		public Icon GetDirectoryIcon()
		{
			return mDirectoryIcon;
		}

		public Icon GetIconForFile(string fileName)
		{
			return mGenericFileIcon;
		}
	}
}
