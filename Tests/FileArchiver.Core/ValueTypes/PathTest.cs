using System;

using FileArchiver.Core.ValueTypes;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.ValueTypes
{
	internal class PathTest
	{
		[Test]
		public void LeadingSlashInUnixPathsIsNotTrimmed()
		{
			var path           = new Path("/Directory/File.txt");
			var pathAsString   = path.ToString();
			var expectedString = System.IO.Path.Combine("/", "Directory", "File.txt");

			Assert.That(pathAsString, Is.EqualTo(expectedString));
		}

		[Test]
		public void RootDirectoryInUnixPathsIsNotTrimmed()
		{
			var path           = new Path("/");
			var pathAsString   = path.ToString();

			Assert.That(pathAsString, Is.EqualTo("/"));
		}

		[Test]
		public void WhenPathsAreEqual_EqualsReturnTrue()
		{
			var firstPath  = new Path("Directory/Subdirectory/File.txt");
			var secondPath = new Path("Directory/Subdirectory/File.txt");

			Assert.That(firstPath.Equals(secondPath), Is.True);
		}

		[Test]
		public void WhenPathsAreDifferent_EqualsReturnFalse()
		{
			var firstPath  = new Path("Directory/Subdirectory/File.txt");
			var secondPath = new Path("Directory/Subdirectory2/File.txt");

			Assert.That(firstPath.Equals(secondPath), Is.False);
		}

		[Test]
		public void WhenPathsAreEqualButWithDifferentCase_EqualsReturnTrue()
		{
			var firstPath  = new Path("Directory/SubDirectory/File.TXT");
			var secondPath = new Path("DirectOry/SubdirEctory/File.txt");

			Assert.That(firstPath.Equals(secondPath), Is.True);
		}

		[Test]
		public void WhenPathsAreEqualWithMixedSeparators_EqualsReturnTrue()
		{
			var firstPath  = new Path("Directory\\SubDirectory/SubDir/File.txt");
			var secondPath = new Path("Directory/SubDirectory\\SubDir/File.txt");

			Assert.That(firstPath.Equals(secondPath), Is.True);
		}

		[Test]
		public void WhenPathsAreEqualButOneIsWithTrallingSlash_EqualsReturnTrue()
		{
			var firstPath  = new Path("Directory\\SubDirectory");
			var secondPath = new Path("DirectOry\\SubDirectory\\");

			Assert.That(firstPath.Equals(secondPath), Is.True);
		}

		[Test]
		public void ChangingFileNameWorks()
		{
			var startingPath = new Path("Directory/Subdirectory/File.txt");

			var resultPath   = startingPath.ChangeFileName(new FileName("FileWithNewName.doc"));

			Assert.That(resultPath, Is.EqualTo(new Path("Directory/Subdirectory/FileWithNewName.doc")));
		}

		[Test]
		public void FileNamePropertyContainsCorrectValue()
		{
			var path = new Path("Directory/Subdirectory/File.txt");

			Assert.That(path.FileName, Is.EqualTo(new FileName("File.txt")));
		}

		[Test]
		public void ParentDirectoryPropertyContainsCorrectValue()
		{
			var path = new Path("Directory/Subdirectory/File.txt");

			Assert.That(path.ParentDirectory, Is.EqualTo(new Path("Directory/Subdirectory/")));
		}

		[Test]
		public void WhenPathPointsToDriveLetter_ParentDirectoryReturnsNull()
		{
			var path = new Path("C:/");

			Assert.That(path.ParentDirectory, Is.Null);
		}

		[Test]
		public void WhenFileIsInRootDirectory_ParentDirectoryContainsCorrectValueAndNotNull()
		{
			var path = new Path("Directory");

			Assert.That(path.ParentDirectory, Is.EqualTo(new Path("")));
		}

		[Test]
		public void WhenPathPointsToRootDirectory_ParentDirectoryReturnsNull()
		{
			var path = new Path("");

			Assert.That(path.ParentDirectory, Is.Null);
		}

		[Test]
		public void ChangingDirectoryWorks()
		{
			var path    = new Path("/directory/subdirectory/subdirectory2/file.txt");
			var newPath = path.ChangeDirectory(new Path("/directory/subdirectory"), new Path("/directory/newsubdir"));
		 
			Assert.That(newPath, Is.EqualTo(new Path("/directory/newsubdir/subdirectory2/file.txt")));
		}

		[Test]
		public void WhenCurrentDirectoryIsSetToRoot_ChangingDirectoryDoesNotRemoveRemoveFirstCharacterFromPreviousPath()
		{
			var path    = new Path("subdirectory2/file.txt");
			var newPath = path.ChangeDirectory(Path.Root, new Path("directory/subdirectory"));
		 
			Assert.That(newPath, Is.EqualTo(new Path("directory/subdirectory/subdirectory2/file.txt")));
		}

		[Test]
		public void RemoveExtension_KeepsTheParentDirectory()
		{
			var path    = new Path("C:/directory/file.txt");
			var newPath = path.RemoveExtension();
		 
			Assert.That(newPath, Is.EqualTo(new Path("C:/directory/file")));
		}

		[Test]
		public void Remove_KeepsTheBeginningOfPathOnly()
		{
			var path         = new Path("C:/directory/path/to/remove.txt");
			var pathToRemove = new Path("path/to/remove.txt");
			var newPath      = path.Remove(pathToRemove);
		 
			Assert.That(newPath, Is.EqualTo(new Path("C:/directory")));
		}

		[Test]
		public void Remove_IsCaseInsensitive()
		{
			var path         = new Path("C:/directory/path/to/remove.txt");
			var pathToRemove = new Path("PaTh/To/rEmOvE.txt");
			var newPath      = path.Remove(pathToRemove);
		 
			Assert.That(newPath, Is.EqualTo(new Path("C:/directory")));
		}

		[Test]
		public void WhenPathToRemoveIsNotPartOfOriginalPath_Remove_ThrowsArgumentException()
		{
			var path         = new Path("C:/directory/path/to/remove.txt");
			var pathToRemove = new Path("no/path/like/this/exists");

			Assert.Throws<ArgumentException>(() => path.Remove(pathToRemove));
		}

		[Test]
		public void WhenPathsAreUnrelated_IsAncestorOfReturnsFalseInBothCases()
		{
			var firstPath  = new Path("directory/subdirectory/file");
			var secondPath = new Path("directory/another_subdirectory/file2");

			Assert.That(firstPath.IsAncestorOf(secondPath), Is.False);
			Assert.That(secondPath.IsAncestorOf(firstPath), Is.False);
		}

		[Test]
		public void WhenPathsAreEqual_IsAncestorOfReturnsFalse()
		{
			var firstPath  = new Path("directory/subdirectory/file");
			var secondPath = new Path("Directory/Subdirectory/file");

			Assert.That(firstPath.IsAncestorOf(secondPath), Is.False);
			Assert.That(secondPath.IsAncestorOf(firstPath), Is.False);
		}

		[Test]
		public void WhenFirstPathsIsParentOfSecond_IsAncestorOfReturnsTrueForFirstPathButFalseForSecond()
		{
			var firstPath  = new Path("directory/subdirectory");
			var secondPath = new Path("Directory/Subdirectory/file");

			Assert.That(firstPath.IsAncestorOf(secondPath), Is.True);
			Assert.That(secondPath.IsAncestorOf(firstPath), Is.False);
		}

		[Test]
		public void WhenFirstPathsIsGrandParentOfSecond_IsAncestorOfReturnsTrueForFirstPathButFalseForSecond()
		{
			var firstPath  = new Path("directory");
			var secondPath = new Path("Directory/Subdirectory/file");

			Assert.That(firstPath.IsAncestorOf(secondPath), Is.True);
			Assert.That(secondPath.IsAncestorOf(firstPath), Is.False);
		}
	}
}
