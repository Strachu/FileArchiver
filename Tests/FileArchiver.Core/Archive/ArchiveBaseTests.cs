using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Tests.Archive
{
	[TestFixture]
	internal class ArchiveBaseTests
	{
		private ArchiveBase mTestedArchive;

		[SetUp]
		public void SetUp()
		{
			mTestedArchive = A.Fake<ArchiveBase>();

			A.CallTo(mTestedArchive).CallsBaseMethod();
		}

		[Test]
		public void WhenAddingFileToRootDirectory_ItCanLaterBeRetrieved()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(Path.Root, file);

			Assert.That(mTestedArchive.RootFiles,                    Contains.Item(file));
			Assert.That(mTestedArchive.GetFile(file.Id),             Is.EqualTo(file));
			Assert.That(mTestedArchive.GetFile(new Path("File")),    Is.EqualTo(file));
			Assert.That(mTestedArchive.FileExists(new Path("File")), Is.True);
		}

		[Test]
		public void WhenAddingDirectoryWithFilesToRootDirectory_ItsChildrenAreAlsoAvailableWithGetFile()
		{
			var file1 = new FileEntry.Builder().WithName(new FileName("File1")).Build();
			var file2 = new FileEntry.Builder().WithName(new FileName("File2")).Build();
			var file3 = new FileEntry.Builder().WithName(new FileName("File3")).Build();

			var directory = new FileEntry.Builder().AsDirectory()
			                                       .WithName(new FileName("Directory"))
			                                       .WithFiles(file1, file2, file3)
			                                       .Build();

			mTestedArchive.AddFile(Path.Root, directory);

			Assert.That(mTestedArchive.RootFiles,                               Contains.Item(directory));
			Assert.That(mTestedArchive.GetFile(directory.Id),                   Is.EqualTo(directory));
			Assert.That(mTestedArchive.GetFile(new Path("Directory")),          Is.EqualTo(directory));
			Assert.That(mTestedArchive.FileExists(new Path("Directory")),       Is.True);

			Assert.That(mTestedArchive.GetFile(file1.Id),                       Is.EqualTo(file1));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/File1")),    Is.EqualTo(file1));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File1")), Is.True);

			Assert.That(mTestedArchive.GetFile(file2.Id),                       Is.EqualTo(file2));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/File2")),    Is.EqualTo(file2));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File2")), Is.True);

			Assert.That(mTestedArchive.GetFile(file3.Id),                       Is.EqualTo(file3));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/File3")),    Is.EqualTo(file3));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File3")), Is.True);
		}

		[Test]
		public void WhenAddingFileToSubDirectory_ItCanLaterBeRetrieved()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().AsDirectory().WithName(new FileName("Directory")).Build());

			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(new Path("Directory"), file);

			Assert.That(mTestedArchive.GetFile(file.Id),                       Is.EqualTo(file));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/File")),    Is.EqualTo(file));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File")), Is.True);
		}

		[Test]
		public void WhenAddingDirectoryWithFilesToSubDirectory_ItsChildrenAreAlsoAvailableWithGetFile()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().AsDirectory().WithName(new FileName("Directory")).Build());

			var file1 = new FileEntry.Builder().WithName(new FileName("File1")).Build();
			var file2 = new FileEntry.Builder().WithName(new FileName("File2")).Build();
			var file3 = new FileEntry.Builder().WithName(new FileName("File3")).Build();

			var directory = new FileEntry.Builder().AsDirectory()
			                                       .WithName(new FileName("Directory"))
			                                       .WithFiles(file1, file2, file3)
			                                       .Build();

			mTestedArchive.AddFile(new Path("Directory"), directory);

			Assert.That(mTestedArchive.GetFile(directory.Id),                             Is.EqualTo(directory));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/Directory")),          Is.EqualTo(directory));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/Directory")),       Is.True);

			Assert.That(mTestedArchive.GetFile(file1.Id),                                 Is.EqualTo(file1));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/Directory/File1")),    Is.EqualTo(file1));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/Directory/File1")), Is.True);

			Assert.That(mTestedArchive.GetFile(file2.Id),                                 Is.EqualTo(file2));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/Directory/File2")),    Is.EqualTo(file2));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/Directory/File2")), Is.True);

			Assert.That(mTestedArchive.GetFile(file3.Id),                                 Is.EqualTo(file3));
			Assert.That(mTestedArchive.GetFile(new Path("Directory/Directory/File3")),    Is.EqualTo(file3));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/Directory/File3")), Is.True);
		}

		[Test]
		public void WhenFileAlreadyExists_AddingItToRootDirectoryThrowsFileExistsException()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().WithName(new FileName("File")).Build());

			Assert.Throws<FileExistsException>(() =>
			{
				mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().WithName(new FileName("File")).Build());
			});
		}

		[Test]
		public void WhenFileAlreadyExists_AddingItToSubDirectoryThrowsFileExistsException()
		{
			mTestedArchive.AddFile(Path.Root,             new FileEntry.Builder().AsDirectory().WithName(new FileName("Directory")).Build());
			mTestedArchive.AddFile(new Path("Directory"), new FileEntry.Builder().WithName(new FileName("File")).Build());

			Assert.Throws<FileExistsException>(() =>
			{
				mTestedArchive.AddFile(new Path("Directory"), new FileEntry.Builder().WithName(new FileName("File")).Build());
			});
		}

		[Test]
		public void WhenFileAlreadyExists_WhenUsingNonAsciiCharacters_AddingItArchiveThrowsFileExistsException()
		{
			mTestedArchive.AddFile(Path.Root,
			                       new FileEntry.Builder().WithName(new FileName("漢字 żółta żaba żarła żur 汉字")).Build());

			Assert.Throws<FileExistsException>(() =>
			{
				mTestedArchive.AddFile(Path.Root,
				                       new FileEntry.Builder().WithName(new FileName("漢字 żółta żaba żarła żur 汉字")).Build());
			});
		}
		
		[Test]
		public void WhenAddingToSubDirectory_DestinationDirectoryDoesNotExist_ThrowsException()
		{
			Assert.Throws<FileNotFoundException>(() =>
			{
				mTestedArchive.AddFile(new Path("ImaginedDirectory"),
				                       new FileEntry.Builder().WithName(new FileName("File")).Build());
			});
		}
		
		[Test]
		public void WhenAddingToSubDirectory_DestinationDirectoryPointsToAFile_ThrowsException()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().WithName(new FileName("File")).Build());

			Assert.Throws<ArgumentException>(() =>
			{
				mTestedArchive.AddFile(new Path("File"), new FileEntry.Builder().WithName(new FileName("File")).Build());
			});
		}
		
		[Test]
		public void WhenAddingFile_OnFileAddedEventIsRaised()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			FileEntry filePassedToEvent = null;
			mTestedArchive.FileAdded += (sender, e) => filePassedToEvent = e.AddedFile;

			mTestedArchive.AddFile(Path.Root, file);

			Assert.That(filePassedToEvent, Is.EqualTo(file));
		}
		
		[Test]
		public void WhenGettingFileByPath_FileDoesNotExist_ExceptionIsThrown()
		{
			Assert.Throws<FileNotFoundException>(() =>
			{
				mTestedArchive.GetFile(new Path("ImaginedDirectory/ImaginedFile"));
			});
		}
		
		[Test]
		public void WhenGettingFileByID_FileDoesNotExist_ExceptionIsThrown()
		{
			Assert.Throws<FileNotFoundException>(() =>
			{
				mTestedArchive.GetFile(Guid.NewGuid());
			});
		}
		
		[Test]
		public void RemovingFile_FileDoesNotExist_ExceptionIsNotThrown()
		{
			Assert.DoesNotThrow(() =>
			{
				mTestedArchive.RemoveFile(new Path("ImaginedDirectory/ImaginedFile"));
			});
		}
		
		[Test]
		public void RemovingFile_FromRootDiretory_Works()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(Path.Root, file);
			mTestedArchive.RemoveFile(new Path("File"));

			Assert.That(mTestedArchive.RootFiles,                    Has.No.Member(file));
			Assert.That(mTestedArchive.FileExists(new Path("File")), Is.False);
		}
		
		[Test]
		public void RemovingFile_FromSubDiretory_Works()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(Path.Root,             new FileEntry.Builder().AsDirectory().WithName(new FileName("Directory")).Build());
			mTestedArchive.AddFile(new Path("Directory"), file);

			mTestedArchive.RemoveFile(new Path("Directory/File"));

			Assert.That(mTestedArchive.RootFiles,                              Has.No.Member(file));
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File")), Is.False);
		}

		[Test]
		public void RemovingDirectory_RemovesAlsoItsChildrenFromIndex()
		{
			var file1 = new FileEntry.Builder().WithName(new FileName("File1")).Build();
			var file2 = new FileEntry.Builder().WithName(new FileName("File2")).Build();
			var file3 = new FileEntry.Builder().WithName(new FileName("File3")).Build();

			var directory = new FileEntry.Builder().AsDirectory()
			                                       .WithName(new FileName("Directory"))
			                                       .WithFiles(file1, file2, file3)
			                                       .Build();

			mTestedArchive.AddFile(Path.Root, directory);

			mTestedArchive.RemoveFile(new Path("Directory"));

			Assert.That(mTestedArchive.RootFiles,                               Has.No.Member(directory));
			Assert.That(mTestedArchive.FileExists(new Path("Directory")),       Is.False);
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File1")), Is.False);
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File2")), Is.False);
			Assert.That(mTestedArchive.FileExists(new Path("Directory/File3")), Is.False);
		}
		
		[Test]
		public void WhenRemovingFile_OnFileRemovedEventIsRaised()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(Path.Root, file);

			FileEntry filePassedToEvent = null;
			mTestedArchive.FileRemoved += (sender, e) => filePassedToEvent = e.RemovedFile;
			
			mTestedArchive.RemoveFile(new Path("File"));

			Assert.That(filePassedToEvent, Is.EqualTo(file));
		}
		
		[Test]
		public void WhenArchiveIsEmpty_IsModifiedReturnsFalse()
		{
			Assert.That(mTestedArchive.IsModified, Is.False);
		}
		
		[Test]
		public void WhenFileIsAddedWithUnchangedState_IsModifiedReturnsFalse()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().WithName(new FileName("File")).Build());

			Assert.That(mTestedArchive.IsModified, Is.False);
		}
		
		[Test]
		public void WhenFileIsAddedWithNewState_IsModifiedReturnsTrue()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().AsNew().WithName(new FileName("File")).Build());

			Assert.That(mTestedArchive.IsModified, Is.True);
		}
		
		[Test]
		public void WhenFileIsAddedWithNewStateToSubDirectory_IsModifiedReturnsTrue()
		{
			mTestedArchive.AddFile(Path.Root,             new FileEntry.Builder().AsDirectory().WithName(new FileName("Directory")).Build());
			mTestedArchive.AddFile(new Path("Directory"), new FileEntry.Builder().AsNew().WithName(new FileName("File")).Build());

			Assert.That(mTestedArchive.IsModified, Is.True);
		}
		
		[Test]
		public void WhenFileIsRemovedWithUnchangedState_IsModifiedReturnsTrue()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().WithName(new FileName("File")).Build());

			mTestedArchive.RemoveFile(new Path("File"));

			Assert.That(mTestedArchive.IsModified, Is.True);
		}
		
		[Test]
		public void WhenFileIsRemovedWithNewState_IsModifiedReturnsFalse_BecauseTheAddingWasReversed()
		{
			mTestedArchive.AddFile(Path.Root, new FileEntry.Builder().AsNew().WithName(new FileName("File")).Build());

			mTestedArchive.RemoveFile(new Path("File"));

			Assert.That(mTestedArchive.IsModified, Is.False);
		}
		
		[Test]
		public void WhenFileIsRemoved_IsLaterAddedBack_IsModifiedReturnsFalse()
		{
			var file = new FileEntry.Builder().WithName(new FileName("File")).Build();

			mTestedArchive.AddFile(Path.Root, file);
			mTestedArchive.RemoveFile(file.Name);
			mTestedArchive.AddFile(Path.Root, file);

			Assert.That(mTestedArchive.IsModified, Is.False);
		}
	}
}
