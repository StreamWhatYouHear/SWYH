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
using OpenSource.UPnP.AV;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Enumerates the known storage mediums and allows
	/// easy translation of the enumeration into
	/// string values through an indexer.
	/// </summary>
	[Serializable()]
	public class StorageMediums
	{
		/// <summary>
		/// Complete enumeration of possible storage medium values.
		/// </summary>
		public enum Enum
		{
			UNKNOWN,
			DV,
			MINI_DV,
			VHS,
			W_VHS,
			S_VHS,
			D_VHS,
			VHSC,
			VIDEO8,
			HI8,
			CD_ROM,
			CD_DA,
			CD_R,
			CD_RW,
			VIDEO_CD,
			SACD,
			MD_AUDIO,
			MD_PICTURE,
			DVD_ROM,
			DVD_R,
			DVD_PLUS_RW,
			DVD_MINIS_RW,
			DVD_RAM
		}

		/// <summary>
		/// Translates an storage medium enumeration value into a string.
		/// </summary>
		public string this[Enum val]
		{
			get
			{
				string retVal = "";
				switch (val)
				{
					case Enum.CD_DA:
					case Enum.CD_R:
					case Enum.CD_ROM:
					case Enum.CD_RW:
					case Enum.D_VHS:
					case Enum.DVD_R:
					case Enum.DVD_RAM:
					case Enum.DVD_ROM:
					case Enum.MD_AUDIO:
					case Enum.MD_PICTURE:
					case Enum.MINI_DV:
					case Enum.S_VHS:
					case Enum.VIDEO_CD:
					case Enum.W_VHS:
						retVal = val.ToString().Replace("_", "-");
						break;

					case Enum.DV:
					case Enum.HI8:
					case Enum.SACD:
					case Enum.UNKNOWN:
					case Enum.VHS:
					case Enum.VHSC:
					case Enum.VIDEO8:
						retVal = val.ToString();
						break;

					case Enum.DVD_MINIS_RW:
						retVal = "DVD-RW";
						break;
					case Enum.DVD_PLUS_RW:
						retVal = "DVD+RW";
						break;

					default:
						throw new Exception("unknown StorageMedium");
				}

				return retVal;
			}
		}
	}

}
