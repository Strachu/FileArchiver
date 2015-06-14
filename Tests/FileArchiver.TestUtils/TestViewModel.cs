using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using FileArchiver.Core.Utils;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.TestUtils
{
	/// <summary>
	/// A class which represents typical behavior of a "View Model" for use in tests which
	/// tests interaction with properties or property change notifications.
	/// </summary>
	internal class TestViewModel : NotifyPropertyChangedHelper
	{
		private string           mProperty;
		private IEnumerable<int> mCollection;

		public string Field = null;

		public string Property
		{
			get { return mProperty; }
			set { SetFieldWithNotification(ref mProperty, value); }
		}

		public IEnumerable<int> Collection
		{
			get { return mCollection; }
			set { SetFieldWithNotification(ref mCollection, value); }
		}

		public TestViewModel NestedViewModelProperty
		{
			get;
			set;
		}

		public TestViewModel NestedViewModelField = null;

		public void RaisePropertyChangedFor<T>(Expression<Func<T>> property)
		{
			OnPropertyChanged(PropertyName.Of(property));
		}
	}
}
