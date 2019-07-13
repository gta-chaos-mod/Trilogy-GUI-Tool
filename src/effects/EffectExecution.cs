using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GTA_SA_Chaos.effects
{
    public static class EffectExecution
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesWritten
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

        public static bool Write<T>(IntPtr lpBaseAddress, T value) where T : struct
        {
            if (ProcessHooker.HasExited())
            {
                return false;
            }

            var buffer = new T[Marshal.SizeOf<T>()];
            buffer[0] = value;
            return WriteProcessMemory(ProcessHooker.GetHandle(), lpBaseAddress, buffer, Marshal.SizeOf<T>(), out var _);
        }

        public static bool Write<T>(IntPtr lpBaseAddress, T value, List<int> offsets) where T : struct
        {
            IntPtr address = lpBaseAddress;

            var lastOffset = offsets.Last();
            offsets.RemoveAt(offsets.Count - 1);

            foreach (var offset in offsets)
            {
                if(!Read<IntPtr>(IntPtr.Add(address, offset), out address))
                {
                    return false;
                }
            }

            return Write<T>(IntPtr.Add(address, lastOffset), value);
        }
    }
}
