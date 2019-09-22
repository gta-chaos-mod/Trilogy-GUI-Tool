// Copyright (c) 2019 Lordmau5

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GtaChaos.Models.Utils
{
    public static class MemoryHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        public static bool Read<T>(IntPtr lpBaseAddress, out T value) where T : struct
        {
            if (ProcessHooker.HasExited())
            {
                value = default;
                return false;
            }

            T[] buffer = new T[Marshal.SizeOf<T>()];
            ReadProcessMemory(ProcessHooker.GetHandle(), lpBaseAddress, buffer, Marshal.SizeOf<T>(), out var _);

            value = buffer.FirstOrDefault();
            return true;
        }

        public static bool Read<T>(IntPtr lpBaseAddress, out T value, List<int> offsets) where T : struct
        {
            IntPtr address = lpBaseAddress;

            var lastOffset = offsets.Last();
            offsets.RemoveAt(offsets.Count - 1);

            foreach (var offset in offsets)
            {
                if (!Read<IntPtr>(IntPtr.Add(address, offset), out address))
                {
                    value = default;
                    return false;
                }
            }

            return Read<T>(IntPtr.Add(address, lastOffset), out value);
        }
    }
}
