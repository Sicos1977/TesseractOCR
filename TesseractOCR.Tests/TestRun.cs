using System;

namespace Tesseract.Tests
{
    /// <summary>
    ///     Represents a test run.
    /// </summary>
    public class TestRun
    {
        private TestRun()
        {
            StartedAt = DateTime.Now;
        }

        public DateTime StartedAt { get; }

        public static readonly TestRun Current = new();
    }
}