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
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Forms;

using FileArchiver.Presentation.Utils.Windows.Forms;

using Lang = FileArchiver.Presentation.Properties.Resources;

namespace FileArchiver.Presentation.Progress
{
	/// <summary>
	/// A form displaying the progress of an arbitrary operation and giving the user ability to cancel it.
	/// </summary>
	internal partial class ProgressForm : FormBase, IProgressView
	{
		private CancellationTokenSource mCancelTokenSource;

		private readonly string         mOperationTitle;
		private bool                    mClosingProgrammatically;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressForm"/> class.
		/// </summary>
		/// <param name="operationTitle">
		/// The operation title.
		/// </param>
		/// <param name="operationDescription">
		/// The operation description.
		/// </param>
		public ProgressForm(string operationTitle, string operationDescription)
		{
			Contract.Requires(!String.IsNullOrEmpty(operationTitle));
			Contract.Requires(!String.IsNullOrEmpty(operationDescription));

			InitializeComponent();

			mCancelTokenSource        = new CancellationTokenSource();
			Progress                  = new Progress<double?>(ProgressChanged);

			var progressBarPadding    = base.Width - mProgressBar.Width;

			mOperationTitle           = operationTitle;
			mLabel.Text               = operationDescription;
			mClosingProgrammatically  = false;

			base.Width                = progressBarPadding + mLabel.Width;

			CenterDescriptionLabel();

			ProgressChanged(0.0);
		}

		private void CenterDescriptionLabel()
		{
			mLabel.Left = (base.Width - mLabel.Width) / 2;
		}

		public IProgress<double?> Progress
		{
			get;
			private set;
		}

		public CancellationToken CancelToken
		{
			get { return mCancelTokenSource.Token; }
		}

		public new void Hide()
		{
			mClosingProgrammatically = true;

			base.Close();
		}

		private void ProgressChanged(double? progress)
		{
			if(progress != null)
			{
				base.Text = String.Format("{0}: {1:P0}", mOperationTitle, progress);

				mProgressBar.Style = ProgressBarStyle.Continuous;
				mProgressBar.Value = (int)(progress.Value * mProgressBar.Maximum);
			}
			else
			{
				base.Text = mOperationTitle;

				mProgressBar.Style = ProgressBarStyle.Marquee;
			}
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			Cancel();
		}

		private void Close_Click(object sender, FormClosingEventArgs e)
		{
			if(!mClosingProgrammatically)
			{
				e.Cancel = true;

				Cancel();
			}
		}

		private void Cancel()
		{
			mCancelTokenSource.Cancel();

			mLabel.Text           = Lang.Cancelling;
			mCancelButton.Enabled = false;
		}

		protected override void Dispose(bool disposeManagedResources)
		{
			if(disposeManagedResources)
			{
				if(components != null)
				{
					components.Dispose();
					components = null;
				}

				if(mCancelTokenSource != null)
				{
					mCancelTokenSource.Dispose();
					mCancelTokenSource = null;
				}
			}
			base.Dispose(disposeManagedResources);
		}
	}
}
