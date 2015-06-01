/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH.Audio.Mp3
 *	 File: AudioWaveFormat.cs
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
    using System.Runtime.InteropServices;

    public enum WaveFormats
    {
        Pcm = 1,
        Float = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;

        public WaveFormat(int rate, int bits, int channels)
        {
            wFormatTag = (short)WaveFormats.Pcm;
            nChannels = (short)channels;
            nSamplesPerSec = rate;
            wBitsPerSample = (short)bits;
            cbSize = 0;

            nBlockAlign = (short)(channels * (bits / 8));
            nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
        }
    }
}
