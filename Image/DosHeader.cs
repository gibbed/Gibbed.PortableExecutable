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
	public struct DosHeader
	{
		/// <summary>
		/// Magic number
		/// </summary>
		public UInt16 Magic;

		/// <summary>
		/// Bytes on last page of file
		/// </summary>
		public UInt16 e_cblp;
		
		/// <summary>
		/// Pages in file
		/// </summary>
		public UInt16 PageCount;
		
		/// <summary>
		/// Relocations
		/// </summary>
		public UInt16 RelocationCount;
		
		/// <summary>
		/// Size of header in paragraphs
		/// </summary>
		public UInt16 e_cparhdr;
		
		/// <summary>
		/// Minimum extra paragraphs needed
		/// </summary>
		public UInt16 MinimumAlloc;
		
		/// <summary>
		/// Maximum extra paragraphs needed
		/// </summary>
		public UInt16 MaxAlloc;
		
		/// <summary>
		/// Initial (relative) SS value
		/// </summary>
		public UInt16 SS;
		
		/// <summary>
		/// Initial SP value
		/// </summary>
		public UInt16 SP;
		
		/// <summary>
		/// Checksum
		/// </summary>
		public UInt16 Checksum;
		
		/// <summary>
		/// Initial IP value
		/// </summary>
		public UInt16 IP;
		
		/// <summary>
		/// Initial (relative) CS value
		/// </summary>
		public UInt16 CS;
		
		/// <summary>
		/// File address of relocation table
		/// </summary>
		public UInt16 RelocOffset;
		
		/// <summary>
		/// Overlay number
		/// </summary>
		public UInt16 OverlayNumber;

		/// <summary>
		/// Reserved words
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public UInt16[] Reserved;
		
	
		/// <summary>
		/// OEM identifier (for e_oeminfo)
		/// </summary>
		public UInt16 OEMId;
		
		/// <summary>
		/// OEM information; e_oemid specific
		/// </summary>
		public UInt16 OEMInfo;
		
		/// <summary>
		/// Reserved words
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public UInt16[] Reserved2;
		
		/// <summary>
		/// File address of new exe header
		/// </summary>
		public UInt32 NewExeOffset;
	}
}
