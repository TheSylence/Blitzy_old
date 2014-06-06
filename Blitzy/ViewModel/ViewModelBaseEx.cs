// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GalaSoft.MvvmLight;
using log4net;

namespace Blitzy.ViewModel
{
	public class ViewModelBaseEx : ViewModelBase, IRequestCloseViewModel, IDisposable
	{
		#region Constructor

		public ViewModelBaseEx()
		{
			Log = LogManager.GetLogger( GetType() );
			IsDisposed = false;
			ObjectsToDispose = new Stack<IDisposable>();
		}

		#endregion Constructor

		#region Disposable

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ViewModelBaseEx"/> is reclaimed by garbage collection.
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Only for debugging purposes" )]
		~ViewModelBaseEx()
		{
#if DEBUG
			LogDebug( "Finalizer called on object: {0}", this );
#endif

			Dispose( false );
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
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

		#endregion Disposable

		#region Methods

		#region Logging

		protected void LogDebug( string format, params object[] args )
		{
#if DEBUG
			if( Log.IsDebugEnabled )
			{
				Log.DebugFormat( CultureInfo.InvariantCulture, format, args );
			}
#endif
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

		#endregion Logging

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
			ObjectsToDispose.Push( obj );
			return obj;
		}

		#endregion Methods

		#region Properties

		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed { get; protected set; }

		#endregion Properties

		#region Attributes

		internal Stack<IDisposable> ObjectsToDispose;
		protected ILog Log;
		private bool MessagesRegistered;

		#endregion Attributes

		#region Events

		public event EventHandler<CloseViewEventArgs> RequestClose;

		public event EventHandler<EventArgs> RequestHide;

		public event EventHandler<EventArgs> RequestShow;

		#endregion Events
	}
}