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
using System.Net;
using System.Text;
using System.Collections;
using OpenSource.UPnP.AV;
using System.Text.RegularExpressions;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// An efficient IMediaComparer that will never match any object.
	/// </summary>
	public sealed class MatchOnNever : IMediaComparer
	{
		/// <summary>
		/// Always returns false.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch (IUPnPMedia mediaObject)
		{
			return false;
		}
	}

	/// <summary>
	/// An efficient IMediaComparer that will always match any object.
	/// </summary>
	public sealed class MatchOnAny : IMediaComparer
	{
		/// <summary>
		/// Always returns true.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch (IUPnPMedia mediaObject)
		{
			return true;
		}
	}

	/// <summary>
	/// An efficient IMediaComparer that simply tests whether
	/// a media object is a container.
	/// </summary>
	public sealed class MatchOnContainers : IMediaComparer
	{
		/// <summary>
		/// Returns true if the media object is a container.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch(IUPnPMedia mediaObject)
		{
			return (mediaObject.IsContainer == true);
		}
	}

	/// <summary>
	/// An efficient IMediaComparer that simply tests whether
	/// a media object is an item.
	/// </summary>
	public sealed class MatchOnItem : IMediaComparer
	{
		/// <summary>
		/// Returns true if the media object is a container.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch(IUPnPMedia mediaObject)
		{
			return (mediaObject.IsItem == true);
		}
	}

	/// <summary>
	/// An efficient IMediaComparer that simply tests whether
	/// a media object is an item that points to another item.
	/// </summary>
	public sealed class MatchOnItemRef : IMediaComparer
	{
		/// <summary>
		/// Returns true if the media object is a container.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch(IUPnPMedia mediaObject)
		{
			return (mediaObject.IsReference == true);
		}
	}

	/// <summary>
	/// An efficient IMediaComparer that simply tests whether
	/// a media object is of a particular class or derived
	/// from a particular class.
	/// </summary>
	public sealed class MatchOnClass : IMediaComparer
	{
		/// <summary>
		/// Must indicate the media class and whether derived media class
		/// types should also be included as matches.
		/// </summary>
		/// <param name="mediaClass"></param>
		/// <param name="includeDerived"></param>
		public MatchOnClass (MediaClass mediaClass, bool includeDerived)
		{
			this.m_Class = mediaClass;
			this.m_IncludeDerived = includeDerived;
		}

		/// <summary>
		/// Returns true if the media object matches the specified class type.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		public bool IsMatch(IUPnPMedia mediaObject)
		{
			if (this.m_IncludeDerived)
			{
				return (mediaObject.Class.IsA(this.m_Class));
			}
			else
			{
				return (string.Compare(mediaObject.Class.ToString(), this.m_Class.ToString(), true) == 0);
			}
		}

		/// <summary>
		/// Returns the media that the object will match on.
		/// </summary>
		public MediaClass Class
		{
			get
			{
				return this.m_Class;
			}
		}

		/// <summary>
		/// Indicates if derived classes will also match.
		/// </summary>
		public bool IncludeDerived
		{
			get
			{
				return this.m_IncludeDerived;
			}
		}

		private MediaClass m_Class;
		private bool m_IncludeDerived;
	}

}
