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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal class GroupBox : NotifyPropertyChangedHelper, ISettingsControl
	{
		private IEnumerable<ISettingsControl> mCurrentGroupControls;
		private PropertyReference<bool>       mVisibleProperty;

		public GroupBox(string title, Expression<Func<IEnumerable<ISettingsControl>>> controlsProperty,
		                              Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(controlsProperty != null);

			var alwaysTrueVariable = true;
			visibleProperty        = visibleProperty ?? (() => alwaysTrueVariable);

			var settingsPanel = new ArchiveSettingsPanel()
			{
				Dock            = DockStyle.Fill,
				ControlsBinding = controlsProperty
			};

			var groupBox = new System.Windows.Forms.GroupBox()
			{
				Text         = title,
				AutoSize     = true,
				Dock         = DockStyle.Fill,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
			};

			groupBox.Controls.Add(settingsPanel);

			Control = groupBox;

			ManageVisibility(controlsProperty, visibleProperty);
		}

		private void ManageVisibility(Expression<Func<IEnumerable<ISettingsControl>>> controlsProperty,
		                              Expression<Func<bool>> visibleProperty)
		{
			var controlsPropertyReference = PropertyReference.To(controlsProperty);
			controlsPropertyReference.PropertyChanged += (sender, e) =>
			{
				OnControlsInGroupListChanged(controlsPropertyReference.Value);
			};

			mVisibleProperty = PropertyReference.To(visibleProperty);
			mVisibleProperty.PropertyChanged += (sender, e) =>
			{
				UpdateVisiblePropertyValue();
			};

			OnControlsInGroupListChanged(controlsPropertyReference.Value);
		}

		private void OnControlsInGroupListChanged(IEnumerable<ISettingsControl> newControlList)
		{
			ForEachPropertyChangeNotifingControl(p => p.PropertyChanged -= ChildControlVisibilityChanged);

			mCurrentGroupControls = newControlList;

			ForEachPropertyChangeNotifingControl(p => p.PropertyChanged += ChildControlVisibilityChanged);

			UpdateVisiblePropertyValue();
		}

		private void ForEachPropertyChangeNotifingControl(Action<INotifyPropertyChanged> action)
		{
			if(mCurrentGroupControls == null)
				return;

			foreach(var control in mCurrentGroupControls)
			{
				var propertyNotifier = control as INotifyPropertyChanged;
				if(propertyNotifier == null)
					continue;

				action(propertyNotifier);
			}
		}

		private void ChildControlVisibilityChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateVisiblePropertyValue();
		}

		private void UpdateVisiblePropertyValue()
		{
			bool anyControlVisible = mCurrentGroupControls != null && mCurrentGroupControls.Any(control => control.Visible);

			SetPropertyWithNotification(() => Visible, mVisibleProperty.Value && anyControlVisible);
		}

		public object Control
		{
			get;
			private set;
		}

		public bool Visible
		{
			get;
			private set;
		}
	}
}
