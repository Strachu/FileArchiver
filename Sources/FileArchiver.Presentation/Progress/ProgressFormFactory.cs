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
using System.Threading;

using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.Progress
{
	internal class ProgressFormFactory : IProgressViewFactory
	{
		private readonly SynchronizationContext mUIThread;

		public ProgressFormFactory()
		{
			mUIThread = SynchronizationContext.Current;
		}

		public IProgressView ShowProgressForNextOperation(string operationTitle, string operationDescription)
		{
			var form = new ProgressForm(operationTitle, operationDescription);

			InvokeLater(() =>
			{
				// This can happen when some operation ends prematurely without being awaited and the call to
				// progress.Hide is done before the UI thread gets a change to execute this code.
				if(!form.IsDisposed)
				{
					form.ShowDialog();
				}
			});

			return form;
		}

		private void InvokeLater(Action action)
		{
			mUIThread.Post(action);
		}
	}
}
