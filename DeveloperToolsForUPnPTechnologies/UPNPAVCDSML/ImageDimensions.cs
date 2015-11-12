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
using System.Text;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// UPNP ContentDirectory has a unique way of representing image dimensions,
	/// and the values associated with it are value types so this class
	/// was created to meet the needs of representing a more complex value type.
	/// </summary>
	[Serializable()]
	public struct ImageDimensions : IValueType
	{
		/// <summary>
		/// The compare method takes the width*height and compares
		/// to a numerical value (obj) or another ImageDimension
		/// object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			System.Type type = obj.GetType();

			int size = this.Height * this.Width;
			if (type == typeof(ImageDimensions))
			{
				ImageDimensions d = (ImageDimensions) obj;
				
				int size2 = d.Height * d.Width;
				return size.CompareTo(size2);
			}
			
			return size.CompareTo(obj);
		}
		
		/// <summary>
		/// Returns string value
		/// </summary>
		public object Value { get { return this.ToString(); } }
		/// <summary>
		/// Max of 9999
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// Max of 9999
		/// </summary>
		public readonly int Height;
		/// <summary>
		/// Set this to true to indicate that a value exists
		/// </summary>
		private bool m_IsValid;
		/// <summary>
		/// Returns true if the image dimensions are actually
		/// present.
		/// </summary>
		public bool IsValid { get { return this.m_IsValid; } }

		/// <summary>
		/// WWWWxHHHH, where W is width digit, H is height digit, x is delimiter
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			//CDS spec formerly requirex 4-digit lengths
			//sb.AppendFormat("{0}x{1}", Width.ToString("0000"), Height.ToString("0000"));
			sb.AppendFormat("{0}x{1}", this.Width, this.Height);
			return sb.ToString();
		}
		/// <summary>
		/// Expects another ImageDimension object; returns false 
		/// object does not qualify. Compares
		/// width and height. 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ImageDimensions d2 = (ImageDimensions) obj;
			if (this.Height == d2.Height)
			{
				if (this.Width == d2.Width)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Takes the IsValid, Width, and Height
		/// and multiplies each by a prime number
		/// and sums them all for a hash value.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (IsValid.GetHashCode()*2) + (Width*3) + (Height*5);
		}

		/// <summary>
		/// Instantiate an ImageDimension object
		/// with valid width and height parameters.
		/// Keep in mind that ContentDirectory has
		/// a strange 4 digit limit for each.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <exception cref="Error_InvalidRangeForDimensions">
		/// Thrown when an axis has a dimension greater than 9999.
		/// </exception>
		public ImageDimensions(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this.m_IsValid = true;
			this.CheckDimensions();
		}

		/// <summary>
		/// Instantiates an unsigned dimension.
		/// </summary>
		/// <param name="invalid"></param>
		public ImageDimensions(bool invalid)
		{
			this.Height = 0;
			this.Width = 0;
			this.m_IsValid = false;
		}

		/// <summary>
		/// Instantiages an ImageDimension, given its ContentDirectory
		/// specified string representation.
		/// </summary>
		/// <param name="wXh">must be in form WWWWxHHHH</param>
		/// <exception cref="Error_InvalidRangeForDimensions">
		/// DEPRECATED: Thrown when an axis has a dimension greater than 9999.
		/// </exception>
		public ImageDimensions(string wXh)
		{
			wXh = wXh.ToLower();
			int posX = wXh.IndexOf("x");
			
			this.Width = int.Parse(wXh.Substring(0, posX));
			this.Height = int.Parse(wXh.Substring(posX+1));
			this.m_IsValid = true;
			this.CheckDimensions();
		}

		/// <summary>
		/// DEPRECATED:
		/// Throws an exception if the dimensions exceed
		/// CDS specifications.
		/// </summary>
		private void CheckDimensions()
		{
			/*
			if ((Width > 9999) || (Height > 9999))
			{
				throw new Error_InvalidRangeForDimensions(Width, Height);
			}
			*/		
		}

	}
}
