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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Gibbed.IO;

namespace Gibbed.PortableExecutable
{
    public abstract class BaseExecutable<TOptionalHeader>
        where TOptionalHeader : Image.IOptionalHeader
    {
        #region Fields
        private long _BaseFilePosition;
        private Image.DosHeader _DosHeader;
        private Image.CoffHeader _CoffHeader;
        private TOptionalHeader _OptionalHeader;
        private readonly List<Image.DataDirectory> _Directories;
        private readonly List<Image.SectionHeader> _Sections;
        #endregion

        internal BaseExecutable()
        {
            this._Directories = new List<Image.DataDirectory>();
            this._Sections = new List<Image.SectionHeader>();
        }

        #region Properties
        public long BaseFilePosition
        {
            get { return this._BaseFilePosition; }
            set { this._BaseFilePosition = value; }
        }

        public Image.DosHeader DosHeader
        {
            get { return this._DosHeader; }
            set { this._DosHeader = value; }
        }

        public Image.CoffHeader CoffHeader
        {
            get { return this._CoffHeader; }
            set { this._CoffHeader = value; }
        }

        public TOptionalHeader OptionalHeader
        {
            get { return this._OptionalHeader; }
            set { this._OptionalHeader = value; }
        }

        public List<Image.DataDirectory> Directories
        {
            get { return this._Directories; }
        }

        public List<Image.SectionHeader> Sections
        {
            get { return this._Sections; }
        }
        #endregion

        public Image.SectionHeader? GetSection(string name)
        {
            var result = this._Sections.FirstOrDefault(candidate => candidate.Name == name);
            return result == default(Image.SectionHeader) ? (Image.SectionHeader?)null : result;
        }

        public Image.SectionHeader? GetSection(ulong virtualAddress, bool relative)
        {
            if (relative == true)
            {
                virtualAddress += this._OptionalHeader.ImageBase;
            }

            var result = this._Sections.FirstOrDefault(c => virtualAddress >= c.VirtualAddress &&
                                                            virtualAddress <= c.VirtualAddress + c.VirtualSize);
            return result == default(Image.SectionHeader) ? (Image.SectionHeader?)null : result;
        }

        public long GetFileOffset(ulong virtualAddress, bool relative)
        {
            if (relative == true)
            {
                virtualAddress += this._OptionalHeader.ImageBase;
            }

            foreach (Image.SectionHeader section in this._Sections)
            {
                var startAddress = this._OptionalHeader.ImageBase + section.VirtualAddress;
                var endAddress = startAddress + section.VirtualSize;

                if (virtualAddress >= startAddress &&
                    virtualAddress < endAddress)
                {
                    return (long)(section.PointerToRawData + (virtualAddress - startAddress));
                }
            }

            return 0;
        }

        public long GetFileOffset(uint virtualAddress)
        {
            return this.GetFileOffset(virtualAddress, false);
        }

        public ulong GetMemoryAddress(long fileOffset, bool relative)
        {
            foreach (var section in this._Sections)
            {
                if (fileOffset >= section.PointerToRawData &&
                    fileOffset < (section.PointerToRawData + section.SizeOfRawData))
                {
                    var virtualAddress = section.VirtualAddress + (ulong)(fileOffset - section.PointerToRawData);
                    if (relative == true)
                    {
                        virtualAddress += this._OptionalHeader.ImageBase;
                    }
                    return virtualAddress;
                }
            }

            return 0;
        }

        protected static void Read(Stream input, BaseExecutable<TOptionalHeader> instance)
        {
            var basePosition = input.Position;

            var dosHeader = input.ReadStructure<Image.DosHeader>();
            if (dosHeader.Magic != Image.DosHeader.Signature) // MZ
            {
                throw new FormatException("DOS header has bad magic");
            }

            input.Position = basePosition + dosHeader.NewExeOffset;
            var peHeaders = input.ReadStructure<Image.PeHeaders<TOptionalHeader>>();

            if (peHeaders.Magic != Image.PeHeaders<TOptionalHeader>.Signature)
            {
                throw new FormatException("NT headers has bad signature");
            }

            if (peHeaders.FileHeader.SizeOfOptionalHeader != Marshal.SizeOf(typeof(TOptionalHeader)))
            {
                throw new FormatException("NT headers has a size mismatch");
            }

            var optionalHeader = (Image.IOptionalHeader)peHeaders.OptionalHeader;
            if (optionalHeader.Magic != optionalHeader.Signature)
            {
                throw new FormatException("optional header has bad magic");
            }

            var directories = new Image.DataDirectory[peHeaders.OptionalHeader.NumberOfRvaAndSizes];
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = input.ReadStructure<Image.DataDirectory>();
            }

            var sections = new Image.SectionHeader[peHeaders.FileHeader.NumberOfSections];
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i] = input.ReadStructure<Image.SectionHeader>();
            }

            instance.BaseFilePosition = basePosition;
            instance.DosHeader = dosHeader;
            instance.CoffHeader = peHeaders.FileHeader;
            instance.OptionalHeader = peHeaders.OptionalHeader;
            instance.Directories.Clear();
            instance.Directories.AddRange(directories);
            instance.Sections.Clear();
            instance.Sections.AddRange(sections);
        }

        public class ExportInfo
        {
            #region Fields
            private readonly Dictionary<uint, uint> _OrdinalLookup;
            private readonly Dictionary<string, uint> _NameLookup;
            #endregion

            public ExportInfo()
            {
                this._OrdinalLookup = new Dictionary<uint, uint>();
                this._NameLookup = new Dictionary<string, uint>();
            }

            #region Properties
            public Dictionary<uint, uint> Exports
            {
                get { return this._OrdinalLookup; }
            }

            public Dictionary<string, uint> ExportNames
            {
                get { return this._NameLookup; }
            }
            #endregion

            public uint GetExport(uint ordinal)
            {
                if (this._OrdinalLookup.ContainsKey(ordinal) == false)
                {
                    throw new KeyNotFoundException("no export with that ordinal");
                }

                return this._OrdinalLookup[ordinal];
            }

            public uint GetExport(string name)
            {
                if (this._NameLookup.ContainsKey(name) == false)
                {
                    throw new KeyNotFoundException("no export with that name");
                }

                return this._OrdinalLookup[this._NameLookup[name]];
            }

            public static ExportInfo Read(Stream input, BaseExecutable<Image.IOptionalHeader> executable)
            {
                var instance = new ExportInfo();
                if (executable.Directories.Count < 1 ||
                    executable.Directories[0].VirtualAddress == 0 || executable.Directories[0].Size == 0)
                {
                    return instance;
                }

                var fileOffset = executable.GetFileOffset(executable.Directories[0].VirtualAddress, true);

                input.Position = executable.BaseFilePosition + fileOffset;
                var exportDirectory = input.ReadStructure<Image.ExportDirectory>();

                if (exportDirectory.NumberOfNames > 0)
                {
                    var nameAddresses = new UInt32[exportDirectory.NumberOfNames];
                    var ordinals = new UInt16[exportDirectory.NumberOfNames];

                    var offsetOfNames = executable.GetFileOffset(exportDirectory.AddressOfNames, true);
                    input.Position = executable.BaseFilePosition + offsetOfNames;
                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        nameAddresses[i] = input.ReadValueU32();
                    }

                    var offsetOfNameOrdinals = executable.GetFileOffset(exportDirectory.AddressOfNameOrdinals, true);
                    input.Position = executable.BaseFilePosition + offsetOfNameOrdinals;
                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        ordinals[i] = input.ReadValueU16();
                    }

                    var names = new string[exportDirectory.NumberOfNames];
                    for (uint i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        var nameOffset = executable.GetFileOffset(nameAddresses[i], true);
                        input.Position = executable.BaseFilePosition + nameOffset;
                        names[i] = input.ReadStringZ(Encoding.ASCII);
                    }

                    for (int i = 0; i < exportDirectory.NumberOfNames; i++)
                    {
                        instance._NameLookup.Add(names[i], exportDirectory.Base + ordinals[i]);
                    }
                }

                if (exportDirectory.NumberOfFunctions > 0)
                {
                    var offsetOfFunctions = executable.GetFileOffset(exportDirectory.AddressOfFunctions, true);
                    input.Position = executable.BaseFilePosition + offsetOfFunctions;
                    for (uint i = 0; i < exportDirectory.NumberOfFunctions; i++)
                    {
                        var address = input.ReadValueU32();
                        if (address == 0)
                        {
                            continue;
                        }

                        instance._OrdinalLookup.Add(exportDirectory.Base + i, address);
                    }
                }

                return instance;
            }
        }
    }
}
