namespace VeynEngine;

/// <summary>
/// The game loop class has the job of managing and scheduling the game loop and incoming external tasks.
/// </summary>
public sealed class GameLoop(GameWindow window)
{
    /// <summary>
    /// The delta time between the last frame and the current frame.
    /// </summary>
    public float DeltaTime { get; private set; }
    
    /// <summary>
    /// The total elapsed seconds since glfw was initialized.
    /// </summary>
    public float TotalElapsedSeconds { get; private set; }
    
    private bool _running;

    /// <summary>
    /// Starts the game loop.
    /// </summary>
    /// <exception cref="VeynEngineException">Thrown when the game loop is already running.</exception>
    internal void Start()
    {
        if (_running)
            throw new VeynEngineException("The game loop is already running.");
        
        _running = true;
        
        while (_running)
        {
            RecalculateDeltaTime();
            
            window.OnTick(DeltaTime);
            window.Glfw.PollEvents();
            window.OnRender(DeltaTime);
        }
    }
    
    /// <summary>
    /// Stops the game loop.
    /// </summary>
    internal void Stop()
    {
        _running = false;
    }
    
    private void RecalculateDeltaTime()
    {
        var time = (float)window.Glfw.GetTime();
        
        DeltaTime = time - TotalElapsedSeconds;
        TotalElapsedSeconds = time;
    }
}