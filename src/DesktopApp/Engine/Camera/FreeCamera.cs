using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Engine.Camera;

public class FreeCamera(GraphicsDevice graphicsDevice) : CameraBase(graphicsDevice)
{
    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public Vector3 Position { get; set; }

    private Vector3 Target { get; set; }

    private Vector3 Translation { get; set; } = Vector3.Zero;

    public void Rotate(float yawChange, float pitchChange)
    {
        Yaw += yawChange;
        Pitch += pitchChange;
    }

    public void Move(Vector3 translation)
    {
        Translation += translation;
    }

    public override void Update()
    {
        // calculates the rotation matrix
        var rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0);

        // offsets the position and reset the translation
        Translation = Vector3.Transform(Translation, rotation);
        Position += Translation;
        Translation = Vector3.Zero;

        // calculates the new target
        var forward = Vector3.Transform(Vector3.Forward, rotation);
        Target = Position + forward;

        // calculates the up vector
        var up = Vector3.Transform(Vector3.Up, rotation);

        // calculates the view matrix
        View = Matrix.CreateLookAt(Position, Target, up);
    }
}
