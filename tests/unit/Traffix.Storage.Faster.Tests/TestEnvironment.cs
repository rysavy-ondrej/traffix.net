using System;
using System.IO;

namespace Traffix.Storage.Faster.Tests
{
    public static class TestEnvironment
    {
        public static readonly string DataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\data"));
    }
}
