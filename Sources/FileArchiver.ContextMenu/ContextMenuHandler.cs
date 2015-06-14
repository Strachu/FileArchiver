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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using Liferay.Nativity.Modules.ContextMenu.Model;

using Lang = FileArchiver.ContextMenu.Properties.Resources;

namespace FileArchiver.ContextMenu
{
	/// <summary>
	/// Class responsible for controlling what context menu entries to display for files selected in OS file manager.
	/// </summary>
	internal class ContextMenuHandler
	{
		private const string MAIN_ASSEMBLY_NAME = "FileArchiver.exe";

		private readonly IArchiveLoadingService mLoadingService;

		public ContextMenuHandler(IArchiveLoadingService loadingService)
		{
			Contract.Requires(loadingService != null);

			mLoadingService = loadingService;
		}

		/// <summary>
		/// Gets the list of context menu entries to display for specified files.
		/// </summary>
		/// <param name="selectedFiles">
		/// The files for which the context menu will be shown.
		/// </param>
		/// <returns>
		/// The collection of context menu entries to be displayed in OS file manager.
		/// </returns>
		public IEnumerable<ContextMenuItem> GetContextMenu(IEnumerable<string> selectedFiles)
		{
			Contract.Requires(selectedFiles != null);
			Contract.Requires(Contract.ForAll(selectedFiles, path => !String.IsNullOrWhiteSpace(path)));
			Contract.Ensures(Contract.Result<IEnumerable<ContextMenuItem>>() != null);

			var menuItems = new List<ContextMenuItem>();

			if(selectedFiles.All(IsSupportedArchive))
			{
				menuItems.Add(CreateExtractMenuEntry());
			}

			menuItems.Add(CreatePackMenuEntry());
			return menuItems;
		}

		private bool IsSupportedArchive(string path)
		{
			var fileExtension = Path.GetExtension(path);

			return mLoadingService.SupportedFormats.Any(archive => archive.Extension == fileExtension);
		}

		private ContextMenuItem CreateExtractMenuEntry()
		{
			var menuitem = new ContextMenuItem(Lang.Extract);

			menuitem.Selected += ExtractSelected;
			return menuitem;
		}

		private ContextMenuItem CreatePackMenuEntry()
		{
			var menuitem = new ContextMenuItem(Lang.Pack);

			menuitem.Selected += PackSelected;
			return menuitem;
		}

		private void ExtractSelected(ContextMenuItem sender, IEnumerable<string> selectedArchives)
		{
			var arguments = String.Format("--extract {0}", ConvertFileListToArgumentString(selectedArchives));

			Process.Start(MAIN_ASSEMBLY_NAME, arguments);
		}

		private void PackSelected(ContextMenuItem sender, IEnumerable<string> paths)
		{
			var arguments = String.Format("--pack {0}", ConvertFileListToArgumentString(paths));

			Process.Start(MAIN_ASSEMBLY_NAME, arguments);
		}

		private static string ConvertFileListToArgumentString(IEnumerable<string> files)
		{
			var builder = new StringBuilder(capacity: 100);

			foreach(var file in files)
			{
				builder.Append("\"");
				builder.Append(file);
				builder.Append("\"");
				builder.Append(" ");
			}
			builder.Remove(builder.Length - 1, 1);

			return builder.ToString();
		}
	}
}
