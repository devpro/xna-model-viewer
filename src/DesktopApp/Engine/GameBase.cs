using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Engine;

public abstract class GameBase : Game
{
    private SpriteBatch _spriteBatch;

    private SpriteFont _gameFont;

    protected GraphicsDeviceManager Graphics { get; private set; }

    protected abstract string GameVersion { get; }

    protected GameBase()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gameFont = Content.Load<SpriteFont>($"Fonts/sample");
    }

    protected override void UnloadContent()
    {
        Components.Clear(); // needed?
        Content.Unload(); // needed?
    }

    protected Texture2D LoadTexture2D(string textureFileName)
    {
        try
        {
            return Content.Load<Texture2D>($"Textures/{textureFileName}");
        }
        catch (Exception exc)
        {
            // TODO: Microsoft.Xna.Framework.Content.ContentLoadException
            Debug.WriteLine($"Cannot load {textureFileName} texture. Message: \"{exc.Message}\"", "LoadContent");
            throw;
        }
    }

    protected void SetWindowName(string windowName)
    {
        Window.Title = $"{windowName} - v{GameVersion}";
    }

    protected void DrawString(string text, Vector2 position, Color color)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.DrawString(_gameFont, text, position, color);
        _spriteBatch.End();
    }
}
