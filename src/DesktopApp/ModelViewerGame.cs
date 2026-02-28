using System.Collections.Generic;
using DesktopApp.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DesktopApp;

public class ModelViewerGame : Game
  {
    private GraphicsDeviceManager           graphics;
    private SpriteBatch                     spriteBatch; // can be used to draw textures
    private Dictionary<string, CustomModel> models;
    private Dictionary<string, CameraBase>  cameras;
    private MouseState                      lastMouseState;
    private string                          currentCamera;

    public ModelViewerGame()
    {
      graphics              = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      models                = new Dictionary<string, CustomModel>();
      cameras               = new Dictionary<string, CameraBase>();
      currentCamera         = "free";

      graphics.PreferredBackBufferWidth  = 800;
      graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize()
    {
      models.Add("ship", new CustomModel(Content.Load<Model>("Ship/ship"), GraphicsDevice) {
        Position = Vector3.Zero, Rotation = Vector3.Zero, Scale = new Vector3(0.6f) }
      );
      models.Add("car", new CustomModel(Content.Load<Model>("Car/car"), GraphicsDevice) {
        Position = new Vector3(0, 100, 0), Rotation = Vector3.Zero, Scale = new Vector3(1.6f) }
      );
      models.Add("dude", new CustomModel(Content.Load<Model>("Dude/dude"), GraphicsDevice) {
        Position = new Vector3(0, 200, 0), Rotation = Vector3.Zero, Scale = new Vector3(1.6f) }
      );

      base.Initialize();
    }

    protected override void LoadContent()
    {
      spriteBatch     = new SpriteBatch(GraphicsDevice);

      cameras["free"] = new FreeCamera(GraphicsDevice) {
        Position = new Vector3(1000, 0, -2000),
        Yaw      = MathHelper.ToRadians(153),   // Turned around 153 degrees
        Pitch    = MathHelper.ToRadians(5)      // Pitched up 13 degrees
      };

      cameras["chase"] = new ChaseCamera(GraphicsDevice) {
        PositionOffset         = new Vector3(0, 400, 1500),
        TargetOffset           = new Vector3(0, 200, 0),
        RelativeCameraRotation = new Vector3(0, 0, 0)
      };

      lastMouseState = Mouse.GetState();
    }

    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    protected override void Update(GameTime gameTime)
    {
      var keyState = Keyboard.GetState();

      // allows the game to exit
      if (checkGameExit()) { this.Exit(); return; }

      if      (keyState.IsKeyDown(Keys.C)) { currentCamera = "chase"; }
      else if (keyState.IsKeyDown(Keys.F)) { currentCamera = "free"; }

      updateCamera(gameTime);

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      //var target = models["ship"].Position;

      var view       = cameras[currentCamera].View;
      var projection = cameras[currentCamera].Projection;

      foreach (var model in models.Values) {
        model.Draw(view, projection);
      }

      base.Draw(gameTime);
    }

    protected bool checkGameExit()
    {
      return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
        || Keyboard.GetState().IsKeyDown(Keys.Escape);
    }

    private void updateCamera(GameTime gameTime)
    {
      var mouseState = Mouse.GetState();
      var keyState   = Keyboard.GetState();

      // Determine how much the camera should turn
      float deltaX = (float)lastMouseState.X - (float)mouseState.X;
      float deltaY = (float)lastMouseState.Y - (float)mouseState.Y;

      // Rotate the camera
      ((FreeCamera)cameras["free"]).Rotate(deltaX * .01f, deltaY * .01f);

      var translation = Vector3.Zero;

      // Determine in which direction to move the camera
      if (keyState.IsKeyDown(Keys.Z)) { translation += Vector3.Forward;  }
      if (keyState.IsKeyDown(Keys.S)) { translation += Vector3.Backward; }
      if (keyState.IsKeyDown(Keys.Q)) { translation += Vector3.Left;     }
      if (keyState.IsKeyDown(Keys.D)) { translation += Vector3.Right;    }

      // Move 3 units per millisecond, independent of frame rate
      translation *= 3 * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

      // Move the camera
      ((FreeCamera)cameras["free"]).Move(translation);

      cameras["free"].Update();

      // Move the camera to the new model's position and orientation
      ((ChaseCamera)cameras["chase"]).Move(models["ship"].Position, models["ship"].Rotation);

      // Update the camera
      cameras["chase"].Update();

      // Update the mouse state
      lastMouseState = mouseState;
    }
  }
