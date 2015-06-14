using FileArchiver.Core.Utils;
using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Utils
{
	internal class MultipleChoicesEnablementManagerTests
	{
		private TestViewModel mTestObject;

		[SetUp]
		public void SetUp()
		{
			mTestObject = new TestViewModel();
		}

		[Test]
		public void EnabledPropertyIsInitializedToTrueWhenTheCollectionHasMultipleEntries()
		{
			mTestObject.Collection = new int[] {1, 2, 3};

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			Assert.That(testedManager.Enabled, Is.True);
		}

		[Test]
		public void EnabledPropertyIsInitializedToFalseWhenTheCollectionHasOneEntry()
		{
			mTestObject.Collection = new int[] { 1 };

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			Assert.That(testedManager.Enabled, Is.False);
		}

		[Test]
		public void EnabledPropertyIsInitializedToFalseWhenTheCollectionIsEmpty()
		{
			mTestObject.Collection = new int[] { };

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			Assert.That(testedManager.Enabled, Is.False);
		}

		[Test]
		public void EnabledPropertyIsSetToTrueAfterCollectionReceiveMultipleEntries()
		{
			mTestObject.Collection = new int[] { 1 };

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { 1, 2, 3 };

			Assert.That(testedManager.Enabled, Is.True);
		}

		[Test]
		public void EnabledPropertyIsSetToFalseAfterCollectionReceiveOneEntry()
		{
			mTestObject.Collection = new int[] { 1, 2, 3 };

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { 1 };

			Assert.That(testedManager.Enabled, Is.False);
		}

		[Test]
		public void EnabledPropertyIsSetToFalseAfterCollectionIsEmptied()
		{
			mTestObject.Collection = new int[] { 1, 2, 3 };

			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { };

			Assert.That(testedManager.Enabled, Is.False);
		}

		[Test]
		public void PropertyNotificationIsRaisedAfterEnabledChanges()
		{
			var testedManager = MultipleChoicesEnablementManager.For(() => mTestObject.Collection);

			string changedPropertyName = null;
			testedManager.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			mTestObject.Collection = new int[] { 1, 2, 3 };

			Assert.That(changedPropertyName, Is.EqualTo(PropertyName.Of(() => testedManager.Enabled)));
		}
	}
}
