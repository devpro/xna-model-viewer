using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp;

public class SimpleCamera
{
    private const float FieldOfView = MathHelper.PiOver4;

    private const float NearPlaneDistance = 1.0f;

    private const int FarPlaneDistance = 10000;

    public Vector3 CameraTarget     { get; private set; }

    public Vector3 CameraUpVector   { get; private set; }

    public Vector3 CameraPosition   { get; set; }

    public Matrix  ProjectionMatrix { get; private set; }

    public Matrix  ViewMatrix
    {
        get { return Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUpVector); }
    }

    public SimpleCamera(GraphicsDevice graphicsDevice)
    {
        CameraPosition = Vector3.Zero;
        CameraTarget   = Vector3.Zero;
        CameraUpVector = Vector3.Up;

        var aspectRatio = graphicsDevice.Viewport.AspectRatio;
        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            FieldOfView, aspectRatio, NearPlaneDistance, FarPlaneDistance
        );
    }
}
