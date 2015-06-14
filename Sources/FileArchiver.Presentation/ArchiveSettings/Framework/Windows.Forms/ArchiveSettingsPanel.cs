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
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Forms;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms
{
	internal partial class ArchiveSettingsPanel : UserControl
	{
		// Windows Forms designer doesn't like constructors with parameters
		public ArchiveSettingsPanel()
		{
			InitializeComponent();
		}

		public Expression<Func<IEnumerable<ISettingsControl>>> ControlsBinding
		{
			set
			{
				Contract.Requires(value != null);

				var controlsPropertyReference = PropertyReference.To(value);

				controlsPropertyReference.PropertyChanged += (sender, e) =>
				{
					RecreateControls(controlsPropertyReference.Value);
				};

				RecreateControls(controlsPropertyReference.Value);
			}
		}

		private void RecreateControls(IEnumerable<ISettingsControl> controls)
		{
			try
			{
				mLayoutPanel.SuspendLayout();
				mLayoutPanel.Controls.Clear();

				if(controls == null)
					return;

				int rowCount = 0;
				foreach(var control in controls)
				{
					var windowsFormsControl = control.Control as System.Windows.Forms.Control;
					if(windowsFormsControl == null)
					{
						// This control has no implementation for Windows Forms.
						// Plugin has implemented a custom control in some UI technologies but not this one.
						// Ignore this instead of throwing exception - it is better for an user to just show some settings and let the rest assume default
						// than to not allow him to use the plugin at all.
						// TODO Logging it to a file would be nice
						continue;
					}

					windowsFormsControl.DataBindings.AddFromExpressionIfSpecified("Visible", () => control.Visible, readOnly: true);

					var ordinaryControlWithTitle = control as ISettingsControlWithTitle;
					if(ordinaryControlWithTitle != null)
					{
						var titleControl = CreateTitleLabelFor(ordinaryControlWithTitle);

						mLayoutPanel.Controls.Add(titleControl,        0, rowCount);
						mLayoutPanel.Controls.Add(windowsFormsControl, 1, rowCount);
						rowCount++;
						continue;
					}

					mLayoutPanel.Controls.Add(windowsFormsControl,  0, rowCount);
					mLayoutPanel.SetColumnSpan(windowsFormsControl, 2);
					rowCount++;
				}
			}
			finally
			{
				mLayoutPanel.ResumeLayout(true);
			}
		}

		private Label CreateTitleLabelFor(ISettingsControlWithTitle control)
		{
			var label = new Label
			{
				Text      = control.Title,
				AutoSize  = true,
				Margin    = new Padding(3, 3, 3, 5),
				TextAlign = ContentAlignment.MiddleLeft,
			};

			label.DataBindings.AddFromExpressionIfSpecified("Visible", () => control.Visible, readOnly: true);

			return label;
		}
	}
}
