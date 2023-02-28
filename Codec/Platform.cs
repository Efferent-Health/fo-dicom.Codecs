using System.Reflection;

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
            var arch = typeof(string).Assembly.GetName().ProcessorArchitecture;

            if (Platform.Current == Platform.Type.win_x64)
            {
                if (arch == ProcessorArchitecture.Amd64)
                    return Type.win_x64;
            }
            else if (Platform.Current == Platform.Type.linux_x64)
            {
                if (arch == ProcessorArchitecture.Amd64)
                    return Type.linux_x64;
            }
            else if (Platform.Current == Platform.Type.osx_x64)
            {
                if (arch == ProcessorArchitecture.Amd64)
                    return Type.osx_x64;
                else if (arch == ProcessorArchitecture.MSIL)
                    return Type.osx_arm64;
            }

            return Type.unsupported;
        }
    }
}
