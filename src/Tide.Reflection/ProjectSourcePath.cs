using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tide.Reflection
{
    #nullable enable
    internal static class ProjectSourcePath
    {
        private const string myRelativePath = nameof(ProjectSourcePath) + ".cs";
        private static string? lazyValue;
        public static string Path => lazyValue ??= CalculatePath();

        private static string CalculatePath()
        {
            string pathName = GetSourceFilePathName();
            Debug.Assert(pathName.EndsWith(myRelativePath, StringComparison.Ordinal));
            return pathName.Substring(0, pathName.Length - myRelativePath.Length);
        }
        public static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        {
            return callerFilePath ?? "";
        }
    }
}