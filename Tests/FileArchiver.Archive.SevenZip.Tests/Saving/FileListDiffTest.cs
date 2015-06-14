using System;
using System.Linq;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

using NUnit.Framework;

namespace FileArchiver.Archive.SevenZip.Tests.Saving
{
	internal class FileListDiffTest
	{
		[Test]
		public void WhenFilesWereAddedToEmptyList_TheyAreInAddedFilesList()
		{
			var originalFileList = new FileEntry[0];
			var currentFileList  = new FileEntry[]
			{
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile")).Build(),
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile2")).Build()
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.AddedFiles,    Is.EquivalentTo(currentFileList));
			Assert.That(diff.ModifiedFiles, Is.Empty);
			Assert.That(diff.RemovedFiles,  Is.Empty);
		}

		[Test]
		public void WhenFilesWereAddedToListWithSomeEntries_TheExistingFilesAreNotInAddedFilesList()
		{
			var filesToAdd = new FileEntry[]
			{
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile")).Build(),
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile2")).Build()
			};

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("OldFile")).Build(),
				new FileEntry.Builder().WithName(new FileName("OldFile2")).Build()
			};

			var currentFileList = originalFileList.Concat(filesToAdd);

			var diff = FileListDiff.Compare(originalFileList, currentFileList.ToList());

