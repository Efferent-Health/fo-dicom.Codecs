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
            var arch = RuntimeInformation.OSArchitecture;

            if (arch.Equals(Architecture.X64))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Type.win_x64;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Type.linux_x64;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Type.osx_x64;
                }
            }
            else if (arch.Equals(Architecture.Arm64))
            {   
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Type.win_arm64;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Type.linux_arm64;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Type.osx_arm64;
                }
            }

            return Type.unsupported;
        }
    }
}
