using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using log4net;

namespace Blitzy.ViewModel
{
	public class ViewModelBaseEx : ViewModelBase, IRequestCloseViewModel, IDisposable
	{
		public ViewModelBaseEx( DbConnectionFactory connectionFactory, ViewServiceManager serviceManager = null, IMessenger messenger = null )
		{
			StoreCreationStack();

			if( messenger != null )
			{
				MessengerInstance = messenger;
			}

			ServiceManagerInstance = serviceManager ?? ViewServiceManager.Default;
			ConnectionFactory = connectionFactory;

			Log = LogManager.GetLogger( GetType() );
			IsDisposed = false;
			ObjectsToDispose = new Stack<IDisposable>();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ViewModelBaseEx"/> is reclaimed by garbage collection.
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Only for debugging purposes" )]
		~ViewModelBaseEx()
		{
			LogFinalizer();

			Dispose( false );
		}

		public event EventHandler<CloseViewEventArgs> RequestClose;

		public event EventHandler<EventArgs> RequestHide;

		public event EventHandler<EventArgs> RequestShow;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		public virtual void Reset()
		{
			if( !MessagesRegistered )
			{
				RegisterMessages();
				MessagesRegistered = true;
			}
		}

		protected void Close( bool? result = null )
		{
			if( RequestClose != null )
			{
				RequestClose( this, new CloseViewEventArgs( result ) );
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose( bool managed )
		{
			if( !IsDisposed )
			{
				if( managed )
				{
					while( ObjectsToDispose.Count > 0 )
					{
						ObjectsToDispose.Pop().Dispose();
					}
				}

				IsDisposed = true;
			}
		}

		protected void DisposeObject( IDisposable obj )
		{
			IDisposable onStack = null;
			Queue<IDisposable> tmpQueue = new Queue<IDisposable>();

			do
			{
				if( onStack != null )
				{
					tmpQueue.Enqueue( onStack );
				}

				onStack = ObjectsToDispose.Pop();
			} while( !Equals( onStack, obj ) );

			onStack.Dispose();

			while( tmpQueue.Count > 0 )
			{
				ObjectsToDispose.Push( tmpQueue.Dequeue() );
			}
		}

		protected void Hide()
		{
			if( RequestHide != null )
			{
				RequestHide( this, EventArgs.Empty );
			}
		}

		[Conditional( "DEBUG" )]
		protected void LogDebug( string format, params object[] args )
		{
			if( Log.IsDebugEnabled )
			{
				Log.DebugFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogError( string format, params object[] args )
		{
			if( Log.IsErrorEnabled )
			{
				Log.ErrorFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogFatal( string format, params object[] args )
		{
			if( Log.IsFatalEnabled )
			{
				Log.FatalFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogInfo( string format, params object[] args )
		{
			if( Log.IsInfoEnabled )
			{
				Log.InfoFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogWarning( string format, params object[] args )
		{
			if( Log.IsWarnEnabled )
			{
				Log.WarnFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected virtual void RegisterMessages()
		{
		}

		protected void Show()
		{
			if( RequestShow != null )
			{
				RequestShow( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Adds a new object to the internal container of disposable objects.
		/// </summary>
		/// <typeparam name="T">Type of the object to add. Must implement IDisposable</typeparam>
		/// <param name="obj">The object to add.</param>
		/// <returns>The added objects</returns>
		protected T ToDispose<T>( T obj ) where T : IDisposable
		{
			if( obj == null )
			{
				throw new ArgumentNullException( "obj" );
			}

			ObjectsToDispose.Push( obj );
			return obj;
		}

		[Conditional( "DEBUG" )]
		private void LogFinalizer()
		{
			LogDebug( "Finalizer called on object: {0}", this );
		}

		[Conditional( "DEBUG" )]
		private void StoreCreationStack()
		{
			CreationStack = new StackTrace( true );
		}

		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed { get; protected set; }

		public TaskScheduler TaskScheduler
		{
			get { return _TaskScheduler ?? TaskScheduler.Default; }
			set { _TaskScheduler = value; }
		}

		protected DbConnectionFactory ConnectionFactory { get; private set; }

		protected ViewServiceManager ServiceManagerInstance { get; private set; }

		internal Stack<IDisposable> ObjectsToDispose;

		protected ILog Log;

		// For testing purposes
		private TaskScheduler _TaskScheduler;

		private StackTrace CreationStack;
		private bool MessagesRegistered;
	}
}