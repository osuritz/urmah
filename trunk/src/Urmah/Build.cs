using System;
using System.Collections.Generic;
using System.Text;

namespace Urmah
{
    internal sealed class Build
    {
#if DEBUG
        public const bool IsDebug = true;
        public const bool IsRelease = !IsDebug;
        public const string Type = "Debug";
        public const string TypeUppercase = "DEBUG";
        public const string TypeLowercase = "debug";
#else
        public const bool IsDebug = false;
        public const bool IsRelease = !IsDebug;
        public const string Type = "Release";
        public const string TypeUppercase = "RELEASE";
        public const string TypeLowercase = "release";
#endif
    }
}
