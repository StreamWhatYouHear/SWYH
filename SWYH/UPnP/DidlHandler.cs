/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: DidlHandler.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2017 - Sebastien Warin <http://sebastien.warin.fr> and others	
 *
 *   This file is part of Stream What Your Hear.
 *	 
 *	 Stream What Your Hear is free software: you can redistribute it and/or modify
 *	 it under the terms of the GNU General Public License as published by
 *	 the Free Software Foundation, either version 2 of the License, or
 *	 (at your option) any later version.
 *	 
 *	 Stream What Your Hear is distributed in the hope that it will be useful,
 *	 but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	 GNU General Public License for more details.
 *	 
 *	 You should have received a copy of the GNU General Public License
 *	 along with Stream What Your Hear. If not, see <http://www.gnu.org/licenses/>.
 */

namespace SWYH.UPnP
{
    using System.Web;

    internal class DidlHandler
    {
        public static string GetContainer(string id, string parentID, string title, string upnpClass = "object.container", string restricted = "0")
        {
            string retVal =
            "<container id=\"" + id + "\" restricted=\"" + restricted + "\" parentID=\"" + parentID + "\">" +
            "<dc:title>" + HttpUtility.HtmlEncode(title) + "</dc:title>" +
            "<upnp:class>" + upnpClass + "</upnp:class>" +
            "</container>";
            return retVal;
        }

        public static string BeginDidl()
        {
            return "<DIDL-Lite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\">";
        }

        public static string EndDidl()
        {
            return "</DIDL-Lite>";
        }
    }
}
