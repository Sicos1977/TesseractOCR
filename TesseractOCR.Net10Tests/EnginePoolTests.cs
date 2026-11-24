using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Exceptions;

namespace Tesseract.Tests;

[TestClass]
public class EnginePoolTests
{
    private static string DataPath => Path.Combine(AppContext.BaseDirectory, "tessdata");
    private static TesseractOCR.Pix.Image LoadTestPix() => TesseractOCR.Pix.Image.Create(100, 100, 1);

    [TestMethod]
    public async Task ParallelScansReturnExpectedTextAndPoolCanBeReused()
    {
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 2);
        var tasks = Enumerable.Range(0, 6).Select(_ => Task.Run(async () =>
        {
            using var image = CreateTextImage();
            return await pool.ProcessAsync(image, PageSegMode.SingleLine);
        })).ToArray();

        var texts = await Task.WhenAll(tasks);
        foreach (var text in texts)
            Assert.AreEqual("TEST TEST", text.Trim());

        using var nextImage = CreateTextImage();
        Assert.AreEqual("TEST TEST", (await pool.ProcessAsync(nextImage, PageSegMode.SingleLine)).Trim());
    }

    [TestMethod]
    public async Task RecognizesGeneratedText()
    {
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
        using var image = CreateTextImage();
        var text = await pool.ProcessAsync(image, PageSegMode.SingleLine);
        Assert.AreEqual("TEST TEST", text.Trim());
    }

    [TestMethod]
    public async Task RecognizesBitmapLoadedFromFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"tesseract-pool-{Guid.NewGuid():N}.bmp");
        try
        {
            using (var original = CreateTextImage())
                original.Save(path);
            using var image = TesseractOCR.Pix.Image.LoadFromFile(path);
            using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
            Assert.AreEqual("TEST TEST", (await pool.ProcessAsync(image, PageSegMode.SingleLine)).Trim());
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static unsafe TesseractOCR.Pix.Image CreateTextImage()
    {
        var glyphs = new[]
        {
            new[] { "11111", "00100", "00100", "00100", "00100", "00100", "00100" },
            new[] { "11111", "10000", "10000", "11110", "10000", "10000", "11111" },
            new[] { "11111", "10000", "10000", "11111", "00001", "00001", "11111" },
            new[] { "11111", "00100", "00100", "00100", "00100", "00100", "00100" }
        };
        const int scale = 6;
        var image = TesseractOCR.Pix.Image.Create(400, 90, 1);
        var data = image.GetData();
        for (var letter = 0; letter < 8; letter++)
        for (var y = 0; y < 7; y++)
        for (var x = 0; x < 5; x++)
        {
            if (glyphs[letter % 4][y][x] != '1') continue;
            for (var dy = 0; dy < scale; dy++)
            for (var dx = 0; dx < scale; dx++)
            {
                var px = 10 + letter * 36 + (letter >= 4 ? 18 : 0) + x * scale + dx;
                var py = 15 + y * scale + dy;
                TesseractOCR.Pix.Data.SetDataBit((uint*)data.PixData + py * data.WordsPerLine, px, 1);
            }
        }
        return image;
    }

    [TestMethod]
    public async Task ReusesEngineAndDisposesItOnShutdown()
    {
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
        using var image = LoadTestPix();
        await pool.ProcessAsync(image, PageSegMode.SingleLine);
        var objects = (ObjectPool<Engine>)typeof(EnginePool).GetField("_pool", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pool)!;
        var engine = objects.Get();
        objects.Return(engine);

        await pool.ProcessAsync(image, PageSegMode.SingleLine);
        var reused = objects.Get();
        Assert.AreSame(engine, reused);
        objects.Return(reused);
        pool.Dispose();
        Assert.IsTrue(engine.IsDisposed);
    }

    [TestMethod]
    public async Task LogsCreationAcquisitionReturnReuseAndDisposalAtDebugLevel()
    {
        var logger = new RecordingLogger();
        var pool = new EnginePool(DataPath, Language.English, logger, maximumEngines: 1);
        using var image = LoadTestPix();
        try
        {
            await pool.ProcessAsync(image, PageSegMode.SingleLine);
            await pool.ProcessAsync(image, PageSegMode.SingleLine);
        }
        finally
        {
            pool.Dispose();
        }

        var entries = logger.Entries.ToArray();
        Assert.AreEqual(8, entries.Length);
        Assert.IsTrue(entries.All(entry => entry.Level == LogLevel.Debug));
        StringAssert.Contains(entries[0].Message, "Engine pool created");
        StringAssert.Contains(entries[1].Message, "created");
        StringAssert.Contains(entries[2].Message, "acquired from pool");
        StringAssert.Contains(entries[3].Message, "returned to pool");
        StringAssert.Contains(entries[4].Message, "acquired from pool");
        StringAssert.Contains(entries[5].Message, "returned to pool");
        StringAssert.Contains(entries[6].Message, "disposed");
        StringAssert.Contains(entries[7].Message, "Engine pool closed");
        var id = entries[1].EngineId;
        Assert.IsNotNull(id);
        foreach (var entry in entries.Skip(2).Take(5))
            Assert.AreEqual(id, entry.EngineId);
    }

    [TestMethod]
    public async Task CancelAfterAcquisitionLogsDisposalInsteadOfReturn()
    {
        using var cancellation = new CancellationTokenSource();
        var logger = new RecordingLogger { OnAcquired = cancellation.Cancel };
        using var pool = new EnginePool(DataPath, Language.English, logger, maximumEngines: 1);
        using var image = LoadTestPix();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await pool.ProcessAsync(image, cancellationToken: cancellation.Token));
        var entries = logger.Entries.ToArray();
        Assert.AreEqual(1, entries.Count(entry => entry.Message.EndsWith(" disposed")));
        Assert.AreEqual(0, entries.Count(entry => entry.Message.Contains("returned to pool")));
        Assert.AreEqual(entries[1].EngineId, entries[^1].EngineId);
    }

    [TestMethod]
    public async Task ClosingPoolDuringScanLogsDisposalOnReturn()
    {
        var logger = new RecordingLogger();
        using var pool = new EnginePool(DataPath, Language.English, logger, maximumEngines: 1);
        logger.OnAcquired = pool.Dispose;
        using var image = LoadTestPix();

        Assert.AreEqual(string.Empty, (await pool.ProcessAsync(image, PageSegMode.SingleLine)).Trim());
        var entries = logger.Entries.ToArray();
        Assert.AreEqual(1, entries.Count(entry => entry.Message.EndsWith(" disposed")));
        Assert.AreEqual(0, entries.Count(entry => entry.Message.Contains("returned to pool")));
        Assert.AreEqual(entries[1].EngineId, entries[^1].EngineId);
    }

    [TestMethod]
    public async Task WaitingRequestCanBeCancelledWithoutLosingCapacity()
    {
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
        using var image = LoadTestPix();
        var slots = GetSlots(pool);
        await slots.WaitAsync();
        using var cancellation = new CancellationTokenSource();
        var waiting = pool.ProcessAsync(image, cancellationToken: cancellation.Token);
        Assert.IsFalse(waiting.IsCompleted);
        cancellation.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await waiting);
        Assert.AreEqual(0, slots.CurrentCount);
        slots.Release();
        Assert.AreEqual(string.Empty, (await pool.ProcessAsync(image, PageSegMode.SingleLine)).Trim());
    }

    [TestMethod]
    public async Task DisposeRejectsWaitersAndNewRequests()
    {
        using var pool = new EnginePool(DataPath, Language.English, maximumEngines: 1);
        using var image = LoadTestPix();
        var slots = GetSlots(pool);
        await slots.WaitAsync();
        var waiting = pool.ProcessAsync(image);
        Assert.IsFalse(waiting.IsCompleted);
        pool.Dispose();
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(async () => await waiting);
        slots.Release();
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(async () => await pool.ProcessAsync(image));
    }

    [TestMethod]
    public async Task FailedEngineCreationReleasesCapacity()
    {
        using var pool = new EnginePool("missing-tessdata-directory", Language.English, maximumEngines: 1);
        using var image = LoadTestPix();
        await Assert.ThrowsExactlyAsync<TesseractException>(async () => await pool.ProcessAsync(image));
        var second = pool.ProcessAsync(image);
        await Assert.ThrowsExactlyAsync<TesseractException>(async () => await second.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [TestMethod]
    public void MaximumMustBePositive()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new EnginePool(DataPath, Language.English, maximumEngines: 0));
    }

    [TestMethod]
    public void InvalidInitialOptionsDoNotPreventLaterEngineInitialization()
    {
        var options = new Dictionary<string, object> { ["invalid"] = new object() };
        Assert.ThrowsExactly<ArgumentException>(() => new Engine(DataPath, Language.English, initialOptions: options));
        using var engine = new Engine(DataPath, Language.English);
        using var image = LoadTestPix();
        using var page = engine.Process(image, PageSegMode.SingleLine);
        Assert.AreEqual(string.Empty, page.Text.Trim());
    }

    private static SemaphoreSlim GetSlots(EnginePool pool) =>
        (SemaphoreSlim)typeof(EnginePool).GetField("_slots", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pool)!;

    private sealed class RecordingLogger : ILogger
    {
        public ConcurrentQueue<(LogLevel Level, string Message, object EngineId)> Entries { get; } = new();
        public Action OnAcquired { get; set; }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var properties = state as IEnumerable<KeyValuePair<string, object>>;
            var id = properties?.FirstOrDefault(pair => pair.Key == "EngineId").Value;
            var message = formatter(state, exception);
            Entries.Enqueue((logLevel, message, id));
            if (message.Contains("acquired from pool"))
                OnAcquired?.Invoke();
        }
    }
}
