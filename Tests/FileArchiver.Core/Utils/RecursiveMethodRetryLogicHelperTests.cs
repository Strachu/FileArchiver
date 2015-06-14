using System;
using System.IO;

using FileArchiver.Core.Utils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Utils
{
	internal class RecursiveMethodRetryLogicHelperTests
	{
		private void RecursiveDelegateInvoker(Predicate<int> action, Func<Exception, RetryAction> exceptionHandler,
														  RecursiveMethodRetryLogicHelper tryHelper)
		{
			tryHelper.Try(() =>
			{
				bool continuee = action(0);
				if(!continuee)
					return;

				RecursiveDelegateInvoker(action, exceptionHandler, tryHelper);
			},
			exceptionHandler);
		}

		private void StartRecursiveDelegateInvoker(Predicate<int> action, Func<Exception, RetryAction> exceptionHandler)
		{
			RecursiveDelegateInvoker(action, exceptionHandler, new RecursiveMethodRetryLogicHelper());
		}

		[Test]
		public void CorrectExceptionIsPassed()
		{
			int       timesActionDone = 0;
			Exception caughtException = null;

			StartRecursiveDelegateInvoker(x =>
			{
				if(timesActionDone >= 1)
					throw new ArgumentException("Message");

				timesActionDone++;

				return true;
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

			StartRecursiveDelegateInvoker(x =>
			{
				timesActionDone++;
				if(timesActionDone == 2)
					throw new Exception();

				return true;
			},
			exception =>
			{
				return RetryAction.Ignore;
			});

			Assert.That(timesActionDone, Is.EqualTo(2));
		}

		[Test]
		public void WhenAbortIsReturnedAnOperationCanceledExceptionIsThrown()
		{
			int timesActionDone = 0;

			Assert.Throws<OperationCanceledException>(() =>
			{
				StartRecursiveDelegateInvoker(x =>
				{
					timesActionDone++;
					if(timesActionDone == 2)
						throw new Exception();

					return true;
				},
				exception =>
				{
					return RetryAction.Abort;
				});
			});

			Assert.That(timesActionDone, Is.EqualTo(2));
		}

		[Test]
		public void WhenAbortIsReturnedAnOperationCanceledExceptionIsThrownAndTheStrategyIsNotExecutedMoreThanOnceEvenInDeepRecursion()
		{
			int timesActionDone      = 0;
			int timesStrategyInvoked = 0;

			Assert.Throws<OperationCanceledException>(() =>
			{
				StartRecursiveDelegateInvoker(x =>
				{
					timesActionDone++;
					if(timesActionDone == 10)
						throw new Exception();

					return true;
				},
				exception =>
				{
					timesStrategyInvoked++;
					return RetryAction.Abort;
				});
			});

			Assert.That(timesStrategyInvoked, Is.EqualTo(1));
		}

		[Test]
		public void ShouldRethrowException()
		{
			int timesActionDone = 0;

			Assert.Throws<OutOfMemoryException>(() =>
			{
				StartRecursiveDelegateInvoker(x =>
				{
					timesActionDone++;
					if(timesActionDone == 2)
						throw new OutOfMemoryException();

					return true;
				},
				exception =>
				{
					return RetryAction.RethrowException;
				});
			});

			Assert.That(timesActionDone, Is.EqualTo(2));
		}

		[Test]
		public void ShouldRethrowExceptionAndTheStrategyIsNotExecutedMoreThanOnceEvenInDeepRecursion()
		{
			int timesActionDone      = 0;
			int timesStrategyInvoked = 0;

			Assert.Throws<OutOfMemoryException>(() =>
			{
				StartRecursiveDelegateInvoker(x =>
				{
					timesActionDone++;
					if(timesActionDone == 10)
						throw new OutOfMemoryException();

					return true;
				},
				exception =>
				{
					timesStrategyInvoked++;
					return RetryAction.RethrowException;
				});
			});

			Assert.That(timesStrategyInvoked, Is.EqualTo(1));
		}

		[Test]
		public void ShouldDoTheOperationUntilItSucceedsIfTryIsAlwaysReturned()
		{
			int timesActionDone = 0;

			StartRecursiveDelegateInvoker(x =>
			{
				timesActionDone++;

				if(timesActionDone == 10)
					throw new IOException();

				return (timesActionDone < 20);
			},
			exception =>
			{
				return RetryAction.Retry;
			});

			Assert.That(timesActionDone, Is.EqualTo(20));
		}
	}
}
