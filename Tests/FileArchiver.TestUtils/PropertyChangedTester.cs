using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Core.Utils;
using NUnit.Framework;

namespace FileArchiver.TestUtils
{
	public class PropertyChangedTester
	{
		private readonly List<string> mChangedProperties = new List<string>();

		public PropertyChangedTester(INotifyPropertyChanged testSubject)
		{
			Contract.Requires(testSubject != null);

			testSubject.PropertyChanged += PropertyChanged;
		}

		private void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			mChangedProperties.Add(e.PropertyName);
		}

		public void AssertPropertyChangedRaisedFor<TMemberReturn>(Expression<Func<TMemberReturn>> propertyAccessor)
		{
			var propertyName = PropertyName.Of(propertyAccessor);

			Assert.That(mChangedProperties, Has.Some.EqualTo(propertyName),
			            "The PropertyChange notification for " + propertyName + " has not been raised.");
		}

		public void AssertNoPropertyChangedRaised()
		{
			Assert.That(mChangedProperties, Is.Empty);
		}
	}
}
