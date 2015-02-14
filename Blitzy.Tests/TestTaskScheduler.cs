using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class TestTaskScheduler : TaskScheduler
	{
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return Enumerable.Empty<Task>();
		}

		protected override void QueueTask( Task task )
		{
			TryExecuteTask( task );
		}

		protected override bool TryExecuteTaskInline( Task task, bool taskWasPreviouslyQueued )
		{
			return TryExecuteTask( task );
		}

		public override int MaximumConcurrencyLevel { get { return 1; } }
	}
}