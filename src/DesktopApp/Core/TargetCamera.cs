using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

class TargetCamera(GraphicsDevice graphicsDevice) : CameraBase(graphicsDevice)
{
    public Vector3 Position { get; set; }
    public Vector3 Target   { get; set; }

    public override void Update()
    {
        View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
    }
}
