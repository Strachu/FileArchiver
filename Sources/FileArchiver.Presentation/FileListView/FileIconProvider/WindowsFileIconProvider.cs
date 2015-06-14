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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace FileArchiver.Presentation.FileListView
{
	/// <summary>
	/// An <see cref="IFileIconProvider"/> using P/Invoke to get an icon depending on file extension.
	/// </summary>
	/// <remarks>
	/// It's calling into WinAPI, so it's Windows specific.
	/// </remarks>
	internal class WindowsFileIconProvider : IFileIconProvider
	{
		private readonly Icon                      mDirectoryIcon = PInvokeIconRetriever.GetDirectoryIcon();
		private readonly IDictionary<string, Icon> mFileIconPool  = new Dictionary<string, Icon>(); 

		public Icon GetDirectoryIcon()
		{
			return mDirectoryIcon;
		}

		public Icon GetIconForFile(string fileName)
		{
			Icon icon = null;

			// Pooling of icons is critical otherwise after a few calls (exactly 3297 in my case) the ShGetFileIcon()
			// method will return null icon handle and the GUI will keep crashing with OutOfMemoryException.
			// Theoretically the icons should be released with a call to DestroyIcon(), but even if we keep releasing
			// unused icons the application will crash when opening a directory with more than 3297 files because all
			// these icons will be in use (unless we improve FileListViewModel to keep in memory only currently displayed
			// files).
			// With pooling we offset the limit to a directory with more than 3297 file extensions which is very unlikely.

			var fileExtension = Path.GetExtension(fileName);
			if(!mFileIconPool.TryGetValue(fileExtension, out icon))
			{
				icon = PInvokeIconRetriever.GetIconForFile(fileName);

				mFileIconPool[fileExtension] = icon;
			}

			return icon;
		}

		private static class PInvokeIconRetriever
		{
			private const Int32  MAX_PATH                 = 0x00000104;

			private const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x10;
			private const UInt32 FILE_ATTRIBUTE_NORMAL    = 0x80;

			private const UInt32 SHGFI_ICON               = 0x000000100;
			private const UInt32 SHGFI_SMALLICON          = 0x000000001;
			private const UInt32 SHGFI_USEFILEATTRIBUTES  = 0x000000010;

			private readonly static UIntPtr ERROR         = new UIntPtr(0);

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			private struct ShFileInfo
			{
				public IntPtr hIcon;
				public Int32  iIcon;
				public UInt32 dwAttributes;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
				public string szDisplayName;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
				public string szTypeName;
			}

			[DllImport("Shell32.dll", CharSet = CharSet.Auto)]
			private static extern UIntPtr SHGetFileInfo(string pszPath,
																	  UInt32 dwFileAttributes,
																	  ref    ShFileInfo psfi,
																	  UInt32 cbFileInfo,
																	  UInt32 uFlags);

			public static Icon GetDirectoryIcon()
			{
				return GetIcon("Directory", FILE_ATTRIBUTE_DIRECTORY);
			}

			public static Icon GetIconForFile(string fileName)
			{
				return GetIcon(fileName, FILE_ATTRIBUTE_NORMAL);
			}

			private static Icon GetIcon(string path, UInt32 fileAttributes)
			{
				var returnedInfo = new ShFileInfo();

				var returnCode = SHGetFileInfo(path, fileAttributes, ref returnedInfo, (uint)Marshal.SizeOf(returnedInfo),
														 SHGFI_USEFILEATTRIBUTES | SHGFI_ICON | SHGFI_SMALLICON);

				return (returnCode != ERROR) ? Icon.FromHandle(returnedInfo.hIcon) : SystemIcons.Error;
			}			
		}
	}
}
