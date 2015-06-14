using FileArchiver.Core.Utils;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Utils
{
	internal class MultipleChoicesVisibilityManagerTests
	{
		private TestViewModel mTestObject;

		[SetUp]
		public void SetUp()
		{
			mTestObject = new TestViewModel();
		}

		[Test]
		public void VisiblePropertyIsInitializedToTrueWhenTheCollectionHasAnyEntry()
		{
			mTestObject.Collection = new int[] { 1 };

			var testedManager = MultipleChoicesVisibilityManager.For(() => mTestObject.Collection);

			Assert.That(testedManager.Visible, Is.True);
		}

		[Test]
		public void VisiblePropertyIsInitializedToFalseWhenTheCollectionIsEmpty()
		{
			mTestObject.Collection = new int[] { };

			var testedManager = MultipleChoicesVisibilityManager.For(() => mTestObject.Collection);

			Assert.That(testedManager.Visible, Is.False);
		}

		[Test]
		public void VisiblePropertyIsSetToTrueAfterCollectionReceiveSomeEntries()
		{
			mTestObject.Collection = new int[] { };

			var testedManager = MultipleChoicesVisibilityManager.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { 1, 2, 3 };

			Assert.That(testedManager.Visible, Is.True);
		}

		[Test]
		public void VisiblePropertyIsSetToFalseAfterCollectionIsEmptied()
		{
			mTestObject.Collection = new int[] { 1, 2, 3 };

			var testedManager = MultipleChoicesVisibilityManager.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { };

			Assert.That(testedManager.Visible, Is.False);
		}

		[Test]
		public void PropertyNotificationIsRaisedAfterVisibleChanges()
		{
			var testedManager = MultipleChoicesVisibilityManager.For(() => mTestObject.Collection);

			string changedPropertyName = null;
			testedManager.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			mTestObject.Collection = new int[] { 1, 2, 3 };

			Assert.That(changedPropertyName, Is.EqualTo(PropertyName.Of(() => testedManager.Visible)));
		}
	}
}
