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

using FileArchiver.Presentation.Settings;

namespace FileArchiver.Presentation.Shell
{
	public interface IMainView
	{
		event EventHandler                  ViewShown;
		event EventHandler<CancelEventArgs> ViewClosing;

		string Title      { set; }
		string Language   { set; }
		string StatusText { set; }

		/// <summary>
		/// Loads or saves current window layout setting to a specified structure.
		/// </summary>
		/// <value>
		/// The window's current layout settings.
		/// </value>
		WindowLayoutSettings LayoutSettings { get; set; }

		void Close();
	}
}
