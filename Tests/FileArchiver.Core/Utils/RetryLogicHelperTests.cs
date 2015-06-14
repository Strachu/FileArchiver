using System;

using FileArchiver.Core.Utils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Utils
{
	[TestFixture]
	public class RetryLogicHelperTests
	{
		[Test]
		public void CorrectExceptionIsPassed()
		{
			Exception caughtException = null;

			RetryLogicHelper.Try(() =>
			{
				throw new ArgumentException("Message");
			},
			exception =>
			{
				caughtException = exception;
				return RetryAction.Ignore;
			});

			Assert.That(caughtException is ArgumentException);
			Assert.That(caughtException.Message, Is.EqualTo("Message"));
		}

		[Test]
		public void WhenIgnoreIsReturnedDoNotExecuteTheActionAgain()
		{
			int timesActionDone = 0;

			RetryLogicHelper.Try(() =>
			{
				timesActionDone++;
				throw new Exception();
			},
			exception =>
			{
				return RetryAction.Ignore;
			});

			Assert.That(timesActionDone, Is.EqualTo(1));
		}

		[Test]
		public void WhenAbortIsReturnedAnOperationCanceledExceptionIsThrown()
		{
			int timesActionDone = 0;

			Assert.Throws<OperationCanceledException>(() =>
			{
				RetryLogicHelper.Try(() =>
				{
					timesActionDone++;
					throw new Exception();
				},
				exception =>
				{
					return RetryAction.Abort;
				});
			});

			Assert.That(timesActionDone, Is.EqualTo(1));
		}

		[Test]
		public void ShouldRethrowException()
		{
			int timesActionDone = 0;

			Assert.Throws<OutOfMemoryException>(() =>
			{
				RetryLogicHelper.Try(() =>
				{
					timesActionDone++;
					throw new OutOfMemoryException();
				},
				exception =>
				{
					return RetryAction.RethrowException;
				});
			});

			Assert.That(timesActionDone, Is.EqualTo(1));
		}

		[Test]
		public void ShouldDoTheOperationUntilItSucceedsIfTryIsAlwaysReturned()
		{
			int timesActionDone = 0;

			RetryLogicHelper.Try(() =>
			{
				timesActionDone++;

				if(timesActionDone == 10)
					return;

				throw new StackOverflowException();
			},
			exception =>
			{
				return RetryAction.Retry;
			});

			Assert.That(timesActionDone, Is.EqualTo(10));
		}
	}
}
