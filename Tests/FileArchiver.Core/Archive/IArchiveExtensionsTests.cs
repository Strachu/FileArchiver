using System.Threading;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Archive
{
	[TestFixture]
	public class IArchiveExtensionsTests
	{
		private IArchive mArchiveMock;

		[SetUp]
		public void SetUp()
		{
			mArchiveMock = A.Fake<ArchiveBase>();

			A.CallTo(mArchiveMock).CallsBaseMethod();

			A.CallTo(() => mArchiveMock.ExtractFilesAsync(null, null, CancellationToken.None, null))
			 .WithAnyArguments()
			 .Returns(Task.FromResult(0));

			var directory = new FileEntry.Builder().WithName(new FileName("Directory")).AsDirectory().Build();
			var file      = new FileEntry.Builder().WithName(new FileName("NestedFile")).Build();

			mArchiveMock.AddFile(Path.Root,      directory);
			mArchiveMock.AddFile(directory.Path, file);
		}

		[Test]
		public void WhenFileIsInRootDirectory_UpdatingWorks()
		{
			var filePath  = new Path("Directory");
			var sizeToSet = 1234;

			var file    = mArchiveMock.GetFile(filePath);
			var newFile = file.BuildCopy().WithSize(sizeToSet).Build();

			mArchiveMock.UpdateFile(newFile);

			file = mArchiveMock.GetFile(filePath);

			Assert.That(file.Size, Is.EqualTo(sizeToSet));
		}

		[Test]
		public void WhenFileIsSomeDirectory_UpdatingWorks()
		{
			var filePath  = new Path("Directory/NestedFile");
			var sizeToSet = 2345;

			var file    = mArchiveMock.GetFile(filePath);
			var newFile = file.BuildCopy().WithSize(sizeToSet).Build();

			mArchiveMock.UpdateFile(newFile);

			file = mArchiveMock.GetFile(filePath);

			Assert.That(file.Size, Is.EqualTo(sizeToSet));
		}

		[Test]
		public void WhenFileNameChanged_UpdatingWorks()
		{
			var file    = mArchiveMock.GetFile(new Path("Directory/NestedFile"));
			var newFile = file.BuildCopy().WithName(new FileName("NewName")).Build();

			mArchiveMock.UpdateFile(newFile);

			Assert.That(mArchiveMock.FileExists(new Path("Directory/NewName")));
		}

		[Test]
		public void WhenFilesToExtractListIsEmpty_ExtractFilesOverloadWithDestinationDirectory_DoesNotCrash()
		{
			var emptyFileList = new Path[] { };

			Assert.DoesNotThrow(async () =>
			{
				await mArchiveMock.ExtractFilesAsync(new Path("C:\\directory"), emptyFileList, A.Fake<FileExtractionErrorHandler>(),
				                                     CancellationToken.None);
			});
		}

		[Test]
		public async void WhenExtractingFilesFromRootDirectory_ExtractFilesOverloadWithDestinationDirectoryWorks()
		{
			var filesToExtract = new Path[]
			{
				new Path("Directory"),
				new Path("File")
			};

			await mArchiveMock.ExtractFilesAsync(new Path("C:\\directory"), filesToExtract, A.Fake<FileExtractionErrorHandler>(),
				                                  CancellationToken.None);

			ArchiveTestUtil.AssertFilesExtractedTo(mArchiveMock, new Path("C:\\directory"));
			ArchiveTestUtil.AssertFilesExtracted(mArchiveMock, filesToExtract.Select(x => x.ToString()).ToArray());
		}

		[Test]
		public async void WhenExtractingFilesFromNestedDirectory_ExtractFilesOverloadWithDestinationDirectory_SkipsParentDirectoryInDestinationPath()
		{
			var filesToExtract = new Path[]
			{
				new Path("Directory/NestedFiles"),
				new Path("Directory/NestedFiles2")
			};

			await mArchiveMock.ExtractFilesAsync(new Path("C:\\dir"), filesToExtract, A.Fake<FileExtractionErrorHandler>(),
				                                  CancellationToken.None);

			ArchiveTestUtil.AssertFilesExtractedTo(mArchiveMock, new Path("C:\\dir"));
			ArchiveTestUtil.AssertFilesExtracted(mArchiveMock, filesToExtract.Select(x => x.ToString()).ToArray());
		}
	}
}
