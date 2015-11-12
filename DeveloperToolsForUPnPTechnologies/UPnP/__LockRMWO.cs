/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Threading;

namespace OpenSource.UPnP
{
	/// <summary>
	/// This is a synchronization object (that should have a better name
	/// but I couldn't think of the real term).
	/// 
	/// In any case, it allows many simultaneous "read" operations
	/// but only write operation at any time.
	/// 
	/// The read and write operations should span large chunks of code.
	/// It may not be efficient to use this class if you want to guard
	/// really small code blocks. This class uses a Mutex, which
	/// forces a small degree of blocking for any operation,
	/// including read ops.
	/// 
	/// If a client issues a write op:
	///		- all subsequent read/write ops are blocked/queued,
	///		- write op blocks until all existing read-ops complete,
	///		- write op blocks until all previous write ops complete
	///		- then the write op is done
	///		- then read or write ops continue
	///		
	///	If a client issues a read op:
	///		- all subsequent write ops are blocked
	///		- read op blocks until write op is complete
	///		- read op blocks until 
	/// </summary>
	public class LockRMWO
	{
		private System.Threading.ReaderWriterLock RWLock = new ReaderWriterLock();
		/// <summary>
		/// Starts a read operation if no write operations are being serviced.
		/// Otherwise, the write operation is queued. 
		/// 
		/// A successive write operation is queued until all pending read
		/// operations are considered complete through EndRead().
		/// 
		/// A successive read operation is allowed to execute simultaneously.
		/// </summary>
		public void StartRead()
		{
			RWLock.AcquireReaderLock(30000);
		}

		/// <summary>
		/// Finishes a read operation and allows write operations
		/// to begin if no read operations are still being
		/// serviced.
		/// </summary>
		public void EndRead()
		{
			RWLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Starts a write operation if no read ops are being serviced.
		/// Queues a write operation if a read is still in progress.
		/// 
		/// A successive write operation is queued until all pending read/write 
		/// operations are considered completed.
		/// 
		/// A successive read operation is queued.
		/// </summary>
		public void StartWrite()
		{
			RWLock.AcquireWriterLock(30000);
		}


		/// <summary>
		/// Completes a write operation
		/// </summary>
		public void EndWrite()
		{
			RWLock.ReleaseWriterLock();
		}
	}

}
