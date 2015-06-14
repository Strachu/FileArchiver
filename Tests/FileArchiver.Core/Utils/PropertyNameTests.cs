using FileArchiver.Core.Utils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Utils
{
	internal class PropertyNameTests
	{
		private class TestClass
		{
			public string Field = null;
			public string Property { get; set; }
		}

		private TestClass mTestObject;

		[SetUp]
		public void SetUp()
		{
			mTestObject = new TestClass();
		}

		[Test]
		public void NameOfFieldFromExistingObject()
		{
			Assert.That(PropertyName.Of(() => mTestObject.Field), Is.EqualTo("Field"));
		}

		[Test]
		public void NameOfPropertyFromExistingProperty()
		{
			Assert.That(PropertyName.Of(() => mTestObject.Property), Is.EqualTo("Property"));
		}

		[Test]
		public void NameOfFieldFromClass()
		{
			Assert.That(PropertyName.Of<TestClass>(x => x.Field), Is.EqualTo("Field"));
		}

		[Test]
		public void NameOfPropertyFromClass()
		{
			Assert.That(PropertyName.Of<TestClass>(x => x.Property), Is.EqualTo("Property"));
		}
	}
}
