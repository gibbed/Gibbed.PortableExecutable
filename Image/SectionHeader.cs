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
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct SectionHeader
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		public string Name;
		// public UInt32 PhysicalAddress;
		public UInt32 VirtualSize;
		public UInt32 VirtualAddress;
		public UInt32 SizeOfRawData;
		public UInt32 PointerToRawData;
		public UInt32 PointerToRelocations;
		public UInt32 PointerToLineNumbers;
		public UInt16 NumberOfRelocations;
		public UInt16 NumberOfLineNumbers;
		public UInt32 Characteristics;
	}
}
