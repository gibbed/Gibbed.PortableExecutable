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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.IO;

namespace Gibbed.PortableExecutable
{
    public class Executable32
    {
        public Image.DosHeader DosHeader;
        public Image.NTHeaders32 NTHeaders32;
        public List<Image.DataDirectory> Directories;
        public List<Image.SectionHeader> Sections;
        
        public Dictionary<UInt32, UInt32> Exports;
        public Dictionary<string, UInt32> ExportNames;

        public Image.SectionHeader GetSection(string name)
        {
            return this.Sections.SingleOrDefault(candidate => candidate.Name == name);
        }

        public UInt32 GetExport(UInt32 ordinal)
        {
            if (this.Exports.ContainsKey(ordinal) == false)
            {
                throw new KeyNotFoundException("no export with that ordinal");
            }

            return this.Exports[ordinal];
        }

        public UInt32 GetExport(string name)
        {
            if (this.ExportNames.ContainsKey(name) == false)
            {
                throw new KeyNotFoundException("no export with that name");
            }

            return this.Exports[this.ExportNames[name]];
        }

        public uint GetFileOffset(uint virtualAddress, bool relative)
        {
            if (relative == true)
            {
                virtualAddress += this.NTHeaders32.OptionalHeader.ImageBase;
            }

            foreach (Image.SectionHeader section in this.Sections)
            {
                if (virtualAddress >= (this.NTHeaders32.OptionalHeader.ImageBase + section.VirtualAddress) && virtualAddress < (this.NTHeaders32.OptionalHeader.ImageBase + section.VirtualAddress + section.VirtualSize))
                {
                    return section.PointerToRawData + (virtualAddress - (this.NTHeaders32.OptionalHeader.ImageBase + section.VirtualAddress));
                }
            }

            return 0;
        }

        public uint GetFileOffset(uint virtualAddress)
        {
            return this.GetFileOffset(virtualAddress, false);
        }

        public void Read(Stream input)
        {
            this.DosHeader = input.ReadStructure<Image.DosHeader>();
            if (this.DosHeader.Magic != 0x5A4D) // MZ
            {
                throw new FormatException("dos header has bad magic");
            }

            input.Seek(this.DosHeader.NewExeOffset, SeekOrigin.Begin);
            this.NTHeaders32 = input.ReadStructure<Image.NTHeaders32>();
            if (this.NTHeaders32.Signature != 0x4550 || this.NTHeaders32.FileHeader.SizeOfOptionalHeader != 0xE0) // PE
            {
                throw new FormatException("nt header has bad signature");
            }
            else if (this.NTHeaders32.OptionalHeader.Magic != 0x10B) // IMAGE_NT_OPTIONAL_HDR32_MAGIC
            {
                throw new FormatException("optional header has bad magic");
            }

            this.Directories = new List<Image.DataDirectory>();
            for (int i = 0; i < this.NTHeaders32.OptionalHeader.NumberOfRvaAndSizes; i++)
            {
                this.Directories.Add(input.ReadStructure<Image.DataDirectory>());
            }

            this.Sections = new List<Image.SectionHeader>();
            for (int i = 0; i < this.NTHeaders32.FileHeader.NumberOfSections; i++)
            {
                this.Sections.Add(input.ReadStructure<Image.SectionHeader>());
            }

            this.Exports = new Dictionary<UInt32, UInt32>();
            this.ExportNames = new Dictionary<string, UInt32>();

            if (
                this.Directories.Count >= 1 &&
                this.Directories[0].VirtualAddress != 0 &&
                this.Directories[0].Size != 0)
            {
                var fileOffset = this.GetFileOffset(this.Directories[0].VirtualAddress, true);

                input.Seek(fileOffset, SeekOrigin.Begin);
                Image.ExportDirectory exportDirectory = input.ReadStructure<Image.ExportDirectory>();

                if (exportDirectory.NumberOfNames > 0)
                {
                    var nameOffsets = new UInt32[exportDirectory.NumberOfNames];
                    var names = new string[exportDirectory.NumberOfNames];
                    var ordinals = new UInt16[exportDirectory.NumberOfNames];

                    input.Seek(this.GetFileOffset(exportDirectory.AddressOfNames, true), SeekOrigin.Begin);
                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        nameOffsets[i] = input.ReadValueU32();
                    }

                    input.Seek(this.GetFileOffset(exportDirectory.AddressOfNameOrdinals, true), SeekOrigin.Begin);
                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        ordinals[i] = input.ReadValueU16();
                    }

                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        input.Seek(this.GetFileOffset(nameOffsets[i], true), SeekOrigin.Begin);
                        var name = input.ReadStringZ(Encoding.ASCII);
                        this.ExportNames.Add(name, exportDirectory.Base + ordinals[i]);
                    }
                }

                if (exportDirectory.NumberOfFunctions > 0)
                {
                    input.Seek(this.GetFileOffset(exportDirectory.AddressOfFunctions, true), SeekOrigin.Begin);

                    for (uint i = 0; i < exportDirectory.NumberOfFunctions; i++)
                    {
                        var address = input.ReadValueU32();
                        if (address == 0)
                        {
                            continue;
                        }

                        this.Exports.Add(exportDirectory.Base + i, address);
                    }
                }
            }
        }
    }
}
