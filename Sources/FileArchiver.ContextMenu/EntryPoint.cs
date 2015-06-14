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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Configuration;
using System.Reflection;
using FileArchiver.Presentation;
using Liferay.Nativity.Control;
using Liferay.Nativity.Modules.ContextMenu;

namespace FileArchiver.ContextMenu
{
	internal class EntryPoint
	{
		[STAThread]
		private static void Main()
		{
			var catalogs = new List<ComposablePartCatalog>()
			{
				ApplicationBootstraper.CreateApplicationCatalog(),
				CreateCatalogForThisAssembly(),
				ApplicationBootstraper.CreatePluginsCatalog()
			};

			using(var container = ApplicationBootstraper.InitializeContainer(catalogs))
			{
				ApplicationBootstraper.ApplyLanguageFromSettings();

				var contextMenuHandler = container.GetExportedValue<ContextMenuHandler>();

				NativityControlUtil.NativityControl.Connect();
				NativityControlUtil.NativityControl.SetFilterFolders(String.Empty);

				ContextMenuControlUtil.GetContextMenuControl(NativityControlUtil.NativityControl, contextMenuHandler.GetContextMenu);
			}
		}

		private static ComposablePartCatalog CreateCatalogForThisAssembly()
		{
			var exports = new RegistrationBuilder();

			exports.ForType(typeof(ContextMenuHandler)).Export();

			return new AssemblyCatalog(Assembly.GetExecutingAssembly(), exports);
		}
	}
}
