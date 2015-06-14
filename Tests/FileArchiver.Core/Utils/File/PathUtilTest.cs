using System;

using FileArchiver.Core.Utils.File;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Tests.Utils.File
{
	[TestFixture]
	internal class PathUtilTest
	{
		[Test]
		public void WhenThereIsNoParentDirectory_GetParentDirectoryReturnsEmptyPath()
		{
			var paths = new Path[]
			{
				new Path("Dir\\a.txt"),
				new Path("b.txt"),
				new Path("Directory\\file")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path(String.Empty)));
		}

		[Test]
		public void WhenThereIsNoParentDirectoryWithSharedCharacters_GetParentDirectoryReturnsEmptyPath()
		{
			var paths = new Path[]
			{
				new Path("Dir\\a.txt"),
				new Path("D.txt"),
				new Path("Directory\\file")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path(String.Empty)));
		}

		[Test]
		public void WhenTheParentIsHardDriveLetterWithoutSharedCharactersGetParentDirectoryReturnsHardDriveLetter()
		{
			var paths = new Path[]
			{
				new Path("C:\\Dir\\a.txt"),
				new Path("C:\\b.txt"),
				new Path("C:\\Directory\\file")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path("C:/")));
		}

		[Test]
		public void GetParentDirectoryWhenTheDirectoryIsHardDriveLetterDoesNotReturnMoreThanOneTrailingSlash()
		{
			var paths = new Path[]
			{
				new Path("C:\\file.txt")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path("C:/")));
		}

		[Test]
		public void GetParentDirectoryWithFlatDirectory()
		{
			var paths = new Path[]
			{
				new Path("Directory\\a.txt"),
				new Path("Directory\\b.txt"),
				new Path("Directory\\file")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path("Directory")));
		}

		[Test]
		public void GetParentDirectoryWithHierarchicalDirectory()
		{
			var paths = new Path[]
			{
				new Path("/home/Directory/Dir2/NextDir/abc.txt"),
				new Path("/home/Directory/Dir2/NextDir/Dir/cfe.txt"),
				new Path("/home/Directory/Dir2/NextDir/Dir/xyzf.txt"),
				new Path("/home/Directory/Dir2/Dir/file"),
				new Path("/home/Directory/X/file"),
				new Path("/home/Directory/Dirrr/Dir/rdfs.ext"),
				new Path("/home/Directory/Dir2/NextDir/Dir/file.txt")
			};

			var result = PathUtil.GetParentDirectory(paths);

			Assert.That(result, Is.EqualTo(new Path("/home/Directory")));
		}
	}
}
