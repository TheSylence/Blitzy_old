﻿// $Id$

using System;
using System.Threading;

namespace Blitzy.Utility
{
	/// <summary>
	/// A delegate for reporting binary progress
	/// </summary>
	/// <param name="bytesRead">The amount of bytes already read</param>
	/// <param name="totalBytesToRead">The amount of total bytes to read. Can be -1 if unknown.</param>
	public delegate void ProgressChange( long bytesRead, long totalBytesToRead );

	/// <summary>
	/// The arguments for StreamHelper.CopyFrom(Stream, Stream, CopyFromArguments)
	/// </summary>
	public sealed class CopyFromArguments
	{
		private int _BufferSize = 4096;

		private long _TotalLength = -1;
		private TimeSpan ProgressCallbackInterval = TimeSpan.FromSeconds( 0.2 );

		/// <summary>
		/// Creates the default arguments
		/// </summary>
		public CopyFromArguments()
		{
		}

		/// <summary>
		/// Creates arguments with a progress change callback.
		/// </summary>
		/// <param name="progressChangeCallback">The progress change callback (see <see cref="ProgressChangeCallback"/>)</param>
		public CopyFromArguments( ProgressChange progressChangeCallback )
		{
			ProgressChangeCallback = progressChangeCallback;
		}

		/// <summary>
		/// Creates arguments with a progress change callback and an interval between to progress changes.
		/// </summary>
		/// <param name="progressChangeCallback">The progress change callback (see <see cref="ProgressChangeCallback"/>)</param>
		/// <param name="progressChangeCallbackInterval">The interval between to progress change callbacks (see <see cref="ProgressChangeCallbackInterval"/>)</param>
		public CopyFromArguments( ProgressChange progressChangeCallback,
			TimeSpan progressChangeCallbackInterval )
		{
			ProgressChangeCallback = progressChangeCallback;
			ProgressChangeCallbackInterval = progressChangeCallbackInterval;
		}

		/// <summary>
		/// Creates arguments with a progress change callback, an interval between to progress changes and a total length
		/// </summary>
		/// <param name="progressChangeCallback">The progress change callback (see <see cref="ProgressChangeCallback"/>)</param>
		/// <param name="progressChangeCallbackInterval">The interval between to progress change callbacks (see <see cref="ProgressChangeCallbackInterval"/>)</param>
		/// <param name="totalLength">The total bytes to read (see <see cref="TotalLength"/>)</param>
		public CopyFromArguments( ProgressChange progressChangeCallback,
			TimeSpan progressChangeCallbackInterval, long totalLength )
		{
			ProgressChangeCallback = progressChangeCallback;
			ProgressChangeCallbackInterval = progressChangeCallbackInterval;
			TotalLength = totalLength;
		}

		/// <summary>
		/// Gets or sets the size of the buffer used for copying bytes. Default is 4096.
		/// </summary>
		public int BufferSize { get { return _BufferSize; } set { _BufferSize = value; } }

		/// <summary>
		/// Gets or sets the callback for progress-report. Default is null.
		/// </summary>
		public ProgressChange ProgressChangeCallback { get; set; }

		/// <summary>
		/// Gets or sets the time interval between to progress change callbacks. Default is 200 ms.
		/// </summary>
		public TimeSpan ProgressChangeCallbackInterval
		{
			get { return ProgressCallbackInterval; }
			set { ProgressCallbackInterval = value; }
		}

		/// <summary>
		/// Gets or sets the event for aborting the operation. Default is null.
		/// </summary>
		public WaitHandle StopEvent { get; set; }

		/// <summary>
		/// Gets or sets the total length of stream. Set to -1 if the value has to be determined by stream.Length.
		/// If the stream is not seekable, the total length in the progress report will be stay -1.
		/// </summary>
		public long TotalLength { get { return _TotalLength; } set { _TotalLength = value; } }
	}
}