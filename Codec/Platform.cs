using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FellowOakDicom.Imaging.NativeCodec
{   
    public static class Platform
    {
        private static Type currentType = Type.unknown;

        public enum Type
        {
            unknown,
            unsupported,
            win_x64,
            win_arm64,
            linux_x64,
            linux_arm64,
            osx_x64,
            osx_arm64
        }

        public static Type Current
        {
            get 
            {
                if (currentType == Type.unknown)
                    currentType = getCurrentType();

                return currentType;
            }
        }

        private static Type getCurrentType()
        {
            var arch = getProcessorArchitecture();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (arch == "AMD64")
                    return Type.win_x64;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {   
                if (arch == "AMD64")
                    return Type.linux_x64;
                else if (arch == "ARM64")
                    return Type.linux_arm64;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (arch == "AMD64")
                    return Type.osx_x64;
                else if (arch == "ARM64")
                    return Type.osx_arm64;
            }

            return Type.unsupported;
        }

        private static string getProcessorArchitecture()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;

            using (var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(stream))
            {
                stream.Seek(0x3C, SeekOrigin.Begin);
                var peHeaderOffset = reader.ReadInt32();

                stream.Seek(peHeaderOffset + 4, SeekOrigin.Begin); // Skip PE Signature
                var machineType = reader.ReadUInt16();

                switch (machineType)
                {
                    case 0x014c: 
                        return "X86";
                    case 0x8664: 
                        return "AMD64";
                    case 0x01c0: 
                        return "ARM";
                    case 0xaa64: 
                        return "ARM64";
                    default:
                        return "UNKNOWN";
                }
            }
        }
    }
}