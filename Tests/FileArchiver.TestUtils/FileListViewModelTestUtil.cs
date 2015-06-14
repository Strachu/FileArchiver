using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

using FileArchiver.Presentation.FileListView;

using NUnit.Framework;

namespace FileArchiver.TestUtils
{
	internal static class FileListViewModelTestUtil
	{
		public static void AssertFileListIsSetTo(IFileListViewModel testedModel, params string[] fileNames)
		{
			foreach(var name in fileNames)
			{
				var fileName     = new FileName(name);
				var propertyName = PropertyName.Of<FileEntryViewModel>(x => x.Name);

				Assert.That(testedModel.FilesInCurrentDirectory, Has.Some.Property(propertyName).EqualTo(fileName),
				            "The file \"" + fileName + "\" is missing.");
			}

			Assert.That(testedModel.FilesInCurrentDirectory, Has.Count.EqualTo(fileNames.Length),
			            "The directory has too many files");
		}
		
		public static void AssertOnlyFollowingFilesAreSelected(IFileListViewModel testedModel, params string[] fileNames)
		{
			var selectedFileNames = testedModel.FilesInCurrentDirectory.Where(file => file.Selected).Select(x => x.Name);

			Assert.That(selectedFileNames, Is.EquivalentTo(fileNames.Select(x => new FileName(x))));
		}
	}
}
