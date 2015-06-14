using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.FileListView
{
	internal class FileListViewModelTestBase
	{
		protected IFromFileSystemFileAddingService mFileAddingService;
		protected IArchive                         mArchiveMock;

		protected FileListViewModel                mTestedModel;

		protected PropertyChangedTester            mPropertyChangedTester;

		[SetUp]
		public void SetUp()
		{
			SynchronizationContext.SetSynchronizationContext(new NullSynchronizationContext());

			mFileAddingService = A.Fake<IFromFileSystemFileAddingService>();

			mTestedModel       = new FileListViewModel(new GenericFileIconProvider(), mFileAddingService,
			                                           A.Fake<IFileAddingPerFileErrorPresenterFactory>());

			SetUpFakeArchive();

			mPropertyChangedTester = new PropertyChangedTester(mTestedModel);
		}

		private void SetUpFakeArchive()
		{
			mArchiveMock = A.Fake<ArchiveBase>();

			A.CallTo(mArchiveMock).CallsBaseMethod();

			mArchiveMock.AddFile(Path.Root, CreateFile("Directory1", isDirectory: true));
			mArchiveMock.AddFile(Path.Root, CreateFile("Directory2", isDirectory: true));
			mArchiveMock.AddFile(Path.Root, CreateFile("Directory3", isDirectory: true));
			mArchiveMock.AddFile(Path.Root, CreateFile("File1"));
			mArchiveMock.AddFile(Path.Root, CreateFile("File2"));
			mArchiveMock.AddFile(Path.Root, CreateFile("File3"));

			mArchiveMock.AddFile(new Path("Directory1"), CreateFile("Directory1InDirectory1", isDirectory: true));
			mArchiveMock.AddFile(new Path("Directory1"), CreateFile("File1InDirectory1"));
			mArchiveMock.AddFile(new Path("Directory1"), CreateFile("File2InDirectory1"));

			mArchiveMock.AddFile(new Path("Directory2"), CreateFile("Directory1InDirectory2", isDirectory: true));
			mArchiveMock.AddFile(new Path("Directory2"), CreateFile("File1InDirectory2"));

			mArchiveMock.AddFile(new Path("Directory3"), CreateFile("File1InDirectory3"));
			mArchiveMock.AddFile(new Path("Directory3"), CreateFile("File2InDirectory3"));

			mArchiveMock.AddFile(new Path("Directory2/Directory1InDirectory2"), CreateFile("File1InNestedDirectory"));
			mArchiveMock.AddFile(new Path("Directory2/Directory1InDirectory2"), CreateFile("File2InNestedDirectory"));	
		}

		protected FileEntry CreateFile(string name, bool isDirectory = false)
		{
			var builder = new FileEntry.Builder().WithName(new FileName(name));

			if(isDirectory)
			{
				builder.AsDirectory();
			}

			return builder.Build();
		}
	}
}
