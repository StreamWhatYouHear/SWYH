using System;
using System.IO;
using System.Threading;

namespace Intel.UPNP
{
	/// <summary>
	/// Summary description for CircleStream.
	/// </summary>
	public sealed class CircleStream : Stream
	{
		private byte[] buffer = new byte[32768];
		private int ReadPointer = 0;
		private int WritePointer = 0;
		private int AvailableData = 0;

		private ManualResetEvent ReadEvent = new ManualResetEvent(false);
		
		public CircleStream()
		{
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] InBuffer, int offset, int count)
		{
			return(0);
		}
		public override void Write(byte[] InBuffer, int offset, int count)
		{
		}

		public override bool CanRead
		{
			get
			{
				return(true);
			}
		}
		public override bool CanWrite
		{
			get
			{
				return(true);
			}
		}
		public override bool CanSeek
		{
			get
			{
				return(false);
			}
		}
		public override long Position
		{
			get
			{
				throw(new NotSupportedException());
			}
			set
			{
				throw(new NotSupportedException());
			}
		}
		public override long Length
		{
			get
			{
				throw(new NotSupportedException());
			}
		}
		public override void SetLength(long DesiredLength)
		{
		}
		public override long Seek(long SeekPosition, SeekOrigin Origin)
		{
			throw(new NotSupportedException());
		}

	}
}
