using FileArchiver.Core.Services;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Tests.Services
{
	[TestFixture]
	public class NumberedFileNameGeneratorTests
	{
		private readonly IFileNameGenerator mTestedNameGenerator = new NumberedFileNameGenerator();

		[Test]
		public void GeneratorReturnsOriginalNameWhenItIsAvailable()
		{
			var generatedName = mTestedNameGenerator.GenerateFreeFileName(new Path("OriginalName.txt"), x => false);

			Assert.That(generatedName, Is.EqualTo(new Path("OriginalName.txt")));
		}

		[Test]
		public void GenerateNameWithoutExtension()
		{
			var generatedName = mTestedNameGenerator.GenerateFreeFileName(new Path("OriginalName"),
			                                                              x => PredicateReturningTrueUntilNthCall(3));

			Assert.That(generatedName, Is.EqualTo(new Path("OriginalName - 3")));
		}

		[Test]
		public void GenerateNameWithExtension()
		{
			var generatedName = mTestedNameGenerator.GenerateFreeFileName(new Path("OriginalName.txt"),
			                                                              x => PredicateReturningTrueUntilNthCall(5));

			Assert.That(generatedName, Is.EqualTo(new Path("OriginalName - 5.txt")));
		}

		[Test]
		public void GenerateNameWithPathAndExtension()
		{
			var generatedName = mTestedNameGenerator.GenerateFreeFileName(new Path("C:/Test/OriginalName.exe"),
			                                                              x => PredicateReturningTrueUntilNthCall(2));

			Assert.That(generatedName, Is.EqualTo(new Path("C:/Test/OriginalName - 2.exe")));
		}

		private int mPredicateCallsCount = 0;

		private bool PredicateReturningTrueUntilNthCall(int callNumber)
		{
			if(mPredicateCallsCount < callNumber - 1)
			{
				mPredicateCallsCount++;
				return true;
			}

			mPredicateCallsCount = 0;
			return false;
		}
	}
}
