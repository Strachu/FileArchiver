using System;
using System.Linq.Expressions;

using FileArchiver.Presentation.ArchiveSettings.Framework.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Utils
{
	[TestFixture]
	internal class MutablePropertyReferenceTests : PropertyReferenceTests
	{
		protected override PropertyReference<T> CreateReference<T>(Expression<Func<T>> propertyExpression)
		{
			return MutablePropertyReference.To(propertyExpression);
		}

		[Test]
		public void SetterSetsCorrectValueWhenBothObjectAndAccessedMemberAreFields()
		{
			mTestObject.Field = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObject.Field);

			testedReference.Value = "NewValue";

			Assert.That(mTestObject.Field, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWhenObjectIsAFieldAndAccessedMemberIsAProperty()
		{
			mTestObject.Property = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObject.Property);

			testedReference.Value = "NewValue";

			Assert.That(mTestObject.Property, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWhenObjectIsAPropertyAndAccessedMemberIsAField()
		{
			mTestObjectProperty.Field = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObjectProperty.Field);

			testedReference.Value = "NewValue";

			Assert.That(mTestObjectProperty.Field, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWhenBothObjectAndAccessedMemberAreProperties()
		{
			mTestObjectProperty.Property = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObjectProperty.Property);

			testedReference.Value = "NewValue";

			Assert.That(mTestObjectProperty.Property, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWhenObjectIsAVariable()
		{
			var obj = new TestViewModel
			{
				Property = "OldValue"
			};

			var testedReference = MutablePropertyReference.To(() => obj.Property);

			testedReference.Value = "NewValue";

			Assert.That(obj.Property, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWithFieldsAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelField.Field = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObject.NestedViewModelField.NestedViewModelField.Field);

			testedReference.Value = "NewValue";

			Assert.That(mTestObject.NestedViewModelField.NestedViewModelField.Field, Is.EqualTo("NewValue"));
		}

		[Test]
		public void SetterSetsCorrectValueWithPropertyAccessorTrain()
		{
			mTestObject.NestedViewModelField = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelProperty = new TestViewModel();
			mTestObject.NestedViewModelField.NestedViewModelProperty.Property = "OldValue";

			var testedReference = MutablePropertyReference.To(() => mTestObject.NestedViewModelField.NestedViewModelProperty.Property);

			testedReference.Value = "NewValue";

			Assert.That(mTestObject.NestedViewModelField.NestedViewModelProperty.Property, Is.EqualTo("NewValue"));
		}
	}
}
