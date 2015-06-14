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

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace FileArchiver.Presentation.Utils.Windows.Forms
{
	internal static class ControlExtensions
	{
		/// <summary>
		/// Gets a value indicating whether this control is executed in visual studio designer.
		/// </summary>
		/// <remarks>
		/// http://stackoverflow.com/a/2693338/2579010
		/// </remarks>
		public static bool IsInDesignMode(this Control control)
		{
			Contract.Requires(control != null);

			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				return true;

			while(control != null)
			{
				if(control.Site != null && control.Site.DesignMode)
					return true;

				control = control.Parent;
			}

			return false;
		}

		/// <summary>
		/// Refreshes all resources for a control.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It is the most useful in the case of a language switching during a runtime without application restart.
		/// </para>
		/// <para>
		/// <b>It is required that every control for which the resources should be refreshed is a field of the control but it does not have to be public</b>
		/// </para>
		/// </remarks>
		public static void RefreshResources(this Control form)
		{
			Contract.Requires(form != null);

			try
			{
				var resourceManager = new ComponentResourceManager(form.GetType());

				resourceManager.ApplyResources(form, "$this");

				var fields = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public |
				                                      BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				foreach(var field in fields)
				{
					var obj = field.GetValue(form);
					if(obj != null)
					{
						var control = obj as Control;
						if(control != null)
						{
							control.RefreshResources();
						}

						resourceManager.ApplyResources(obj, field.Name);
					}
				}
			}
			catch(MissingManifestResourceException)
			{
				// It is thrown when ApplyResources is executed for a control which does not have resources,
				// such as built-in TextBox.
			}
		}
	}
}
