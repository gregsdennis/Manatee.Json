// Source: https://stackoverflow.com/a/29610171/878701

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Manatee.Json.Internal;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
    public static class PathExtensions
    {
        public static string AdjustForOS(this string path)
        {
            return Environment.OSVersion.Platform.In(PlatformID.MacOSX, PlatformID.Unix)
                       ? path.Replace("\\", "/")
                       : path.Replace("/", "\\");
        }
    }
}