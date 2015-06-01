/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH.Audio.Mp3
 *	 File: AudioWriterConfig.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2015 - Sebastien Warin <http://sebastien.warin.fr>	 
 * 
 *   This file is part of C# MP3 Compressor written by Idael Cardoso and updated by Sebastien Warin for SWYH
 *   Source : http://www.codeproject.com/Articles/5901/C-MP-Compressor
 *	 Copyright (C) 2002-2003 Idael Cardoso. 
 *
 */	

//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED. 
//  SEE  http://www.mp3dev.org/ FOR TECHNICAL AND COPYRIGHT INFORMATION REGARDING 
//  LAME PROJECT.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2003 Idael Cardoso. 
//
//
//  About Thomson and/or Fraunhofer patents:
//  Any use of this product does not convey a license under the relevant 
//  intellectual property of Thomson and/or Fraunhofer Gesellschaft nor imply 
//  any right to use this product in any finished end user or ready-to-use final 
//  product. An independent license for such use is required. 
//  For details, please visit http://www.mp3licensing.com.
//

namespace SWYH.Audio.Mp3
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class AudioWriterConfig : ISerializable
    {
        protected WaveFormat m_Format;

        /// <summary>
        /// A constructor with this signature must be implemented by descendants. 
        /// <see cref="System.Runtime.Serialization.ISerializable"/> for more information
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> where is the serialized data.</param>
        /// <param name="context">The source (see <see cref="System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        protected AudioWriterConfig(SerializationInfo info, StreamingContext context)
        {
            int rate = info.GetInt32("Format.Rate");
            int bits = info.GetInt32("Format.Bits");
            int channels = info.GetInt32("Format.Channels");
            m_Format = new WaveFormat(rate, bits, channels);
        }

        public AudioWriterConfig(WaveFormat f)
        {
            m_Format = new WaveFormat(f.nSamplesPerSec, f.wBitsPerSample, f.nChannels);
        }

        public AudioWriterConfig()
            : this(new WaveFormat(44100, 16, 2))
        {
        }

        public WaveFormat Format
        {
            get
            {
                return m_Format;
            }
            set
            {
                m_Format = value;
            }
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Format.Rate", m_Format.nSamplesPerSec);
            info.AddValue("Format.Bits", m_Format.wBitsPerSample);
            info.AddValue("Format.Channels", m_Format.nChannels);
        }
    }
}
