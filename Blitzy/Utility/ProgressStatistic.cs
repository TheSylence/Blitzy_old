using System;
using System.Collections.Generic;

namespace Blitzy.Utility
{
	/// <summary>
	/// A class which calculates progress statistics like average bytes per second or estimated finishing time.
	/// To use it, call the ProgressChange method in regular intervals with the actual progress.
	/// </summary>
	public class ProgressStatistic
	{
		public ProgressStatistic()
		{
			StartingTime = DateTime.MinValue;
			FinishingTime = DateTime.MinValue;

			ProgressChangedArgs = new ProgressEventArgs( this ); //Event args can be cached
		}

		#region Methods

		/// <summary>
		/// This method can be called to finish an aborted operation.
		/// If the operation does not reach 100%, "Finished" will be never raised, so this method should be called.
		/// </summary>
		public virtual void Finish()
		{
			if( !HasFinished )
			{
				FinishingTime = DateTime.Now;
				OnFinished();
			}
		}

		/// <summary>
		/// This method can be called to report progress changes.
		/// The signature of this method is compliant with the ProgressChange-delegate
		/// </summary>
		/// <param name="bytesRead">The amount of bytes already read</param>
		/// <param name="totalBytesToRead">The amount of total bytes to read. Can be -1 if unknown.</param>
		/// <exception cref="ArgumentException">Thrown if bytesRead has not changed or even shrunk.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the operation has finished already.</exception>
		public virtual void ProgressChange( long bytesRead, long totalBytesToRead )
		{
			if( bytesRead <= BytesRead )
				throw new ArgumentException( @"Operation cannot go backwards!", "bytesRead" );

			if( HasFinished )
				throw new InvalidOperationException( "Operation has finished already!" );

			if( !_HasStarted )
			{
				StartingTime = DateTime.Now;
				_HasStarted = true;
				OnStarted();
			}

			BytesRead = bytesRead;
			TotalBytesToRead = totalBytesToRead;

			ProcessSample( bytesRead );

			OnProgressChanged();

			if( bytesRead == TotalBytesToRead )
			{
				FinishingTime = DateTime.Now;
				OnFinished();
			}
		}

		#endregion Methods

		#region Properties

		/// <summary>
		/// Gets the average bytes per second.
		/// </summary>
		public double AverageBytesPerSecond { get { return BytesRead / Duration.TotalSeconds; } }

		/// <summary>
		/// Gets the amount of bytes already read.
		/// </summary>
		public long BytesRead { get; private set; }

		/// <summary>
		/// Gets whether the operation has finished
		/// </summary>
		public bool HasFinished { get { return FinishingTime != DateTime.MinValue; } }

		/// <summary>
		/// Gets whether the operation has started
		/// </summary>
		public bool HasStarted { get { return _HasStarted; } }

		/// <summary>
		/// Gets whether the operation is still running
		/// </summary>
		public bool IsRunning { get { return HasStarted && !HasFinished; } }

		#region Time

		/// <summary>
		/// The method which will be used for estimating duration and finishing time
		/// </summary>
		public enum EstimatingMethod
		{
			/// <summary>
			/// Current bytes per second will be used for estimating.
			/// </summary>
			CurrentBytesPerSecond,

			/// <summary>
			/// Average bytes per second will be used for estimating
			/// </summary>
			AverageBytesPerSecond
		}

		/// <summary>
		/// Gets the duration of the operation.
		/// If the operation is still running, the time since starting is returned.
		/// If the operation has not started, TimeSpan.Zero is returned.
		/// If the operation has finished, the time between starting and finishing is returned.
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				if( !HasStarted )
					return TimeSpan.Zero;

				if( !HasFinished )
					return DateTime.Now - StartingTime;

