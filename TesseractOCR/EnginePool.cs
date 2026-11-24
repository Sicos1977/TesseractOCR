using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

namespace TesseractOCR;

/// <summary>
///     A bounded pool of independent OCR engines for recognizing images concurrently.
/// </summary>
/// <remarks>
///     The pool owns its engines, but not the images passed to it. Do not dispose or use an image
///     concurrently while it is being processed. Disposing the pool rejects waiting and new requests;
///     scans already in progress finish and dispose their engines when they return.
/// </remarks>
public sealed class EnginePool : IDisposable
{
    #region Fields
    /// <summary>
    ///     The object used to synchronize access to shared state.
    /// </summary>
    private readonly object _sync = new();

    /// <summary>
    ///     Controls access to available engine slots.
    /// </summary>
    private readonly SemaphoreSlim _slots;

    /// <summary>
    ///     Cancellation token source used to signal shutdown.
    /// </summary>
    private readonly CancellationTokenSource _shutdown = new();

    /// <summary>
    ///     Pool used to reuse <see cref="Engine"/> instances.
    /// </summary>
    private readonly ObjectPool<Engine> _pool;
    private readonly ILogger _logger;

    /// <summary>
    /// Indicates whether the object has been disposed.
    /// </summary>
    private bool _disposed;
    #endregion

    #region EnginePool
    /// <summary>
    ///     Creates a pool whose engines share the specified initialization settings.
    /// </summary>
    /// <param name="dataPath">Directory containing the traineddata files.</param>
    /// <param name="language">Recognition language.</param>
    /// <param name="engineMode">Tesseract engine mode.</param>
    /// <param name="maximumEngines">Maximum number of engines, or null to use the processor count.</param>
    public EnginePool(string dataPath, Language language, EngineMode engineMode = EngineMode.Default, int? maximumEngines = null)
        : this(dataPath, language, null, engineMode, maximumEngines)
    {
    }

    /// <summary>
    ///     Creates a pool with optional lifecycle logging.
    /// </summary>
    /// <param name="dataPath">Directory containing the traineddata files.</param>
    /// <param name="language">Recognition language.</param>
    /// <param name="logger">Logger for pool events, or null to disable logging.</param>
    /// <param name="engineMode">Tesseract engine mode.</param>
    /// <param name="maximumEngines">Maximum number of engines, or null to use the processor count.</param>
    public EnginePool(string dataPath, Language language, ILogger logger, EngineMode engineMode = EngineMode.Default, int? maximumEngines = null)
    {
        var maximum = maximumEngines ?? Environment.ProcessorCount;
        if (maximum <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumEngines));

        _logger = logger ?? NullLogger.Instance;
        _slots = new SemaphoreSlim(maximum, maximum);
        var provider = new DefaultObjectPoolProvider { MaximumRetained = maximum };
        _pool = provider.Create(new EnginePolicy(dataPath, language, engineMode, _logger));
        _logger.LogDebug("Engine pool created with a maximum of {MaximumEngines} engines", maximum);
    }
    #endregion

    #region ProcessAsync
    /// <summary>
    ///     Recognizes an image and returns its text. The caller retains ownership of the image.
    /// </summary>
    /// <remarks>
    ///     Cancellation applies while waiting for an engine and before recognition starts; native OCR
    ///     cannot be interrupted after it begins. The page is disposed before the engine is reused.
    /// </remarks>
    public async Task<string> ProcessAsync(Image image, PageSegMode? pageSegMode = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        lock (_sync)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EnginePool));
        }

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _shutdown.Token);
        try
        {
            await _slots.WaitAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new ObjectDisposedException(nameof(EnginePool));
        }

        Engine engine = null;
        var reusable = false;
        try
        {
            lock (_sync)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(EnginePool));
                cancellationToken.ThrowIfCancellationRequested();
                engine = _pool.Get();
            }

            _logger.LogDebug("Engine {EngineId} acquired from pool", engine.GetHashCode());
            cancellationToken.ThrowIfCancellationRequested();
            string text;
            using (var page = engine.Process(image, pageSegMode))
                text = page.Text;

            engine.ClearAdaptiveClassifier();
            reusable = true;
            return text;
        }
        finally
        {
            try
            {
                if (engine != null)
                {
                    lock (_sync)
                    {
                        if (reusable && !_disposed)
                        {
                            _pool.Return(engine);
                            _logger.LogDebug("Engine {EngineId} returned to pool", engine.GetHashCode());
                        }
                        else
                            engine.Dispose();
                    }
                }
            }
            finally
            {
                _slots.Release();
            }
        }
    }
    #endregion

    #region Dispose
    /// <summary>
    ///     Closes the pool and disposes idle engines; active scans clean up on return.
    /// </summary>
    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
                return;
            _disposed = true;
        }

        try
        {
            ((IDisposable)_pool).Dispose();
        }
        finally
        {
            _shutdown.Cancel();
        }
        _logger.LogDebug("Engine pool closed; active engines will be disposed on return");
    }
    #endregion

    /// <summary>
    ///     Defines the pooling policy for <see cref="Engine"/> instances.
    /// </summary>
    /// <remarks>Creates engines with the configured data path, language, and engine mode. Disposed engines
    /// are not returned to the pool.</remarks>
    private sealed class EnginePolicy : PooledObjectPolicy<Engine>
    {
        #region Fields
        /// <summary>
        ///     The path to the data files.
        /// </summary>
        private readonly string _dataPath;

        /// <summary>
        ///     The language associated with this instance.
        /// </summary>
        private readonly Language _language;

        /// <summary>
        ///     The engine mode associated with this instance.
        /// </summary>
        private readonly EngineMode _engineMode;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnginePolicy"/> class.
        /// </summary>
        /// <param name="dataPath">The path to the data files.</param>
        /// <param name="language">The language to use.</param>
        /// <param name="engineMode">The engine mode to use.</param>
        /// <param name="logger">The logger for engine lifecycle events.</param>
        public EnginePolicy(string dataPath, Language language, EngineMode engineMode, ILogger logger)
        {
            _dataPath = dataPath;
            _language = language;
            _engineMode = engineMode;
            _logger = logger;
        }
        #endregion

        #region Create
        /// <summary>
        ///     Creates a new engine.
        /// </summary>
        /// <returns>A new <see cref="Engine"/> instance.</returns>
        public override Engine Create()
        {
            var engine = new Engine(_dataPath, _language, _engineMode);
            var created = false;
            try
            {
                var engineId = engine.GetHashCode();
                _logger.LogDebug("Engine {EngineId} created", engineId);
                engine.Disposed += (_, _) => _logger.LogDebug("Engine {EngineId} disposed", engineId);
                created = true;
                return engine;
            }
            finally
            {
                if (!created)
                    engine.Dispose();
            }
        }
        #endregion

        #region Return
        /// <summary>
        ///     Indicates whether the engine can be returned.
        /// </summary>
        /// <param name="engine">The engine to evaluate.</param>
        /// <returns>true if the engine is not disposed; otherwise, false.</returns>
        public override bool Return(Engine engine) => !engine.IsDisposed;
        #endregion
    }
}