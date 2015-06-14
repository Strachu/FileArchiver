using System;
using System.Linq.Expressions;

using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Utils
{
	[TestFixture]
	internal class PropertyReferenceTests
	{
		protected TestViewModel mTestObject;
		protected TestViewModel mTestObjectProperty { get; set; }

		[SetUp]
		public void SetUp()
		{
			mTestObject         = new TestViewModel();
			mTestObjectProperty = new TestViewModel();
		}

		protected virtual PropertyReference<T> CreateReference<T>(Expression<Func<T>> propertyExpression)
		{
			return PropertyReference.To(propertyExpression);
		}

		[Test]
		public void GetterReturnsCorrectValueWhenBothObjectAndAccessedMemberAreFields()
		{
			mTestObject.Field = "Test";

			var testedReference = CreateReference(() => mTestObject.Field);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWhenObjectIsAFieldAndAccessedMemberIsAProperty()
		{
			mTestObject.Property = "Test";

			var testedReference = CreateReference(() => mTestObject.Property);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWhenObjectIsAPropertyAndAccessedMemberIsAField()
		{
			mTestObjectProperty.Field = "Test";

			var testedReference = CreateReference(() => mTestObjectProperty.Field);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWhenBothObjectAndAccessedMemberAreProperties()
		{
			mTestObjectProperty.Property = "Test";

			var testedReference = CreateReference(() => mTestObjectProperty.Property);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWhenObjectIsAVariable()
		{
			var obj = new TestViewModel
			{
				Property = "Test"
			};

			var testedReference = CreateReference(() => obj.Property);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWithFieldsAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelField.Field = "Test";

			var testedReference = CreateReference(() => mTestObject.NestedViewModelField.NestedViewModelField.Field);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void GetterReturnsCorrectValueWithPropertyAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelProperty = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelProperty.Property = "Test";

			var testedReference = CreateReference(() => mTestObject.NestedViewModelField.NestedViewModelProperty.Property);

			Assert.That(testedReference.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void ForwardsPropertyNotificationWhenReferencedPropertyChangedWhenObjectIsAFieldAndAccessedMemberIsAProperty()
		{
			var testedReference       = CreateReference(() => mTestObject.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			mTestObject.Property = "Test";

			propertyChangedTester.AssertPropertyChangedRaisedFor(() => testedReference.Value);
		}

		[Test]
		public void ForwardsPropertyNotificationWhenReferencedPropertyChangedBothObjectAndAccessedMemberAreProperties()
		{
			var testedReference       = CreateReference(() => mTestObjectProperty.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			mTestObjectProperty.Property = "Test";

			propertyChangedTester.AssertPropertyChangedRaisedFor(() => testedReference.Value);
		}

		[Test]
		public void ForwardsPropertyNotificationWhenReferencedPropertyChangedWhenObjectIsAVariable()
		{
			var obj = new TestViewModel
			{
				Property = "Test"
			};

			var testedReference       = CreateReference(() => obj.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			obj.Property = "NewValue";

			propertyChangedTester.AssertPropertyChangedRaisedFor(() => testedReference.Value);
		}

		[Test]
		public void ForwardsPropertyNotificationWhenReferencedPropertyChangedWithFieldsAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelField = new TestViewModel();

			var testedReference       = CreateReference(() => mTestObject.NestedViewModelField.NestedViewModelField.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			mTestObject.NestedViewModelField.NestedViewModelField.Property = "NewValue";

			propertyChangedTester.AssertPropertyChangedRaisedFor(() => testedReference.Value);
		}

		[Test]
		public void ForwardsPropertyNotificationWhenReferencedPropertyChangedWithPropertyAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelProperty = new TestViewModel();

			var testedReference       = CreateReference(() => mTestObject.NestedViewModelField.NestedViewModelProperty.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			mTestObject.NestedViewModelField.NestedViewModelProperty.Property = "NewValue";

			propertyChangedTester.AssertPropertyChangedRaisedFor(() => testedReference.Value);
		}

		[Test]
		public void PropertyNotificationIsNotForwardedWhenNotReferencedPropertyChanges()
		{
			var testedReference       = CreateReference(() => mTestObject.Property);
			var propertyChangedTester = new PropertyChangedTester(testedReference);

			mTestObject.RaisePropertyChangedFor(() => mTestObject.NestedViewModelProperty);

			propertyChangedTester.AssertNoPropertyChangedRaised();
		}
	}
}
