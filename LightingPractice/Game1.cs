using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightingPractice;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // content
    private RenderTarget2D _lightMap;
    private RenderTarget2D _mainRenderTarget;
    private Effect _lightGeneratorEffect;
    private Effect _templateEffect;
    private WatchedAsset<Effect> _practiceShader;
    private WatchedAsset<Effect> _lightingShader;
    private WatchedAsset<Effect> _combineShader;
    private Material _grayscaleEffect;
    private Material _multiLightShader;
    private List<LightData> _lights = [];
    private Texture2D _treeTexture;
    private Texture2D _whiteDotTexture;
    private List<Vector2> lightUVs;
    private List<float> lightRadii;
    private List<Vector4> lightColors;
    private List<float> lightIntensities;

    private Color _ambientColor = Color.Aqua;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 400;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _lightMap = CreateRenderTarget2D(80, 80);
        _mainRenderTarget = CreateRenderTarget2D(80, 80);

        // TODO: use this.Content to load your game content here
        _treeTexture = Content.Load<Texture2D>("Trees");
        _whiteDotTexture = Content.Load<Texture2D>("WhiteDot");

        // _templateEffect = Content.Load<Effect>("TemplateShader");
        _practiceShader = Content.Watch<Effect>("PracticeShader");
        _lightingShader = Content.Watch<Effect>("LightingShader");
        _combineShader = Content.Watch<Effect>("CombineShader");
        _grayscaleEffect = Content.WatchMaterial("TemplateShader");
        _grayscaleEffect.SetParameter("Saturation", 1);
        _multiLightShader = Content.WatchMaterial("MultiLightShader");
        _multiLightShader.SetParameter("LightMapSize", new Vector2(80, 80));

        lightUVs = [new(20f, 20f), new(40f, 40f), new(30f, 65f)];
        lightColors = [Color.Aqua.ToVector4(), Color.Honeydew.ToVector4(), Color.GreenYellow.ToVector4()];
        lightRadii = [10, 30, 15];
        lightIntensities = [.5f, .8f, 1];
    }

    protected override void Update(GameTime gameTime)
    {
        _practiceShader.TryRefresh(out _);
        _lightingShader.TryRefresh(out _);
        _combineShader.TryRefresh(out _);
        _grayscaleEffect.Update();
        _multiLightShader.Update();


        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            foreach (var p in _multiLightShader.Effect.Parameters)
            {
                Console.WriteLine(p.Name);
            }
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        Point mousePos = Mouse.GetState().Position;
        GraphicsDevice.SetRenderTarget(_mainRenderTarget);
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_treeTexture, new Rectangle(0, 0, 80, 80), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.End();
        // lightUVs[1] = new Vector2(mousePos.X / 400f, mousePos.Y / 400f);
        _multiLightShader.Effect.Parameters["LightPositions"]?.SetValue([.. lightUVs]);
        _multiLightShader.Effect.Parameters["LightRadii"]?.SetValue([.. lightRadii]);
        _multiLightShader.Effect.Parameters["LightColors"]?.SetValue([.. lightColors]);
        _multiLightShader.Effect.Parameters["PositionsArePixels"]?.SetValue(1);
        _multiLightShader.Effect.Parameters["LightIntensities"]?.SetValue([..lightIntensities]);

        _multiLightShader.Effect.Parameters["LightCount"]?.SetValue(3);
        // Light map
        GraphicsDevice.SetRenderTarget(_lightMap);
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, effect: _multiLightShader.Effect);

        _spriteBatch.Draw(_whiteDotTexture, new Rectangle(0, 0, 80, 80), new Rectangle(0, 0, 1, 1), Color.White);
        _spriteBatch.End();


        // draw to screen
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);


        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
        _spriteBatch.Draw(_lightMap, new Rectangle(0, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.Draw(_mainRenderTarget, new Rectangle(400, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.End();

        _combineShader.Asset.Parameters["LightMapTexture"].SetValue(_lightMap);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, effect: _combineShader.Asset);
        _spriteBatch.Draw(_mainRenderTarget, new Rectangle(800, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.End();


        base.Draw(gameTime);
    }

    const int MAX_LIGHTS = 16;

    private RenderTarget2D CreateRenderTarget2D(int width, int height)
    {
        return new(
            GraphicsDevice,
            width,
            height,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.PreserveContents
        );
    }
    public struct LightData
    {
        public Vector2 Position;
        public float Radius;
        public float Intensity;
        public Vector4 Color;
    }


}
