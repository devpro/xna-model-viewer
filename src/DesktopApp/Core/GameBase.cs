using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DesktopApp.Core;

public abstract class GameBase : Microsoft.Xna.Framework.Game
{
    public static readonly string CONTENT_PATH  = "Content";

    public static readonly string MODELS_PATH   = "Models";

    public static readonly string TEXTURES_PATH = "Textures";

    public static readonly string FONTS_PATH    = "Fonts";

    private SpriteBatch _spriteBatch;
    private SpriteFont  _gameFont;

    /// <summary>
    /// Graphics Device Manager
    /// </summary>
    public GraphicsDeviceManager Graphics { get; private set; }

    public abstract string GameVersion { get; }

    public GameBase()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    #region Protected Game Methods

    /// <summary>
    ///
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gameFont = Content.Load<SpriteFont>(string.Format("sample", FONTS_PATH));
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload all content.
    /// </summary>
    protected override void UnloadContent()
    {
        Components.Clear(); // needed?
        Content.Unload(); // needed?
    }

    #endregion // Protected Game Methods

    /// <summary>
    /// Load a 2D texture
    /// </summary>
    /// <param name="textureName"></param>
    /// <returns></returns>
    protected Texture2D LoadTexture2D(string textureFileName)
    {
        try
        {
            return Content.Load<Texture2D>(string.Format("{0}\\{1}", TEXTURES_PATH, textureFileName));
        }
        catch (Exception exc)
        {
            // TODO: Microsoft.Xna.Framework.Content.ContentLoadException
            Debug.WriteLine(string.Format("Cannot load {0} texture. Message: \"{1}\"", textureFileName, exc.Message), "LoadContent");
            throw;
        }
    }

    /// <summary>
    /// Allows the game to exit
    /// </summary>
    protected void CheckIfGameMustExit(KeyboardState kbState)
    {
        if (kbState.IsKeyDown(Keys.Escape)) { this.Exit(); }
    }

    /// <summary>
    /// Set game window name (title)
    /// </summary>
    /// <param name="windowName">Name to be set</param>
    protected void SetWindowName(string windowName)
    {
        Window.Title = string.Format("{0} - v{1}", windowName, GameVersion);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="text"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
    protected void DrawString(string text, Vector2 position, Color color)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.DrawString(_gameFont, text, position, color);
        _spriteBatch.End();
    }
}
