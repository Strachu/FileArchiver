using System.Threading;

namespace FileArchiver.TestUtils
{
	/// <summary>
	/// A synchronization context which does nothing except just executing operations synchronously on current thread.
	/// </summary>
	internal class NullSynchronizationContext : SynchronizationContext
	{
		public override void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			d(state);
		}
	}
}
