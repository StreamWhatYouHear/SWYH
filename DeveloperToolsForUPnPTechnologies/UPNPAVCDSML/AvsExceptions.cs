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

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Thrown when the SearchCriteria argument in ContentDirectory Search
	/// requests is malformed.
	/// </summary>
	public class Error_MalformedSearchCriteria : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 402.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_MalformedSearchCriteria (string info) : base (402, "Malformed SearchCriteria expression. " + info) {}
	}
	

	/// <summary>
	/// Thrown when calling a control point calls PrepareForConnection and 
	/// the direction of the connection is incorrect. This is only used
	/// for UPnP MediaServer's that have PrepareForConnection().
	/// </summary>
	public class Error_InvalidDirection : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 402.
		/// TODO: Does this need to be changed to a different error code?
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_InvalidDirection (string info) : base (402, "Invalid direction. " + info) {}
	}

	/// <summary>
	/// Indicates that the value used in an operation was invalid.
	/// </summary>
	public class Error_InvalidValue : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP error code 412.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="val"></param>
		public Error_InvalidValue (string info, object val)
			: base (412, info)
		{
			Value = val;
		}

		/// <summary>
		/// The value used in the operation.
		/// </summary>
		object Value;
	}

	/// <summary>
	/// Thrown when a control point calls PrepareForConnection on a mediaserver when the
	/// protocol info string is not supported.
	/// </summary>
	public class Error_IncompatibleProtocolInfo : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 701.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_IncompatibleProtocolInfo (string info) : base (701, "Incompatible ProtocolInfo. " + info) {}
	}
	
	/// <summary>
	/// Thrown when a control point specifies an ID for a media object that doesn't exist
	/// in the content hierarchy of a media server.
	/// </summary>
	public class Error_NoSuchObject : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 701.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_NoSuchObject(string info) : base (701, "No such object. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point uses UpdateObject for modifying an XML element in ContentDirectory entry
	/// when the specified XML element for modification is invalid.
	/// </summary>
	public class Error_InvalidCurrentTagValue : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 702.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_InvalidCurrentTagValue(string info) : base (702, "Invalid current tag value. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point calls PrepareForConnection on the media server 
	/// and the number of connections is exceeded.
	/// </summary>
	public class Error_MaximumConnectionsExceeded : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 704.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_MaximumConnectionsExceeded (string info) : base (704, "Maximum number of connections exceeded. " + info) {}
	}
	
	/// <summary>
	/// Thrown when a control point uses UpdateObject for deleting an XML element in a ContentDirectory entry
	/// when the XML element is a required element.
	/// </summary>
	public class Error_RequiredTag : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 704.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_RequiredTag (string info) : base (704, "Required tag. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point uses UpdateObject for modifying XML element in a ContentDirectory entry
	/// when the XML element is a read-only element.
	/// </summary>
	public class Error_ReadOnlyTag : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 705.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_ReadOnlyTag(string info) : base (705, "Read only tag. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point calls ConnectionComplete and specifies a ConnectionID
	/// that doesn't exist on the MediaServer.
	/// </summary>
	public class Error_InvalidConnection : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 706.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_InvalidConnection(string info) : base (706, "Invalid connection reference. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point uses UpdateObject for modifying an XML element in a ContentDirectory entry
	/// when the number of proposed elements for changing does not match the number
	/// of new values.
	/// </summary>
	public class Error_ParameterMismatch : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 706.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_ParameterMismatch (string info) : base (706, "Parameter mismatch. " + info) {}
	}

	/// <summary>
	/// Thrown if a specified ContentDirectory XML element attribute is not valid.
	/// </summary>
	public class Error_InvalidAttribute : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPnP action-specific error code of 709.
		/// </summary>
		/// <param name="info"></param>
		public Error_InvalidAttribute(string info) : base (709, "Invalid attribute." + info) { Info = info;}

		string Info = "";
	}


	/// <summary>
	/// Thrown when a control point specifies a an ID intended for a media container
	/// and the container does not exist.
	/// </summary>
	public class Error_NoSuchContainer : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 710.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_NoSuchContainer(string info) : base (710, "No such container. " + info) {}
	}
	
	
	/// <summary>
	/// Thrown when a control calls DestroyObject to delete a media object when the permissions
	/// for the object do not allow such an operation.
	/// </summary>
	public class Error_RestrictedObject : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 711.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_RestrictedObject (string info) : base (711, "Restricted object. " + info) {}
	}


	/// <summary>
	/// Thrown when a control point atempts to create or set metadata properties with XML that
	/// is ill-formed or contains bad values.
	/// </summary>
	public class Error_BadMetadata : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 712. Allows inner exception information.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="innerException"></param>
		public Error_BadMetadata (string info, Exception innerException) : base (712, "Bad metadata. " + info, innerException) {}
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 712.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_BadMetadata (string info) : base (712, "Bad metadata. " + info) {}
	}

	/// <summary>
	/// Thrown when attempting to set the class of a media object and an error occurs.
	/// <see cref="IUPnPMedia"/> uses this exception.
	/// </summary>
	public class Error_BadMediaClass : Error_BadMetadata
	{
		/// <summary>
		/// Uses the error code for 
		/// <see cref="Error_BadMetadata"/>, but
		/// has a different message.
		/// </summary>
		/// <param name="classType">the string that attempted to map to a 
		/// <see cref="MediaClass"/>
		/// instance</param>
		public Error_BadMediaClass(string classType)
			: base ("The media class \"" +classType+ "\" is not derived from \"object.item\" nor \"object.container\".")
		{
			MediaClass = classType;
		}

		/// <summary>
		/// The string that attempted to map to a 
		/// <see cref="MediaClass"/>
		/// instance.
		/// </summary>
		public readonly string MediaClass;
	}

	/// <summary>
	/// Thrown when a control point calls DeleteResource on a resource that doesn't exist.
	/// </summary>
	public class Error_NoSuchResource : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 714.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_NoSuchResource (string info) : base (714, "No such resource. " + info) {}
	}

	/// <summary>
	/// Thrown when attempting to access or modify the content hierarchy in a manner
	/// that fails permissions checking.
	/// </summary>
	public class Error_AccessDenied : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 715
		/// </summary>
		/// <param name="info"></param>
		public Error_AccessDenied (string info) : base (715, "Access denied. " + info) {}
	}

	/// <summary>
	/// Thrown when a control point calls StopTransferResource when the file transfer ID
	/// is incorrect.
	/// </summary>
	public class Error_NoSuchFileTransfer : UPnPCustomException
	{
		/// <summary>
		/// Indicates UPNP-AV action-specific error code 717.
		/// </summary>
		/// <param name="info">Message with additional info.</param>
		public Error_NoSuchFileTransfer (string info) : base (717, "No such file transfer. " + info) {}
	}

	/// <summary>
	/// Thrown when attempting to add a MediaResource instance to a IUPnPMedia instance
	/// when the MediaResource instance has already been added to different IUPnPMedia instance.
	/// <see cref="IUPnPMedia"/> uses this exception.
	/// </summary>
	public class Error_MediaResourceHasParent : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor specific UPNP error code of 801.
		/// </summary>
		/// <param name="res">The <see cref="IMediaResource"/> instance that already has a parent.</param>
		public Error_MediaResourceHasParent (IMediaResource res)
			: base (801, "The specified resource cannot be associated with multiple media objects in this MediaServer implementation.")
		{
			this.Resource = res;
		}

		/// <summary>
		/// The IMediaResource instance that already has a parent.
		/// </summary>
		public readonly IMediaResource Resource;
	}

	/// <summary>
	/// Thrown when an attempting to add an object to a container, when the object
	/// already has a parent container.
	/// <see cref="MediaContainer"/> uses this exception.
	/// </summary>
	public class Error_MediaObjectHasParent : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor specific UPNP error code of 802.
		/// </summary>
		/// <param name="obj">The IUPnPMedia instance that already has a parent.</param>
		public Error_MediaObjectHasParent(IUPnPMedia obj)
			: base(802, "The media object (@id=\""+obj.ID+"\") already has a parent (@parentID=\""+obj.ParentID+"\").")
		{
			MediaObj = obj;
		}
 
		/// <summary>
		/// The IUPnPMedia instance that already has a parent.
		/// </summary>
		public readonly IUPnPMedia MediaObj;
	}

	/// <summary>
	/// Thrown when attempting to add a child object that already has a MediaContainer for a parent
	/// or when a container already has an item with the same ID as the one submitted for adding.
	/// <see cref="IUPnPMedia"/> uses this exception.
	/// </summary>
	public class Error_DuplicateIdException : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor specific UPNP error code of 803.
		/// </summary>
		/// <param name="obj">The offending IUPnPMedia that was sent to the container for insertion.</param>
		public Error_DuplicateIdException (IUPnPMedia obj)
			: base(803, "The media object (@id=\""+obj.ID+"\") conflicts with another media object with the same ID.")
		{
			MediaObj = obj;
		}

		/// <summary>
		/// The offending IUPnPmedia that was sent to the container for insertion.
		/// </summary>
		public readonly IUPnPMedia MediaObj;
	}

	/// <summary>
	/// Thrown when when attempting to update the metadata and resources of one media object
	/// with the metadata and resources of another media object, when one object
	/// is a container and the other is an item.
	/// <see cref="IUPnPMedia"/> uses this exception.
	/// </summary>
	public class Error_BaseClassMisMatch : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor specific UPNP error code of 804.
		/// </summary>
		/// <param name="obj1">The object that was getting updated.</param>
		/// <param name="obj2">The object with the new metadata and resources.</param>
		public Error_BaseClassMisMatch (IUPnPMedia obj1, IUPnPMedia obj2)
			: base (804, "Cannot update a media object with class \"" +obj1.Class.ToString()+ "\" with metadata from a class \"" +obj2.Class.ToString()+ "\".")
		{
			MediaObj = obj1;
			UpdateWithThis = obj2;
		}

		/// <summary>
		/// The object that was getting updated.
		/// </summary>
		public readonly IUPnPMedia MediaObj;

		/// <summary>
		/// The object with the new metadata and resources.
		/// </summary>
		public readonly IUPnPMedia UpdateWithThis;
	}

	/// <summary>
	/// Thrown an appropriate constructor cannot be found for
	/// the given value type.
	/// </summary>
	public class Error_NoConstructorForProperty : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor-specific error code of 805
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="type"></param>
		public Error_NoConstructorForProperty (string propertyName, System.Type type)
			: base (805, "No appropriate constructor for " +propertyName+ " (" +type.ToString()+")")
		{
		}
	}

	/// <summary>
	/// Thrown an appropriate constructor cannot be found for
	/// the given value type.
	/// </summary>
	public class Error_InvalidRangeForDimensions : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor-specific error code of 806
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Error_InvalidRangeForDimensions (int width, int height)
			: base (805, "Cannot exceeed 9999 on any axis.")
		{
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// The specified width at time of attempt.
		/// </summary>
		public readonly int Width;

		/// <summary>
		/// The specified height at time of attempt.
		/// </summary>
		public readonly int Height;
	}

	/// <summary>
	/// Thrown when attempting to add a media object that is both
	/// a container and an item.
	/// </summary>
	public class Error_ObjectIsContainerAndItem : UPnPCustomException
	{
		/// <summary>
		/// Indicates vendor specific UPNP error code of 810.
		/// </summary>
		/// <param name="obj"></param>
		public Error_ObjectIsContainerAndItem(IUPnPMedia obj)
			: base (810, "Cannot create an object that is both an item and a container.")
		{
			this.BadObject = obj;
		}

		/// <summary>
		/// The object that is both a container and an item.
		/// </summary>
		public readonly IUPnPMedia BadObject;
	}

	/// <summary>
	/// Thrown when a programmer uses an
	/// <see cref="IUPnPMedia"/>
	/// but improperly attempts to set a metadata value
	/// or change the object's state.
	/// </summary>
	public class Error_MetadataCallerViolation : ApplicationException
	{
		/// <summary>
		/// 
		/// </summary>
		public Error_MetadataCallerViolation ()
			: base ("You do not have permission to call this method from your current assembly/module.")
		{
		}
	}

}
