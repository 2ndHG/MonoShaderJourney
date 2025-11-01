using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightingPractice;

public class Game1 : Game
{
    public static Game1 GameInstance { get; private set; }
    public static SpriteBatch GameSpriteBatch { get; private set; }
    public static GraphicsDevice GameGraphicsDevice { get; private set; }
    public static ContentManager GameContentManager { get; private set; }
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private ShadowTest _shadowTest;
    

    // content
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        GameInstance = this;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 480;
        _graphics.PreferredBackBufferHeight = 810;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        GameContentManager = Content;
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        GameGraphicsDevice = GraphicsDevice; 
        GameSpriteBatch = _spriteBatch;

        _shadowTest = new ShadowTest();
        _shadowTest.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _shadowTest.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _shadowTest.Draw(gameTime);
    }

    public static RenderTarget2D CreateRenderTarget2D(int width, int height)
    {
        return new(
            GameInstance.GraphicsDevice,
            width,
            height,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.PreserveContents
        );
    }
   


}