			Assert.That(diff.AddedFiles, Is.EquivalentTo(filesToAdd));
		}

		[Test]
		public void WhenFilesWereAddedToSubDirectory_TheyAreInAddedFilesList()
		{
			var filesToAdd = new FileEntry[]
			{
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile")).Build(),
				new FileEntry.Builder().AsNew().WithName(new FileName("NewFile2")).Build()
			};

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().AsDirectory().WithName(new FileName("OldDirectory")).Build(),
				new FileEntry.Builder().AsDirectory().WithName(new FileName("OldDirectory2")).Build()
			};
			
			var currentFileList = new FileEntry[]
			{
				originalFileList[0].BuildCopy().WithNewFile(filesToAdd[0]).Build(),
				originalFileList[1].BuildCopy().WithNewFile(filesToAdd[1]).Build()
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.AddedFiles, Is.EquivalentTo(filesToAdd));
		}

		[Test]
		public void WhenDirectoryWithFilesWasAdded_OnlyTheDirectoryIsInAddedFilesList()
		{
			var fileInNewDirectory  = new FileEntry.Builder().AsNew().WithName(new FileName("FileInNewDirectory")).Build();
			var fileInNewDirectory2 = new FileEntry.Builder().AsNew().WithName(new FileName("FileInNewDirectory2")).Build();
			var directory           = new FileEntry.Builder().AsNew()
				                                              .AsDirectory()
				                                              .WithName(new FileName("NewDirectory"))
				                                              .WithFiles(fileInNewDirectory, fileInNewDirectory2)
				                                              .Build();

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("OldFile")).Build(),
				new FileEntry.Builder().AsDirectory().WithName(new FileName("OldDirectory")).Build()
			};
			
			var currentFileList = originalFileList.Concat(directory.ToSingleElementList());

			var diff = FileListDiff.Compare(originalFileList, currentFileList.ToList());

			Assert.That(diff.AddedFiles, Is.EquivalentTo(directory.ToSingleElementList()));
		}

		[Test]
		public void WhenFilesWereRemoved_TheyAreInRemovedFilesList()
		{
			var filesToRemove = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("FileToRemove")).Build(),
				new FileEntry.Builder().WithName(new FileName("FileToRemove2")).Build()
			};

			var originalFileList = filesToRemove.Concat(new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("OldFile")).Build(),
				new FileEntry.Builder().WithName(new FileName("OldFile2")).Build()
			}).ToList();

			var currentFileList = originalFileList.Except(filesToRemove);

			var diff = FileListDiff.Compare(originalFileList, currentFileList.ToList());

			Assert.That(diff.AddedFiles,    Is.Empty);
			Assert.That(diff.ModifiedFiles, Is.Empty);
			Assert.That(diff.RemovedFiles,  Is.EquivalentTo(filesToRemove));
		}

		[Test]
		public void WhenFilesWereRemovedFromSubDirectory_TheyAreInRemovedFilesList()
		{
			var filesToRemove = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("FileToRemove")).Build(),
				new FileEntry.Builder().WithName(new FileName("FileToRemove2")).Build()
			};

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().AsDirectory().WithName(new FileName("DirectoryWithFiles")).WithFiles(filesToRemove).Build(),
				new FileEntry.Builder().AsDirectory().WithName(new FileName("OldDirectory")).Build()
			};
			
			var currentFileList = new FileEntry[]
			{
				originalFileList[0].BuildCopy().WithoutFileNamed(filesToRemove[0].Name)
				                               .WithoutFileNamed(filesToRemove[1].Name).Build(),
				originalFileList[1]
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.RemovedFiles, Is.EquivalentTo(filesToRemove));
		}

		[Test]
		public void WhenDirectoryWithFilesWasRemoved_OnlyTheDirectoryIsInRemovedFilesList()
		{
			var fileInDirectory  = new FileEntry.Builder().WithName(new FileName("FileInDirectory")).Build();
			var fileInDirectory2 = new FileEntry.Builder().WithName(new FileName("FileInDirectory2")).Build();
			var directory        = new FileEntry.Builder().AsDirectory()
				                                           .WithName(new FileName("DirectoryToRemove"))
				                                           .WithFiles(fileInDirectory, fileInDirectory2)
				                                           .Build();

			var originalFileList = new FileEntry[]
			{
				directory,
				new FileEntry.Builder().WithName(new FileName("OldFile")).Build(),
				new FileEntry.Builder().AsDirectory().WithName(new FileName("OldDirectory")).Build(),
			};
			
			var currentFileList = originalFileList.Except(directory.ToSingleElementList());

			var diff = FileListDiff.Compare(originalFileList, currentFileList.ToList());

			Assert.That(diff.RemovedFiles, Is.EquivalentTo(directory.ToSingleElementList()));
		}

		[Test]
		public void WhenReplacingFile_TheOldFileIsInRemovedFilesListAndNewOneInAddedFiles()
		{
			var originalFile = new FileEntry.Builder().WithName(new FileName("ReplacedFile")).Build();
			var newFile      = new FileEntry.Builder().AsNew().WithName(new FileName("ReplacedFile")).Build();

			var originalFileList = new FileEntry[]
			{
				originalFile,
				new FileEntry.Builder().WithName(new FileName("OldFile")).Build(),
			};

			var currentFileList = originalFileList.Except(originalFile.ToSingleElementList())
			                                      .Concat(newFile.ToSingleElementList());

			var diff = FileListDiff.Compare(originalFileList, currentFileList.ToList());

			Assert.That(diff.RemovedFiles, Is.EquivalentTo(originalFile.ToSingleElementList()));
			Assert.That(diff.AddedFiles,   Is.EquivalentTo(newFile.ToSingleElementList()));
		}

		[Test]
		public void WhenFilesWereModified_TheyAreInModifiedFilesList()
		{
			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("File1")).Build(),
				new FileEntry.Builder().WithName(new FileName("File2")).Build(),
				new FileEntry.Builder().WithName(new FileName("File3")).Build(),
				new FileEntry.Builder().WithName(new FileName("File4")).Build()
			};
			
			var modifiedFiles = new FileEntry[]
			{
				originalFileList[0].BuildCopy().ModifiedOn(DateTime.Now).Build(),
				originalFileList[2].BuildCopy().WithSize(1234).Build()
			};

			var currentFileList = new FileEntry[]
			{
				originalFileList[1],
				modifiedFiles[0],
				modifiedFiles[1],
				originalFileList[3]
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.AddedFiles,    Is.Empty);
			Assert.That(diff.ModifiedFiles, Is.EquivalentTo(modifiedFiles));
			Assert.That(diff.RemovedFiles,  Is.Empty);
		}

		[Test]
		public void WhenFilesInSubDirectoryWereModified_TheyAreInModifiedFilesList()
		{
			var fileInDirectory  = new FileEntry.Builder().WithName(new FileName("FileInDirectory")).Build();
			var fileInDirectory2 = new FileEntry.Builder().WithName(new FileName("FileInDirectory2")).Build();
			var fileInDirectory3 = new FileEntry.Builder().WithName(new FileName("FileInDirectory3")).Build();
			var directory        = new FileEntry.Builder().AsDirectory()
				                                           .WithName(new FileName("Directory"))
				                                           .WithFiles(fileInDirectory, fileInDirectory2, fileInDirectory3)
				                                           .Build();

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("File1")).Build(),
				directory,
				new FileEntry.Builder().WithName(new FileName("File2")).Build(),
			};

			var modifiedFiles = new FileEntry[]
			{
				directory.Files.ElementAt(0).BuildCopy().ModifiedOn(DateTime.Now).Build(),
				directory.Files.ElementAt(2).BuildCopy().WithSize(1234).Build()
			};
			
			var currentFileList = new FileEntry[]
			{
				originalFileList[0],
				originalFileList[1].BuildCopy().WithoutFileNamed(modifiedFiles[0].Name)
				                               .WithoutFileNamed(modifiedFiles[1].Name)
				                               .WithFiles(modifiedFiles).Build(),
				originalFileList[2],
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.ModifiedFiles, Is.EquivalentTo(modifiedFiles));
		}

		[Test]
		public void WhenDirectoryAndSomeOfItsFilesWereModified_BothAreIsInModifiedFilesList()
		{
			var fileInDirectory  = new FileEntry.Builder().WithName(new FileName("FileInDirectory")).Build();
			var fileInDirectory2 = new FileEntry.Builder().WithName(new FileName("FileInDirectory2")).Build();
			var fileInDirectory3 = new FileEntry.Builder().WithName(new FileName("FileInDirectory3")).Build();
			var directory        = new FileEntry.Builder().AsDirectory()
				                                           .WithName(new FileName("Directory"))
				                                           .WithFiles(fileInDirectory, fileInDirectory2, fileInDirectory3)
				                                           .Build();

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("File1")).Build(),
				directory,
				new FileEntry.Builder().WithName(new FileName("File2")).Build(),
			};

			var modifiedFiles = new FileEntry[]
			{
				directory.Files.ElementAt(0).BuildCopy().ModifiedOn(DateTime.Now).Build(),
				directory.Files.ElementAt(2).BuildCopy().WithSize(1234).Build(),
				directory.BuildCopy().ModifiedOn(DateTime.Now.AddHours(23.54)).Build()
			};
			
			var currentFileList = new FileEntry[]
			{
				originalFileList[0],
				originalFileList[2],
				modifiedFiles[2].BuildCopy().WithoutFileNamed(modifiedFiles[0].Name)
				                            .WithoutFileNamed(modifiedFiles[1].Name)
				                            .WithNewFile(modifiedFiles[0])
				                            .WithNewFile(modifiedFiles[1])
				                            .Build()
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.ModifiedFiles, Is.EquivalentTo(modifiedFiles));
		}

		[Test]
		public void WhenDirectoryHasBeenRenamed_ItIsInModifiedFilesList()
		{
			var fileInDirectory  = new FileEntry.Builder().WithName(new FileName("FileInDirectory")).Build();
			var fileInDirectory2 = new FileEntry.Builder().WithName(new FileName("FileInDirectory2")).Build();
			var fileInDirectory3 = new FileEntry.Builder().WithName(new FileName("FileInDirectory3")).Build();
			var directory        = new FileEntry.Builder().AsDirectory()
				                                           .WithName(new FileName("Directory"))
				                                           .WithFiles(fileInDirectory, fileInDirectory2, fileInDirectory3)
				                                           .Build();

			var originalFileList = new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("File1")).Build(),
				directory,
				new FileEntry.Builder().WithName(new FileName("File2")).Build(),
			};

			var renamedDirectory = directory.BuildCopy().WithName(new FileName("RenamedDirectory")).Build();
			
			var currentFileList = new FileEntry[]
			{
				originalFileList[0],
				originalFileList[2],
				renamedDirectory
			};

			var diff = FileListDiff.Compare(originalFileList, currentFileList);

			Assert.That(diff.ModifiedFiles, Is.EquivalentTo(renamedDirectory.ToSingleElementList()));
		}
	}
}
