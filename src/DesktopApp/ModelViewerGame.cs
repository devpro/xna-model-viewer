using System.Collections.Generic;
using DesktopApp.Engine;
using DesktopApp.Engine.Camera;
using DesktopApp.Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DesktopApp;

public class ModelViewerGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly Dictionary<string, CustomModel> _models;
    private readonly Dictionary<string, CameraBase> _cameras;
    private MouseState _lastMouseState;
    private string _currentCamera;

    public ModelViewerGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _models = new Dictionary<string, CustomModel>();
        _cameras = new Dictionary<string, CameraBase>();
        _currentCamera = "free";

        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize()
    {
        _models.Add("ship1",
            new CustomModel(Content.Load<Model>("Ships/ship1"), GraphicsDevice)
            {
                Position = new Vector3(0, 100, 0),
                Rotation = Vector3.Zero,
                Scale = new Vector3(1.6f)
            }
        );

        // models.Add("ship", new CustomModel(Content.Load<Model>("Ship/ship"), GraphicsDevice) {
        //   Position = Vector3.Zero, Rotation = Vector3.Zero, Scale = new Vector3(0.6f) }
        // );
        // models.Add("car", new CustomModel(Content.Load<Model>("Car/car"), GraphicsDevice) {
        //   Position = new Vector3(0, 100, 0), Rotation = Vector3.Zero, Scale = new Vector3(1.6f) }
        // );
        // models.Add("dude", new CustomModel(Content.Load<Model>("Dude/dude_converted"), GraphicsDevice) {
        //   Position = new Vector3(0, 200, 0), Rotation = Vector3.Zero, Scale = new Vector3(1.6f) }
        // );

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _cameras["free"] = new FreeCamera(GraphicsDevice)
        {
            Position = new Vector3(1000, 0, -2000),
            Yaw = MathHelper.ToRadians(153),
            Pitch = MathHelper.ToRadians(5)
        };

        _cameras["chase"] = new ChaseCamera(GraphicsDevice)
        {
            PositionOffset = new Vector3(0, 400, 1500),
            TargetOffset = new Vector3(0, 200, 0),
            RelativeCameraRotation = new Vector3(0, 0, 0)
        };

        _lastMouseState = Mouse.GetState();
    }

    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    protected override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        if (keyState.IsKeyDown(Keys.C)) { _currentCamera = "chase"; }
        else if (keyState.IsKeyDown(Keys.F)) { _currentCamera = "free"; }

        UpdateCamera(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var target = _models["ship1"].Position;
        var view = _cameras[_currentCamera].View;
        var projection = _cameras[_currentCamera].Projection;

        foreach (var model in _models.Values)
        {
            model.Draw(view, projection);
        }

        base.Draw(gameTime);
    }

    private void UpdateCamera(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var keyState = Keyboard.GetState();

        // Determine how much the camera should turn
        var deltaX = (float)_lastMouseState.X - mouseState.X;
        var deltaY = (float)_lastMouseState.Y - mouseState.Y;

        // Rotate the camera
        ((FreeCamera)_cameras["free"]).Rotate(deltaX * .01f, deltaY * .01f);

        var translation = Vector3.Zero;

        // Determine in which direction to move the camera
        if (keyState.IsKeyDown(Keys.Z)) { translation += Vector3.Forward; }
        if (keyState.IsKeyDown(Keys.S)) { translation += Vector3.Backward; }
        if (keyState.IsKeyDown(Keys.Q)) { translation += Vector3.Left; }
        if (keyState.IsKeyDown(Keys.D)) { translation += Vector3.Right; }

        // Move 3 units per millisecond, independent of frame rate
        translation *= 3 * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        // Move the camera
        ((FreeCamera)_cameras["free"]).Move(translation);

        _cameras["free"].Update();

        // Move the camera to the new model's position and orientation
        ((ChaseCamera)_cameras["chase"]).Move(_models["ship1"].Position, _models["ship1"].Rotation);

        // Update the camera
        _cameras["chase"].Update();

        // Update the mouse state
        _lastMouseState = mouseState;
    }
}
