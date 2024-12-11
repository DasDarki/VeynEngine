using Silk.NET.OpenGL;
using VeynEngine;

namespace VeynExample;

internal static class Program
{
    private static void Main()
    {
        var game = new ExampleGame();
        game.Run();
    }

    public class ExampleGame() : GameWindow("Test Game", 800, 600)
    {
        protected override void OnRender(float deltaTime)
        {
            Gl.ClearColor(MathF.Sin(Loop.TotalElapsedSeconds), MathF.Cos(Loop.TotalElapsedSeconds), 0.2f, 1.0f);
            Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            unsafe
            {
                Glfw.SwapBuffers(Handle);
            }
        }
    }
}