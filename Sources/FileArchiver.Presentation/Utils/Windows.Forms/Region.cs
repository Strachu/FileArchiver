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

using System.Windows.Forms;

namespace FileArchiver.Presentation.Utils.Windows.Forms
{
	/// <summary>
	/// A wrapper for a panel which allows the panel to be switched at runtime.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Besides the switching of views during runtime this class allows for sharing of references to the view
	/// implemented by the panel in DI container while still retaining support for a Visual Studio Designer
	/// such as setting the size of a panel.
	/// </para>
	/// <para>
	/// It is possible to obtain a full design time support by making a class deriving from a region and
	/// supplying a static panel in its constructor to use when the control is used in design mode.
	/// </para>
	/// </remarks>
	internal class Region : UserControl
	{
		private Control mActivePanel;

		public Control ActivePanel
		{
			get
			{
				return mActivePanel;
			}
			set
			{
				mActivePanel = value;

				mActivePanel.Location = new System.Drawing.Point(0, 0);
				mActivePanel.Dock     = DockStyle.Fill;
				mActivePanel.Margin   = new Padding(0, 0, 0, 0);

				base.Controls.Clear();
				base.Controls.Add(mActivePanel);
			}
		}
	}
}
