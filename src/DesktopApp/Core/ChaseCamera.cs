using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

internal class ChaseCamera(GraphicsDevice graphicsDevice) : CameraBase(graphicsDevice)
{
    private Vector3 Position { get; set; }

    private Vector3 Target { get; set; }

    private Vector3 FollowTargetPosition { get; set; }

    private Vector3 FollowTargetRotation { get; set; }

    public Vector3 PositionOffset { get; init; }

    public Vector3 TargetOffset { get; init; }

    public Vector3 RelativeCameraRotation { get; set; }

    private float _springiness = .15f;

    public float Springiness
    {
        get { return _springiness; }
        set { _springiness = MathHelper.Clamp(value, 0, 1); }
    }

    public void Move(Vector3 newFollowTargetPosition, Vector3 newFollowTargetRotation)
    {
        this.FollowTargetPosition = newFollowTargetPosition;
        this.FollowTargetRotation = newFollowTargetRotation;
    }

    public void Rotate(Vector3 rotationChange)
    {
        this.RelativeCameraRotation += rotationChange;
    }

    public override void Update()
    {
        // Sum the rotations of the model and the camera to ensure it is rotated to the correct position relative to the model's rotation
        var combinedRotation = FollowTargetRotation + RelativeCameraRotation;

        // Calculate the rotation matrix for the camera
        var rotation = Matrix.CreateFromYawPitchRoll(combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

        // Calculate the position the camera would be without the spring
        // value, using the rotation matrix and target position
        var desiredPosition = FollowTargetPosition + Vector3.Transform(PositionOffset, rotation);

        // Interpolate between the current position and desired position
        Position = Vector3.Lerp(Position, desiredPosition, Springiness);

        // Calculate the new target using the rotation matrix
        Target = FollowTargetPosition + Vector3.Transform(TargetOffset, rotation);

        // Obtain the up vector from the matrix
        var up = Vector3.Transform(Vector3.Up, rotation);

        // Recalculate the view matrix
        View = Matrix.CreateLookAt(Position, Target, up);
    }
}
