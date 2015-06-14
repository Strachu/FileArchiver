using System.Collections.Generic;
using System.Linq;

using FileArchiver.Core.Utils;
using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.Presentation.ArchiveSettings.Framework.Windows.Forms.Controls;
using FileArchiver.Presentation.Utils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings.Framework.Windows.Forms.Controls
{
	internal class GroupBoxTests
	{
		private class TestViewModel : NotifyPropertyChangedHelper
		{
			private bool                          mVisible;
			private IEnumerable<ISettingsControl> mControls;

			public TestViewModel()
			{
				Controls     = new ISettingsControl[] {};
				GroupVisible = true;
			}

			public IEnumerable<ISettingsControl> Controls
			{
				get { return mControls; }
				set { SetFieldWithNotification(ref mControls, value); }
			}

			public bool GroupVisible
			{
				get { return mVisible; }
				set { SetFieldWithNotification(ref mVisible, value); }
			}
		}

		private class FakeControl : NotifyPropertyChangedHelper, ISettingsControl
		{
			private bool mVisible;

			public object Control
			{
				get;
				set;
			}

			public bool Visible
			{
				get { return mVisible; }
				set { SetFieldWithNotification(ref mVisible, value); }
			}
		}

		private TestViewModel mBoundViewModel;
		private GroupBox      mTestedGroupBox;

		[SetUp]
		public void SetUp()
		{
			mBoundViewModel = new TestViewModel();
			mTestedGroupBox = new GroupBox("Title", () => mBoundViewModel.Controls, () => mBoundViewModel.GroupVisible);
		}

		[Test]
		public void IsVisibleIfInitializedWithVisibleControls()
		{
			var controls = new ISettingsControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			var testedGroupBox = new GroupBox("Test", () => controls, null);

			Assert.That(testedGroupBox.Visible, Is.True);
		}

		[Test]
		public void IsHiddenIfInitializedWithHiddenControls()
		{
			var controls = new ISettingsControl[] { new FakeControl { Visible = false }, new FakeControl { Visible = false } };

			var testedGroupBox = new GroupBox("Test", () => controls, null);

			Assert.That(testedGroupBox.Visible, Is.False);
		}

		[Test]
		public void IsHiddenIfInitializedWithNullControlsCollection()
		{
			ISettingsControl[] controls = null;

			var testedGroupBox = new GroupBox("Test", () => controls, null);

			Assert.That(testedGroupBox.Visible, Is.False);
		}

		[Test]
		public void IsHiddenIfInitializedWithoutControls()
		{
			var controls = new ISettingsControl[] {};

			var testedGroupBox = new GroupBox("Test", () => controls, null);

			Assert.That(testedGroupBox.Visible, Is.False);
		}

		[Test]
		public void BecomesHiddenWhenAllInitialControlsAreHidden()
		{
			var fakeControls = new FakeControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			var testedGroupBox = new GroupBox("Test", () => fakeControls, null);

			foreach(var control in fakeControls)
			{
				control.Visible = false;
			}

			Assert.That(testedGroupBox.Visible, Is.False);
		}

		[Test]
		public void BecomesVisibleWhenSomeInitialControlsBecomesVisible()
		{
			var fakeControls = new FakeControl[] { new FakeControl { Visible = false }, new FakeControl { Visible = false } };

			var testedGroupBox = new GroupBox("Test", () => fakeControls, null);

			fakeControls.First().Visible = true;

			Assert.That(testedGroupBox.Visible, Is.True);
		}

		[Test]
		public void BecomesVisibleIfVisibleControlsAreAddedToBoundViewModel()
		{
			ForceGroupBoxHidden();

			mBoundViewModel.Controls = new ISettingsControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			Assert.That(mTestedGroupBox.Visible, Is.True);
		}

		[Test]
		public void BecomesHiddenIfControlsAreRemovedInBoundViewModel()
		{
			ForceGroupBoxVisible();

			mBoundViewModel.Controls = new ISettingsControl[] { };

			Assert.That(mTestedGroupBox.Visible, Is.False);
		}

		[Test]
		public void BecomesHiddenWhenAllControlsAreHidden()
		{
			var fakeControls = new FakeControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			mBoundViewModel.Controls = fakeControls;

			foreach(var control in fakeControls)
			{
				control.Visible = false;
			}

			Assert.That(mTestedGroupBox.Visible, Is.False);
		}

		[Test]
		public void StaysVisibleIfOnlySomeControlsAreHiddenWhileTheRestIsVisible()
		{
			var fakeControls = new FakeControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			mBoundViewModel.Controls = fakeControls;

			fakeControls.First().Visible = false;

			Assert.That(mTestedGroupBox.Visible, Is.True);
		}

		[Test]
		public void BecomesVisibleWhenAtLeastOneControlIsMadeVisible()
		{
			var fakeControls = new FakeControl[] { new FakeControl { Visible = false }, new FakeControl { Visible = false } };

			mBoundViewModel.Controls = fakeControls;

			fakeControls.First().Visible = true;

			Assert.That(mTestedGroupBox.Visible, Is.True);
		}

		[Test]
		public void BoundVisiblePropertyForcesTheGroupBoxToBeHiddenIfIsFalseDuringInitialization()
		{
			var groupBoxVisible = false;
			var fakeControls    = new FakeControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			var testedGroupBox = new GroupBox("Test", () => fakeControls, () => groupBoxVisible);

			Assert.That(testedGroupBox.Visible, Is.False);
		}

		[Test]
		public void BecomesHiddenWhenBoundVisiblePropertyIsSetToFalse()
		{
			ForceGroupBoxVisible();

			mBoundViewModel.GroupVisible = false;

			Assert.That(mTestedGroupBox.Visible, Is.False);
		}

		[Test]
		public void DoesNotBecomeVisibleWhenNewVisibleControlsAreAddedIfBoundPropertyForcesItToBeHidden()
		{
			mBoundViewModel.GroupVisible = false;

			mBoundViewModel.Controls = new ISettingsControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			Assert.That(mTestedGroupBox.Visible, Is.False);
		}

		[Test]
		public void RaisesNotificationWhenVisiblePropertyChanges()
		{
			string changedPropertyName = null;
			mTestedGroupBox.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			mBoundViewModel.Controls = new ISettingsControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };

			Assert.That(changedPropertyName, Is.EqualTo(PropertyName.Of(() => mTestedGroupBox.Visible)));
		}

		private void ForceGroupBoxHidden()
		{
			mBoundViewModel.Controls = new ISettingsControl[] { };
		}

		private void ForceGroupBoxVisible()
		{
			mBoundViewModel.Controls = new ISettingsControl[] { new FakeControl { Visible = true }, new FakeControl { Visible = true } };
		}
	}
}
