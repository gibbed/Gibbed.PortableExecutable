/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Runtime.InteropServices;

namespace Gibbed.PortableExecutable.Image
{
	[StructLayout(LayoutKind.Sequential)]
	public struct OptionalHeader32
	{
	    // Standard fields.
		public UInt16 Magic;
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public UInt32 SizeOfCode;
		public UInt32 SizeOfInitializedData;
		public UInt32 SizeOfUninitializedData;
		public UInt32 AddressOfEntryPoint;
		public UInt32 BaseOfCode;
		public UInt32 BaseOfData;

		// NT additional fields.
		public UInt32 ImageBase;
		public UInt32 SectionAlignment;
		public UInt32 FileAlignment;
		public UInt16 MajorOperatingSystemVersion;
		public UInt16 MinorOperatingSystemVersion;
		public UInt16 MajorImageVersion;
		public UInt16 MinorImageVersion;
		public UInt16 MajorSubsystemVersion;
		public UInt16 MinorSubsystemVersion;
		public UInt32 Win32VersionValue;
		public UInt32 SizeOfImage;
		public UInt32 SizeOfHeaders;
		public UInt32 CheckSum;
		public UInt16 Subsystem;
		public UInt16 DllCharacteristics;
		public UInt32 SizeOfStackReserve;
		public UInt32 SizeOfStackCommit;
		public UInt32 SizeOfHeapReserve;
		public UInt32 SizeOfHeapCommit;
		public UInt32 LoaderFlags;
		public UInt32 NumberOfRvaAndSizes;
		//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        //ImageDataDirectory[] DataDirectory;
	}
}
