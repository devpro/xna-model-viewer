using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

class FreeCamera : CameraBase
{
    public float   Yaw         { get; set; }
    public float   Pitch       { get; set; }
    public Vector3 Position    { get; set; }
    public Vector3 Target      { get; private set; }
    public Vector3 Translation { get; private set; }

    public FreeCamera(GraphicsDevice graphicsDevice)
        : base(graphicsDevice)
    {
        Translation = Vector3.Zero;
    }

    public void Rotate(float yawChange, float pitchChange)
    {
        this.Yaw   += yawChange;
        this.Pitch += pitchChange;
    }

    public void Move(Vector3 translation)
    {
        this.Translation += translation;
    }

    public override void Update()
    {
        // Calculate the rotation matrix
        var rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0);

        // Offset the position and reset the translation
        Translation  = Vector3.Transform(Translation, rotation);
        Position    += Translation;
        Translation  = Vector3.Zero;

        // Calculate the new target
        var forward = Vector3.Transform(Vector3.Forward, rotation);
        Target = Position + forward;

        // Calculate the up vector
        var up = Vector3.Transform(Vector3.Up, rotation);

        // Calculate the view matrix
        View = Matrix.CreateLookAt(Position, Target, up);
    }
}
