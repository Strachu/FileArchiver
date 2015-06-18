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
using System.Linq;
using System.Windows.Forms;

using FileArchiver.Presentation.Properties;

namespace FileArchiver.Presentation.Utils.Windows.Forms
{
	/// <summary>
	/// A base class for all forms in the application.
	/// </summary>
	/// The main responsibility of this class is to decorate the form with an application icon and to show it
	/// in the task bar and at the center of the screen if there is no parent window.
	public class FormBase : Form
	{
		private static readonly List<Form> mOpenForms = new List<Form>();

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			mOpenForms.Add(this);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			mOpenForms.Remove(this);
		}

		protected override void CreateHandle()
		{
			// Cannot place this in the constructor as the deriving class will override this in InitializeComponent()
			// neither can this be placed in OnLoad(), as setting the ShowInTaskbar property closes the window.
			if(!mOpenForms.Any())
			{
				base.ShowIcon      = true;
				base.Icon          = Resources.ApplicationIcon;
				base.StartPosition = FormStartPosition.CenterScreen;
				base.ShowInTaskbar = true;
			}

			base.CreateHandle();
		}
	}
}