				return FinishingTime - StartingTime;
			}
		}

		/// <summary>
		/// Gets the estimated duration. Use UsedEstimatingMethod to specify which method will be used for estimating.
		/// If the operation will take more than 200 days, TimeSpan.MaxValue is returned.
		/// </summary>
		public TimeSpan EstimatedDuration
		{
			get
			{
				if( HasFinished )
					return Duration;
				if( TotalBytesToRead == -1 )
					return TimeSpan.MaxValue;

				double bytesPerSecond = 1;
				if( UsedEstimatingMethod == EstimatingMethod.AverageBytesPerSecond )
					bytesPerSecond = AverageBytesPerSecond;
				else if( UsedEstimatingMethod == EstimatingMethod.CurrentBytesPerSecond )
					bytesPerSecond = CurrentBytesPerSecond;

				double seconds = ( TotalBytesToRead - BytesRead ) / bytesPerSecond;
				if( seconds > 60 * 60 * 24 * 200 ) //over 200 Days -> infinite
				{
					return TimeSpan.MaxValue;
				}

				return Duration + TimeSpan.FromSeconds( seconds );
			}
		}

		/// <summary>
		/// Gets the estimated finishing time based on EstimatedDuration.
		/// If the operation will take more than 200 days, DateTime.MaxValue is returned.
		/// If the operation has finished, the actual finishing time is returned.
		/// </summary>
		public DateTime EstimatedFinishingTime
		{
			get
			{
				if( EstimatedDuration == TimeSpan.MaxValue )
					return DateTime.MaxValue;
				return StartingTime + EstimatedDuration;
			}
		}

		/// <summary>
		/// Gets the date time when the operation has finished
		/// </summary>
		public DateTime FinishingTime { get; private set; }

		/// <summary>
		/// Gets the date time when the operation has started
		/// </summary>
		public DateTime StartingTime { get; private set; }

		/// <summary>
		/// Gets or sets which method will be used for estimating.
		/// Can only be set before the operation has started, otherwise an OperationAlreadyStartedException will be thrown.
		/// </summary>
		public EstimatingMethod UsedEstimatingMethod
		{
			get { return _EstimatingMethod; }
			set
			{
				if( HasStarted )
					throw new OperationAlreadyStartedException();
				_EstimatingMethod = value;
			}
		}

		private EstimatingMethod _EstimatingMethod = EstimatingMethod.CurrentBytesPerSecond;

		#endregion Time

		/// <summary>
		/// Gets the progress in percent between 0 and 1.
		/// If the amount of total bytes to read is unknown, -1 is returned.
		/// </summary>
		public double Progress
		{
			get
			{
				if( TotalBytesToRead == -1 )
				{
					return -1;
				}

				return BytesRead / (double)TotalBytesToRead;
			}
		}

		/// <summary>
		/// Gets the amount of total bytes to read. Can be -1 if unknown.
		/// </summary>
		public long TotalBytesToRead { get; private set; }

		#region CurrentBytesPerSecond

		//current sample index in currentBytesSamples
		private void ProcessSample( long bytes )
		{
			if( ( DateTime.Now - LastSample ).Ticks > CurrentBytesCalculationInterval.Ticks / CurrentBytesSamples.Length )
			{
				LastSample = DateTime.Now;

				KeyValuePair<DateTime, long> current = new KeyValuePair<DateTime, long>( DateTime.Now, bytes );

				var old = CurrentBytesSamples[CurrentSample];
				CurrentBytesSamples[CurrentSample] = current;

				if( old.Key == DateTime.MinValue )
				{
					CurrentBytesPerSecond = AverageBytesPerSecond;
				}
				else
				{
					CurrentBytesPerSecond = ( current.Value - old.Value ) / ( current.Key - old.Key ).TotalSeconds;
				}

				CurrentSample++;
				if( CurrentSample >= CurrentBytesSamples.Length )
					CurrentSample = 0;
			}
		}

		/// <summary>
		/// Gets or sets the interval used for the calculation of the current bytes per second. Default is 500 ms.
		/// </summary>
		/// <exception cref="OperationAlreadyStartedException">
		/// Thrown when trying to set although the operation has already started.</exception>
		public TimeSpan CurrentBytesCalculationInterval
		{
			get { return _CurrentBytesCalculationInterval; }
			set
			{
				if( HasStarted )
					throw new InvalidOperationException( "Task has already started!" );
				_CurrentBytesCalculationInterval = value;
			}
		}

		/// <summary>
		/// Gets the approximated current count of bytes processed per second
		/// </summary>
		public double CurrentBytesPerSecond { get; private set; }

		/// <summary>
		/// Gets or sets the number of samples in CurrentBytesPerSecondInterval used for current bytes per second approximation
		/// </summary>
		/// <exception cref="OperationAlreadyStartedException">
		/// Thrown when trying to set although the operation has already started.</exception>
		public int CurrentBytesSampleCount
		{
			get { return CurrentBytesSamples.Length; }
			set
			{
				if( HasStarted )
					throw new InvalidOperationException( "Task has already started!" );
				if( value != CurrentBytesSamples.Length )
				{
					CurrentBytesSamples = new KeyValuePair<DateTime, long>[value];
				}
			}
		}

		private TimeSpan _CurrentBytesCalculationInterval = TimeSpan.FromSeconds( 0.5 );

		private KeyValuePair<DateTime, long>[] CurrentBytesSamples = new KeyValuePair<DateTime, long>[6];

		private int CurrentSample;

		private DateTime LastSample;

		#endregion CurrentBytesPerSecond

		#endregion Properties

		#region Events

		/// <summary>
		/// Will be raised when the operation has finished
		/// </summary>
		public event EventHandler<ProgressEventArgs> Finished;

		/// <summary>
		/// Will be raised when the progress has changed
		/// </summary>
		public event EventHandler<ProgressEventArgs> ProgressChanged;

		/// <summary>
		/// Will be raised when the operation has started
		/// </summary>
		public event EventHandler<ProgressEventArgs> Started;

		protected virtual void OnFinished()
		{
			if( Finished != null )
				Finished( this, ProgressChangedArgs );
		}

		protected virtual void OnProgressChanged()
		{
			if( ProgressChanged != null )
				ProgressChanged( this, ProgressChangedArgs );
		}

		protected virtual void OnStarted()
		{
			if( Started != null )
				Started( this, ProgressChangedArgs );
		}

		private readonly ProgressEventArgs ProgressChangedArgs;

		#endregion Events

		#region Attributes

		private bool _HasStarted;

		#endregion Attributes
	}
}