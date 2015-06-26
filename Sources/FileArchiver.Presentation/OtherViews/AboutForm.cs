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
using System.Diagnostics;
using System.Reflection;

using System.Windows.Forms;

namespace FileArchiver.Presentation.OtherViews
{
	internal partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();

			var appName    = Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
			var appVersion = Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;

			mAppInfoLabel.Text = String.Format(mAppInfoLabel.Text, appName.Product, appVersion.Version);

			ParseCreditsLinks();
		}

		/// <summary>
		/// Parses links in the form of &lta href="url"&gttext&lt/a&gt in the credits label.
		/// </summary>
		private void ParseCreditsLinks()
		{
			var creditsText = mCreditsLabel.Text;
			var linkStart   = 0;
			while((linkStart = creditsText.IndexOf("<a href=", StringComparison.InvariantCultureIgnoreCase)) != -1)
			{
				var link = new LinkLabel.Link
				{
					Start = linkStart
				};

				var urlStart  = link.Start + 9;
				var urlEnd    = creditsText.IndexOf("\">", urlStart, StringComparison.InvariantCultureIgnoreCase);
				link.LinkData = creditsText.Substring(urlStart, urlEnd - urlStart);

				creditsText  = creditsText.Remove(link.Start, urlEnd + 2 - link.Start);

				var linkEnd = creditsText.IndexOf("</a>", link.Start, StringComparison.InvariantCultureIgnoreCase);
				if(linkEnd == -1)
					continue;

				link.Length = linkEnd - link.Start;	

				creditsText = creditsText.Remove(linkEnd, 4);

				mCreditsLabel.Links.Add(link);	
			}

			mCreditsLabel.Text = creditsText;
		}

		private void LinkInCredits_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start((string)e.Link.LinkData);

			e.Link.Visited = true;
		}

		private void ApplicationWebsiteLink_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Strachu/FileArchiver");

			e.Link.Visited = true;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}
