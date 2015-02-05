

using System;
using System.IO;
using System.Threading;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class SlowStream : Stream
	{
		#region Constructor

		public SlowStream( int length, int delay = 10 )
		{
			SleepDelay = delay;
			_Length = length;
			Buffer = new byte[Length];
		}

		public SlowStream( byte[] data, int delay = 10 )
		{
			SleepDelay = delay;
			_Length = data.Length;
			Buffer = data;
		}

		#endregion Constructor

		#region Methods

		public override void Flush()
		{
		}

		public override int Read( byte[] buffer, int offset, int count )
		{
			int maxCount = (int)Math.Min( Length - Position, count );

			int cnt = 0;
			for( int i = 0; i < maxCount; ++i )
			{
				buffer[i + offset] = Buffer[Position];
				Position++;
				++cnt;
				Thread.Sleep( SleepDelay );
			}

			return cnt;
		}

		public override long Seek( long offset, SeekOrigin origin )
		{
			switch( origin )
			{
				case SeekOrigin.Begin:
					Position = offset;
					break;

				case SeekOrigin.Current:
					Position += offset;
					break;

				case SeekOrigin.End:
					Position = Length - offset;
					break;
			}

			return Position;
		}

		public override void SetLength( long value )
		{
			throw new NotSupportedException();
		}

		public override void Write( byte[] buffer, int offset, int count )
		{
			int maxCount = (int)Math.Min( Length - Position, count );

			for( int i = 0; i < maxCount; ++i )
			{
				Buffer[Position] = buffer[i + offset];
				Position++;
				Thread.Sleep( SleepDelay );
			}
		}

		#endregion Methods

		#region Properties

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Length { get { return _Length; } }

		public override long Position { get; set; }

		#endregion Properties

		#region Attributes

		internal byte[] Buffer;
		private readonly long _Length;
		private readonly int SleepDelay;

		#endregion Attributes
	}
}