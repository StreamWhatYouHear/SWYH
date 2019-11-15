/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: Constants.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2019 - Sebastien Warin <http://sebastien.warin.fr> and others
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

namespace SWYH
{
    internal class Constants
    {
        public const string SWYH_PROCESS_NAME = "SWYH";
        public const string RESTART_ARGUMENT_NAME = "--restart";
        public const string STREAM_TO_ARGUMENT_NAME = "--streamto:";
        public const int NUMBER_OF_RESTART_TEST = 20;

        public const string SWYH_CRASHLOG_FILENAME = "SWYH_crash.log";
        public const string REGISTRY_START_SUBKEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string REGISTRY_SWYH_KEY = "StreamWhatYouHear";
        public const int DEFAULT_HTTP_PORT = 5901;

        public const string UPDATE_VERSION_URL = "http://www.streamwhatyouhear.com/update/lastest.txt";
        public const string DOWNLOAD_SWYH_URL = "http://www.streamwhatyouhear.com/download?source=swyh_update";
        public const string SWYH_WEBSITE_URL = "http://www.streamwhatyouhear.com/?source=swyh";
        public const string SWARIN_WEBSITE_URL = "http://sebastien.warin.fr/?source=swyh";
    }
}
