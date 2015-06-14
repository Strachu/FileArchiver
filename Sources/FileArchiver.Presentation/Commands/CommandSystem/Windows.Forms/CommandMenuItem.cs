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
using System.Windows.Forms;

using FileArchiver.Core.Utils;

namespace FileArchiver.Presentation.Commands.CommandSystem.Windows.Forms
{
	/// <summary>
	/// A MenuItem which uses object of type ICommand as the action implementation.
	/// </summary>
	internal class CommandMenuItem : MenuItem
	{
		private ICommand mCommand = new NullCommand();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ICommand Command
		{
			get
			{
				return mCommand;
			}
			set
			{
				mCommand.PropertyChanged -= CommandPropertyChanged;
				mCommand = value;
				mCommand.PropertyChanged += CommandPropertyChanged;

				base.Enabled = mCommand.Enabled;
			}
		}

		protected override async void OnClick(EventArgs e)
		{
			await Command.ExecuteAsync();

			base.OnClick(e);
		}

		private void CommandPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == PropertyName.Of(() => mCommand.Enabled))
			{
				base.Enabled = mCommand.Enabled;
			}
		}
	}
}
