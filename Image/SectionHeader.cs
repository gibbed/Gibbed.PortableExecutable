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

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Gibbed.PortableExecutable.Image
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SectionHeader : IEquatable<SectionHeader>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NameBytes;

        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLineNumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLineNumbers;
        public uint Characteristics;

        public string Name
        {
            get
            {
                var end = Array.IndexOf<byte>(this.NameBytes, 0);
                if (end < 0)
                {
                    end = this.NameBytes.Length;
                }
                return Encoding.UTF8.GetString(this.NameBytes, 0, end);
            }
        }

        public bool Equals(SectionHeader other)
        {
            return Equals(this.NameBytes, other.NameBytes) == true &&
                   this.VirtualSize == other.VirtualSize &&
                   this.VirtualAddress == other.VirtualAddress &&
                   this.SizeOfRawData == other.SizeOfRawData &&
                   this.PointerToRawData == other.PointerToRawData &&
                   this.PointerToRelocations == other.PointerToRelocations &&
                   this.PointerToLineNumbers == other.PointerToLineNumbers &&
                   this.NumberOfRelocations == other.NumberOfRelocations &&
                   this.NumberOfLineNumbers == other.NumberOfLineNumbers &&
                   this.Characteristics == other.Characteristics;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) == true)
            {
                return false;
            }
            return obj is SectionHeader && Equals((SectionHeader)obj) == true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (this.NameBytes != null ? this.NameBytes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)this.VirtualSize;
                hashCode = (hashCode * 397) ^ (int)this.VirtualAddress;
                hashCode = (hashCode * 397) ^ (int)this.SizeOfRawData;
                hashCode = (hashCode * 397) ^ (int)this.PointerToRawData;
                hashCode = (hashCode * 397) ^ (int)this.PointerToRelocations;
                hashCode = (hashCode * 397) ^ (int)this.PointerToLineNumbers;
                hashCode = (hashCode * 397) ^ this.NumberOfRelocations.GetHashCode();
                hashCode = (hashCode * 397) ^ this.NumberOfLineNumbers.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Characteristics;
                return hashCode;
            }
        }

        public static bool operator ==(SectionHeader left, SectionHeader right)
        {
            return left.Equals(right) == true;
        }

        public static bool operator !=(SectionHeader left, SectionHeader right)
        {
            return left.Equals(right) == false;
        }
    }
}
