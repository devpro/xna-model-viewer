using System;
using System.Text;
using DesktopApp.Engine;
using DesktopApp.Engine.Camera;
using DesktopApp.Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DesktopApp;

public class SimpleModelViewerGame : GameBase
{
    private const float FieldOfView = MathHelper.PiOver4;

    private const float NearPlaneDistance = 1.0f;

    private const int FarPlaneDistance = 10000;

    private SimpleModel _model3D;

    private SimpleCamera _camera;

    private Matrix _matrixProjection;

    protected override string GameVersion
    {
        get { return $"1.0.0 (build {DateTime.Now:yyyy-MM-dd})"; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        _camera = new SimpleCamera(Graphics.GraphicsDevice) { CameraPosition = new Vector3(0.0f, 50.0f, 3000.0f) };
        // _camera = new SimpleCamera(Graphics.GraphicsDevice) { CameraPosition = new Vector3(0.0f, 50.0f, 5000.0f) };

        var aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
        _matrixProjection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlaneDistance, FarPlaneDistance);

        SetWindowName("Devpro Model Viewer");
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _model3D = new SimpleModel("Ship/ship", Content);
    }

    protected override void Update(GameTime gameTime)
    {
        var kbState = Keyboard.GetState();

        if (kbState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _model3D.RotationY += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.05f);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

        _model3D.Draw(_camera);

        var sb = new StringBuilder();
        sb.AppendLine($"Model position: X {_model3D.Position.X}, Y {_model3D.Position.Y}, Z {_model3D.Position.Z}");
        sb.AppendLine($"Model rotation: X {_model3D.Rotation.X}, Y {_model3D.Rotation.Y}, Z {_model3D.Rotation.Z}");
        sb.AppendLine($"Model size: {_model3D.GetModelSize()}");
        sb.AppendLine($"Camera position: X {_camera.CameraPosition.X}, Y {_camera.CameraPosition.Y}, Z {_camera.CameraPosition.Z}");
        DrawString(sb.ToString(), new Vector2(15, 15), Color.BlueViolet);

        base.Draw(gameTime);
    }
}
