using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace VeynEngine;

/// <summary>
/// The game window is the main abstraction of any Veyn game. Each game window is a separate window that can be opened and closed at will.
/// </summary>
/// <param name="title">The title of the window.</param>
/// <param name="width">The starting width of the window.</param>
/// <param name="height">The starting height of the window.</param>
/// <param name="resizable">Whether the window should be resizable.</param>
/// <param name="visible">Whether the window should be visible.</param>
/// <param name="focused">Whether the window should be focused.</param>
/// <param name="fullscreen">Whether the window should be fullscreen.</param>
/// <param name="borderless">Whether the window should be borderless.</param>
/// <param name="minimized">Whether the window should be minimized.</param>
/// <param name="maximized">Whether the window should be maximized.</param>
/// <param name="vsync">Whether the window should have vsync enabled.</param>
public abstract unsafe class GameWindow(
    string title, 
    int width, 
    int height,
    bool resizable = true,
    bool visible = true,
    bool focused = true,
    bool fullscreen = false,
    bool borderless = false,
    bool minimized = false,
    bool maximized = false,
    bool vsync = false
) {
    private static int _windowReferenceCount;
    private static bool _glfwInitialized;
    
    /// <summary>
    /// The title of the window.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            Glfw.SetWindowTitle(Handle, value);
        }
    }
    
    private string _title = title;
    
    /// <summary>
    /// The width of the window.
    /// </summary>
    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            Glfw.SetWindowSize(Handle, value, _height);
        }
    }
    
    private int _width = width;
    
    /// <summary>
    /// The height of the window.
    /// </summary>
    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            Glfw.SetWindowSize(Handle, _width, value);
        }
    }
    
    private int _height = height;
    
    /// <summary>
    /// Whether the window is resizable.
    /// </summary>
    public bool Resizable => resizable;

    /// <summary>
    /// Whether the window is visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (value)
                Glfw.ShowWindow(Handle);
            else
                Glfw.HideWindow(Handle);
        }
    }
    
    private bool _visible = visible;
    
    /// <summary>
    /// Whether the window is focused.
    /// </summary>
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            Glfw.FocusWindow(value ? Handle : null);
        }
    }
    
    private bool _focused = focused;
    
    /// <summary>
    /// Whether the window is fullscreen.
    /// </summary>
    public bool Fullscreen
    {
        get => _fullscreen;
        set
        {
            _fullscreen = value;
            
            if (value)
            {
                var monitor = Glfw.GetPrimaryMonitor();
                var mode = Glfw.GetVideoMode(monitor);
                
                Glfw.SetWindowMonitor(Handle, monitor, 0, 0, mode->Width, mode->Height, mode->RefreshRate);
            }
            else
            {
                Glfw.SetWindowMonitor(Handle, null, 0, 0, _width, _height, 0);
            }
        }
    }
    
    private bool _fullscreen = fullscreen;
    
    /// <summary>
    /// Whether the window is borderless.
    /// </summary>
    public bool Borderless => borderless;
    
    /// <summary>
    /// Whether the window is minimized.
    /// </summary>
    public bool Minimized
    {
        get => _minimized;
        set
        {
            _minimized = value;
            
            if (_minimized)
                Glfw.IconifyWindow(Handle);
            else
                Glfw.RestoreWindow(Handle);
        }
    }
    
    private bool _minimized = minimized;
    
    /// <summary>
    /// Whether the window is maximized.
    /// </summary>
    public bool Maximized
    {
        get => _maximized;
        set
        {
            _maximized = value;
            
            if (_maximized)
                Glfw.MaximizeWindow(Handle);
            else
                Glfw.RestoreWindow(Handle);
        }
    }
    
    private bool _maximized = maximized;
    
    /// <summary>
    /// Whether vsync is enabled.
    /// </summary>
    public bool VSync
    {
        get => _vsync;
        set
        {
            _vsync = value;
            Glfw.SwapInterval(value ? 1 : 0);
        }
    }
    
    private bool _vsync = vsync;
    
    /// <summary>
    /// The game loop of the window. Can be used to schedule tasks and update the game state. Is null until the
    /// window gets created.
    /// </summary>
    public GameLoop Loop { get; private set; } = null!;
    
    /// <summary>
    /// A reference to the GLFW API.
    /// </summary>
    public Glfw Glfw { get; } = Glfw.GetApi();

    /// <summary>
    /// A reference to the OpenGL API.
    /// </summary>
    public GL Gl { get; set; } = null!;
    
    /// <summary>
    /// The internal window handle.
    /// </summary>
    public WindowHandle* Handle { get; private set; }

    /// <summary>
    /// Runs the window. This will initialize GLFW, create the window and start the game loop.
    /// </summary>
    public void Run()
    {
        _windowReferenceCount++;
        
        Loop = new GameLoop(this);
        
        OnLoad();
        
        if (!_glfwInitialized)
        {
            if (!Glfw.Init())
                throw new VeynEngineException("Failed to initialize GLFW.");
        }
        
        Glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        Glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        Glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        
        if (resizable)
            Glfw.WindowHint(WindowHintBool.Resizable, true);
        
        if (_visible)
            Glfw.WindowHint(WindowHintBool.Visible, true);
        
        if (_focused)
            Glfw.WindowHint(WindowHintBool.Focused, true);
        
        if (borderless)
            Glfw.WindowHint(WindowHintBool.Decorated, false);
        
        if (_minimized)
            Glfw.WindowHint(WindowHintBool.Iconified, true);
        
        if (_maximized)
            Glfw.WindowHint(WindowHintBool.Maximized, true);
        
        Handle = Glfw.CreateWindow(_width, _height, _title, null, null);

        if (Handle == null)
        {
            var error = Glfw.GetError(out _);
            
            throw new VeynEngineException("Failed to create GLFW window.", new VeynGlfwException(error));
        }
        
        var screen = Glfw.GetPrimaryMonitor();
        var x = (Glfw.GetVideoMode(screen)->Width - _width) / 2;
        var y = (Glfw.GetVideoMode(screen)->Height - _height) / 2;
        
        Glfw.SetWindowPos(Handle, x, y);
        Glfw.MakeContextCurrent(Handle);
        Gl = GL.GetApi(Glfw.GetProcAddress);
        Gl.Viewport(0, 0, (uint)_width, (uint)_height);
        Glfw.SwapInterval(_vsync ? 1 : 0);

        Glfw.SetWindowSizeCallback(Handle, (_, width, height) =>
        {
            _width = width;
            _height = height;
        });
        
        Glfw.SetWindowCloseCallback(Handle, _ =>
        {
            if (!OnClosing())
            {
                Glfw.SetWindowShouldClose(Handle, false);
                return;
            }
            
            InternalClose();
        });
        
        OnStart();
        
        Loop.Start();
    }
    
    /// <summary>
    /// Gets called when the window is being opened.
    /// </summary>
    protected virtual void OnLoad() { }
    
    /// <summary>
    /// Gets called when all the initialization and loading is complete and the window is ready to be displayed.
    /// </summary>
    protected virtual void OnStart() { }
    
    /// <summary>
    /// Gets called when one tick of the game loop is completed. This should be used for updating the game state.
    /// </summary>
    /// <param name="deltaTime">The time in seconds since the last tick.</param>
    protected internal virtual void OnTick(float deltaTime) { }
    
    /// <summary>
    /// Gets called when the window should update. This should be used for any rendering logic.
    /// </summary>
    /// <param name="deltaTime">The time in seconds since the last update.</param>
    protected internal virtual void OnRender(float deltaTime) { }
    
    /// <summary>
    /// Gets called when the window is being closed. This event can be cancelled by returning false aborting the closing process.
    /// </summary>
    /// <returns>
    /// True, if the window should be closed. False, if the window should not be closed.
    /// </returns>
    protected virtual bool OnClosing() => true;
    
    /// <summary>
    /// Gets called when the window is closed.
    /// </summary>
    protected virtual void OnClosed() { }
    
    /// <summary>
    /// Closes the window.
    /// </summary>
    /// <param name="force">If true, the window definitely closes. This will not trigger the <see cref="OnClosing"/> event.</param>
    /// <param name="terminate">If true, the whole game will be terminated after the window is closed.</param>
    public void Close(bool force = true, bool terminate = false)
    {
        if (!force)
        {
            if (!OnClosing())
                return;
        }
        
        Glfw.SetWindowShouldClose(Handle, true);
        InternalClose(terminate);
    }

    private void InternalClose(bool terminate = false)
    {
        Loop.Stop();
        Glfw.DestroyWindow(Handle);
        OnClosed();

        Handle = null;
        _windowReferenceCount--;

        if (!terminate)
        {
            if (_windowReferenceCount != 0)
            {
                return;
            }
        }
        
        Glfw.Terminate();
        _glfwInitialized = false;
    }
}