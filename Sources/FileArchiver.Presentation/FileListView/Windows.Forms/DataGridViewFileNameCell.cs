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
using System.Windows.Forms;

namespace FileArchiver.Presentation.FileListView.Windows.Forms
{
	/// <summary>
	/// Custom cell type for DataGridView displaying an icon before file name.
	/// </summary>
	internal class DataGridViewFileNameWithIconCell : DataGridViewTextBoxCell
	{
		protected override void Paint(Graphics graphics,
		                              Rectangle clipBounds,
		                              Rectangle cellBounds,
		                              int rowIndex,
		                              DataGridViewElementStates cellState,
		                              object value,
		                              object formattedValue,
		                              string errorText,
		                              DataGridViewCellStyle cellStyle,
		                              DataGridViewAdvancedBorderStyle advancedBorderStyle,
		                              DataGridViewPaintParts paintParts)
		{
			DrawBackground(graphics, clipBounds, cellBounds, DataGridView.Rows[rowIndex].Selected, cellStyle, advancedBorderStyle);

			var fileEntry = (FileEntryViewModel)DataGridView.Rows[rowIndex].DataBoundItem;

			var iconBounds    = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Height - 1, cellBounds.Height - 1);
			var textBoxBounds = new Rectangle(cellBounds.X + iconBounds.Width,
			                                  cellBounds.Y,
			                                  cellBounds.Width - iconBounds.Width,
			                                  cellBounds.Height);

			DrawIcon(graphics, iconBounds, fileEntry.Icon);

			base.Paint(graphics, clipBounds, textBoxBounds, rowIndex, cellState, value, formattedValue,
			           errorText, cellStyle, advancedBorderStyle, paintParts);
		}

		private void DrawBackground(Graphics graphics,
		                            Rectangle clipBounds,
		                            Rectangle cellBounds,
		                            bool selected,
		                            DataGridViewCellStyle cellStyle,
		                            DataGridViewAdvancedBorderStyle advancedBorderStyle)
		{
			using(var brush = new SolidBrush(selected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
			{
				graphics.FillRectangle(brush, cellBounds);
			}

			base.PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
		}

		private void DrawIcon(Graphics graphics, Rectangle imageBounds, Icon icon)
		{
			var centeredX = imageBounds.X + (imageBounds.Width  - icon.Width)  / 2;
			var centeredY = imageBounds.Y + (imageBounds.Height - icon.Height) / 2;

			graphics.DrawIcon(icon, centeredX, centeredY);
		}

		public override void PositionEditingControl(bool setLocation,
		                                            bool setSize,
		                                            Rectangle cellBounds,
		                                            Rectangle cellClip,
		                                            DataGridViewCellStyle cellStyle,
		                                            bool singleVerticalBorderAdded,
		                                            bool singleHorizontalBorderAdded,
		                                            bool isFirstDisplayedColumn,
		                                            bool isFirstDisplayedRow)
		{
			base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded,
			                            singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);

			var fileEntry = (FileEntryViewModel)base.DataGridView.Rows[base.RowIndex].DataBoundItem;

			var iconLabel = CreateLabelWithFileIcon(fileEntry.Icon, new Size(cellBounds.Height - 1, cellBounds.Height - 1));
			var textBox   = base.DataGridView.EditingControl;

			textBox.Left  += iconLabel.Width;
			textBox.Width -= iconLabel.Width;

			base.DataGridView.EditingPanel.Controls.Clear();
			base.DataGridView.EditingPanel.Controls.Add(iconLabel);
			base.DataGridView.EditingPanel.Controls.Add(textBox);
		}

		private Label CreateLabelWithFileIcon(Icon icon, Size labelSize)
		{
			return new Label
			{
				Image = icon.ToBitmap(),
				Size  = labelSize
			};
		}
	}
}
