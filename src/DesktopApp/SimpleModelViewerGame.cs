using System;
using System.Text;
using DesktopApp.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DesktopApp;

public class SimpleModelViewerGame : GameBase
  {
    private SimpleModel      _model3D;
    private SimpleCamera _camera;
    private Matrix       _matrixProjection;

    public override string GameVersion
    {
      get { return string.Format("1.0.0 (build {0})", DateTime.Now.ToString("yyyy-MM-dd")); }
    }

    protected override void Initialize()
    {
      base.Initialize();

      _camera = new SimpleCamera(Graphics.GraphicsDevice);
      _camera.CameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);

      float fieldOfView = MathHelper.PiOver4;
      float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
      float nearPlaneDistance = 1.0f;
      float farPlaneDistance = 10000;
      _matrixProjection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);

      SetWindowName("Devpro Model Viewer");
    }

    protected override void LoadContent()
    {
      base.LoadContent();

      string modelName = "p1_wedge";
      _model3D = new SimpleModel(modelName, Content);
    }

    protected override void Update(GameTime gameTime)
    {
      KeyboardState kbState = Keyboard.GetState();

      CheckIfGameMustExit(kbState);

      _model3D.RotationY += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.1f);

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

      _model3D.Draw(_camera);

      base.Draw(gameTime);

      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Format("Model position: X {0}, Y {1}, Z {2}", _model3D.Position.X, _model3D.Position.Y, _model3D.Position.Z));
      sb.AppendLine(string.Format("Model rotation: X {0}, Y {1}, Z {2}", _model3D.Rotation.X, _model3D.Rotation.Y, _model3D.Rotation.Z));
      sb.AppendLine(string.Format("Model size: {0}", _model3D.GetModelSize()));
      sb.AppendLine(string.Format("Camera position: X {0}, Y {1}, Z {2}", _camera.CameraPosition.X, _camera.CameraPosition.Y, _camera.CameraPosition.Z));
      DrawString(sb.ToString(), new Vector2(15, 15), Color.BlueViolet);
    }
  }
