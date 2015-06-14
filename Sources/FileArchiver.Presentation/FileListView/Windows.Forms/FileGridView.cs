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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	/// <summary>
	/// A DataGridView with some changes in navigation behavior making it more suitable as a grid with files.
	/// </summary>
	internal class FileGridView : DataGridView
	{
		protected override bool ProcessDataGridViewKey(KeyEventArgs e)
		{
			// Disable moving to next entry with "Enter" key
			if(e.KeyCode == Keys.Enter)
				return true;

			// Disable exiting edit mode with "Down" and "Up" arrow
			if(base.IsCurrentCellInEditMode && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up))
				return true;

			return base.ProcessDataGridViewKey(e);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			// Disable changing current cell to the next one after committing edit.
			// The cell is changed somewhere between raising CellEndEdit event and exiting of ProcessEnterKey.
			// There is also EndEdit() which doesn't change the current cell but doesn't invoke CellValidating event which cannot be invoked manually.
			if(keyData == Keys.Enter)
			{
				// Calls OnCellEndEdit()
				base.ProcessEnterKey(keyData);

				// Position cannot be restored when the editing has not ended because of validation errors.
				if(!base.IsCurrentCellInEditMode)
				{
					RestoreLastDisplayPosition();
				}
				return true;
			}

			return base.ProcessDialogKey(keyData);
		}

		protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
		{
			base.OnCellEndEdit(e);

			SaveCurrentDisplayPosition();
		}

		private int     mDisplayIndexAfterOnCellEndEdit;
		private Point   mCurrentCellPositionAfterOnCellEndEdit;
		private Point[] mSelectedCellsIndices;

		private void SaveCurrentDisplayPosition()
		{
			mDisplayIndexAfterOnCellEndEdit        = base.FirstDisplayedScrollingRowIndex;
			mCurrentCellPositionAfterOnCellEndEdit = base.CurrentCellAddress;

			mSelectedCellsIndices                  = base.SelectedCells.Cast<DataGridViewCell>().Select(cell => new Point(cell.ColumnIndex, cell.RowIndex)).ToArray();
		}

		private void RestoreLastDisplayPosition()
		{
			base.CurrentCell                     = base[mCurrentCellPositionAfterOnCellEndEdit.X, mCurrentCellPositionAfterOnCellEndEdit.Y];
			base.FirstDisplayedScrollingRowIndex = mDisplayIndexAfterOnCellEndEdit;

			base.ClearSelection();

			foreach(var selectedCell in mSelectedCellsIndices)
			{
				base[selectedCell.X, selectedCell.Y].Selected = true;
			}
		}
	}
}
