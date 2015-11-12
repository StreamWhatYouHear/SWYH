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
using System.Xml;
using System.Text;
using System.Reflection;
using System.Collections;
using OpenSource.UPnP;
using OpenSource.Utilities;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This interface is designed to represent the actual object/values
	/// that are retrieved from the <see cref="MediaProperties"/>
	/// object. Examples of elements include upnp:class, dc:title, upnp:artist, etc.
	/// </para>
	/// <para>
	/// Every class that implements this interface must also
	/// implement a constructor with the signature.
	/// "public class (XmlElement)", such that the object can be 
	/// instantiated directly from XML. This allows 
	/// <see cref="MediaObject"/>
	/// the ability to instantiate object representations of
	/// values that are presented in a DIDL-Lite fragment.
	/// The mapping between DIDL-Lite elements and the actual
	/// objects can be found in 
	/// <see cref="PropertyMappings"/>
	/// </para>
	/// <para>
	/// REQUIREMENT!!!!
	/// Every class that implements this interface should also provide a static
	/// method called GetPossibleAttributes(), which has the exact same return
	/// values as the PossibleAttributes method.
	/// </para>
	/// </summary>
	public interface ICdsElement : IComparable
	{
		/// <summary>
		/// Returns a list of possible attributes that are applicable
		/// to this type of element. If no attributes are allowed,
		/// then it should return null. 
		/// </summary>
		IList PossibleAttributes { get; }

		/// <summary>
		/// Returns the list of attributes that are actually present
		/// for this element. Many attributes in the DIDL-Lite
		/// fragments are optional.
		/// </summary>
		IList ValidAttributes { get; }

		/// <summary>
		/// Returns a raw untyped value. This object
		/// is often a .NET native value or object.
		/// For example, date and time values will often
		/// be wrapped in a .NET DateTime object.
		/// </summary>
		object Value { get; }

		/// <summary>
		/// Returns an object representing the value as either
		/// a .NET primitive (int, uint, string, etc.) or as an
		/// other object that implements IComparable.
		/// </summary>
		IComparable ComparableValue { get; }

		/// <summary>
		/// Returns a string representation of the value,
		/// in cases where the Value.ToString() will
		/// not suffice.
		/// </summary>
		string StringValue { get; }

		/// <summary>
		/// Extracts the value of an attribute. If no
		/// attribute is present, then the value returned
		/// is null.
		/// </summary>
		/// <param name="attribute">attribute name</param>
		/// <returns>returns a comparable value</returns>
		IComparable ExtractAttribute(string attribute);

		/// <summary>
		/// <para>
		/// Prints an XML representation of this metadata element.
		/// Assume the caller desires that the metadata element should
		/// be printed, so as to not require implementations to always
		/// check whether the element should actually be printed.
		/// </para>
		/// </summary>
		/// <param name="formatter">
		/// Provides delegate references that provide the implementation
		/// for actually priting this CDS metadata element.
		/// </param>
		/// <param name="data">
		/// Implementation-specific data and instructions that can be
		/// passed as an argument to the <see cref="ToXmlFormatter.Method"/>
		/// fields in theh "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that is responsible for
		/// printing the XML representation of the media object.
		/// </param>
		void ToXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);
	}
}
