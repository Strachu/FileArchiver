using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Utils
{
	internal class EnumerablePropertyBindingWrapperTests
	{
		private TestViewModel mTestObject;

		[SetUp]
		public void SetUp()
		{
			mTestObject = new TestViewModel();
		}

		[Test]
		public void WrapperIsInitializedWithCollectionContent()
		{
			mTestObject.Collection = new int[] { 1, 2, 3 };

			var testedWrapper = EnumerablePropertyBindingWrapper.For(() => mTestObject.Collection);

			Assert.That(testedWrapper, Is.EquivalentTo(mTestObject.Collection));
		}

		[Test]
		public void WrapperIsSynchronizedWithCollectionAfterItChanges()
		{
			mTestObject.Collection = new int[] { };

			var testedWrapper = EnumerablePropertyBindingWrapper.For(() => mTestObject.Collection);

			mTestObject.Collection = new int[] { 1, 2, 3, 4, 5 };

			Assert.That(testedWrapper, Is.EquivalentTo(mTestObject.Collection));
		}

		[Test]
		public void WrapperRaisesNotificationAfterTheCollectionChanges()
		{
			var testedWrapper = EnumerablePropertyBindingWrapper.For(() => mTestObject.Collection);

			bool listChangedEventRaised = false;
			testedWrapper.ListChanged += (sender, e) => listChangedEventRaised = true;

			mTestObject.Collection = new int[] { 1, 2, 3 };

			Assert.That(listChangedEventRaised, Is.True);
		}

		[Test]
		public void WrapperNotRaisesNotificationWhenOtherPropertyChanges()
		{
			var testedWrapper = EnumerablePropertyBindingWrapper.For(() => mTestObject.Collection);

			bool listChangedEventRaised = false;
			testedWrapper.ListChanged += (sender, e) => listChangedEventRaised = true;

			mTestObject.RaisePropertyChangedFor(() => mTestObject.Property);

			Assert.That(listChangedEventRaised, Is.False);
		}
	}
}
