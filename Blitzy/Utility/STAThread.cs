// $Id$

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Blitzy.Utility
{
	internal class STAThread : BaseObject
	{
		#region Constructor

		private STAThread()
		{
			CanProcess = ToDispose( new AutoResetEvent( false ) );
			ActionQueue = new ConcurrentQueue<Action>();

			ThreadObject = new Thread( RunThreaded ) { Name = "STAThread" };
			ThreadObject.SetApartmentState( ApartmentState.STA );
			IsRunning = true;
			ThreadObject.Start();
		}

		#endregion Constructor

		#region Methods

		internal static void QueueAction( Action action )
		{
			if( Instance == null )
			{
				Instance = new STAThread();
			}

			Instance.ActionQueue.Enqueue( action );
			Instance.CanProcess.Set();
		}

		internal static void Stop()
		{
			if( Instance != null )
			{
				Instance.StopInternal();
				Instance.Dispose();
			}
		}

		private void RunThreaded()
		{
			while( IsRunning )
			{
				CanProcess.WaitOne();

				Action action;
				while( ActionQueue.TryDequeue( out action ) )
				{
					action.Invoke();
				}
			}
		}

		private void StopInternal()
		{
			CanProcess.Set();
			IsRunning = false;
			ThreadObject.Join();
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private static STAThread Instance;
		private readonly ConcurrentQueue<Action> ActionQueue;
		private readonly AutoResetEvent CanProcess;
		private readonly Thread ThreadObject;
		private bool IsRunning;

		#endregion Attributes
	}
}