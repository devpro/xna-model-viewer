using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

class ChaseCamera : CameraBase
{
    public Vector3 Position { get; private set; }
    public Vector3 Target { get; private set; }

    public Vector3 FollowTargetPosition { get; private set; }
    public Vector3 FollowTargetRotation { get; private set; }

    public Vector3 PositionOffset { get; set; }
    public Vector3 TargetOffset { get; set; }

    public Vector3 RelativeCameraRotation { get; set; }

    float springiness = .15f;

    public float Springiness
    {
        get { return springiness; }
        set { springiness = MathHelper.Clamp(value, 0, 1); }
    }

    public ChaseCamera(GraphicsDevice graphicsDevice)
        : base(graphicsDevice)
    {
        //this.PositionOffset = PositionOffset;
        //this.TargetOffset = TargetOffset;
        //this.RelativeCameraRotation = RelativeCameraRotation;
    }

    public void Move(Vector3 NewFollowTargetPosition,
        Vector3 NewFollowTargetRotation)
    {
        this.FollowTargetPosition = NewFollowTargetPosition;
        this.FollowTargetRotation = NewFollowTargetRotation;
    }

    public void Rotate(Vector3 RotationChange)
    {
        this.RelativeCameraRotation += RotationChange;
    }

    public override void Update()
    {
        // Sum the rotations of the model and the camera to ensure it
        // is rotated to the correct position relative to the model's
        // rotation
        Vector3 combinedRotation = FollowTargetRotation +
                                   RelativeCameraRotation;

        // Calculate the rotation matrix for the camera
        Matrix rotation = Matrix.CreateFromYawPitchRoll(
            combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

        // Calculate the position the camera would be without the spring
        // value, using the rotation matrix and target position
        Vector3 desiredPosition = FollowTargetPosition +
                                  Vector3.Transform(PositionOffset, rotation);

        // Interpolate between the current position and desired position
        Position = Vector3.Lerp(Position, desiredPosition, Springiness);

        // Calculate the new target using the rotation matrix
        Target = FollowTargetPosition +
                 Vector3.Transform(TargetOffset, rotation);

        // Obtain the up vector from the matrix
        Vector3 up = Vector3.Transform(Vector3.Up, rotation);

        // Recalculate the view matrix
        View = Matrix.CreateLookAt(Position, Target, up);
    }
}
