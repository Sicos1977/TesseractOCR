using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR.InteropDotNet;

namespace Tesseract.Tests;

[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class WindowsNativeTests
{
    [TestMethod]
    public void LoadsNativeLibraryFromRuntimeDirectory()
    {
        var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        var expected = Path.Combine(AppContext.BaseDirectory, "runtimes", "win-" + architecture, "native", "leptonica-1.87.0.dll");
        Assert.IsTrue(File.Exists(expected), $"Missing runtime asset: {expected}");

        var handle = LibraryLoader.Instance.LoadLibrary("leptonica-1.87.0.dll");
        var path = new StringBuilder(32768);
        Assert.AreNotEqual(0u, GetModuleFileName(handle, path, path.Capacity));
        Assert.AreEqual(Path.GetFullPath(expected), Path.GetFullPath(path.ToString()), true);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetModuleFileName(IntPtr module, StringBuilder fileName, int size);
}
