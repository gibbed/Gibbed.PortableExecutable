/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
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

using System.Runtime.InteropServices;

namespace Gibbed.PortableExecutable.Image
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DosHeader
    {
        public const ushort Signature = 0x5A4D; // 'MZ'

        /// <summary>
        /// Magic number
        /// </summary>
        public ushort Magic;

        /// <summary>
        /// Bytes on last page of file
        /// </summary>
        public ushort e_cblp;

        /// <summary>
        /// Pages in file
        /// </summary>
        public ushort PageCount;

        /// <summary>
        /// Relocations
        /// </summary>
        public ushort RelocationCount;

        /// <summary>
        /// Size of header in paragraphs
        /// </summary>
        public ushort e_cparhdr;

        /// <summary>
        /// Minimum extra paragraphs needed
        /// </summary>
        public ushort MinimumAlloc;

        /// <summary>
        /// Maximum extra paragraphs needed
        /// </summary>
        public ushort MaxAlloc;

        /// <summary>
        /// Initial (relative) SS value
        /// </summary>
        public ushort SS;

        /// <summary>
        /// Initial SP value
        /// </summary>
        public ushort SP;

        /// <summary>
        /// Checksum
        /// </summary>
        public ushort Checksum;

        /// <summary>
        /// Initial IP value
        /// </summary>
        public ushort IP;

        /// <summary>
        /// Initial (relative) CS value
        /// </summary>
        public ushort CS;

        /// <summary>
        /// File address of relocation table
        /// </summary>
        public ushort RelocOffset;

        /// <summary>
        /// Overlay number
        /// </summary>
        public ushort OverlayNumber;

        /// <summary>
        /// Reserved words
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Reserved;

        /// <summary>
        /// OEM identifier
        /// </summary>
        public ushort OEMId;

        /// <summary>
        /// OEM information; OEM identifier specific
        /// </summary>
        public ushort OEMInfo;

        /// <summary>
        /// Reserved words
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] Reserved2;

        /// <summary>
        /// File address of new exe header
        /// </summary>
        public uint NewExeOffset;
    }
}
