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
    private WatchedAsset<Effect> _practiceShader;
    private WatchedAsset<Effect> _combineShader;
    private Material _grayscaleEffect;
    private Material _multiLightShader;
    private Material _multiLightA;
    private MultiLightAData multiLightAData;
    private Texture2D _treeTexture;
    private Texture2D _whiteDotTexture;
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

        _practiceShader = Content.Watch<Effect>("PracticeShader");
        _combineShader = Content.Watch<Effect>("CombineShader");
        _grayscaleEffect = Content.WatchMaterial("TemplateShader");
        _grayscaleEffect.SetParameter("Saturation", 1);
        _multiLightShader = Content.WatchMaterial("MultiLightShader");
        _multiLightShader.SetParameter("LightMapSize", new Vector2(80, 80));

        MultiLightALoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _practiceShader.TryRefresh(out _);
        _combineShader.TryRefresh(out _);
        _grayscaleEffect.Update();
        _multiLightShader.Update();
        _multiLightA.Update();


        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            foreach (var p in _multiLightA.Effect.Parameters)
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

        // Light map
        MultiLightATest();


        // draw to screen
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);


        if (Keyboard.GetState().IsKeyDown(Keys.L))
        {
            _combineShader.Asset.Parameters["LightMapTexture"].SetValue(_lightMap);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, effect: _combineShader.Asset);
            _spriteBatch.Draw(_mainRenderTarget, new Rectangle(800, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
            _spriteBatch.End();
        }



        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
        _spriteBatch.Draw(_lightMap, new Rectangle(0, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.Draw(_mainRenderTarget, new Rectangle(400, 0, 400, 400), new Rectangle(0, 0, 80, 80), Color.White);
        _spriteBatch.End();


        base.Draw(gameTime);
    }

    private void MultiLightALoadContent()
    {
        _multiLightA = Content.WatchMaterial("MultiLightA");
        multiLightAData = new();
        multiLightAData.Colors = [Color.Gold.ToVector4(), Color.White.ToVector4(), Color.Cyan.ToVector4()];
        multiLightAData.Radii = [20, 20, 25];
        multiLightAData.Positions = [new(20, 30), new(60, 60), new(30, 55)];
        multiLightAData.InnerRatios = [.1f, .5f, .4f];
        multiLightAData.Intensities = [1, 1, 1];
        
    }
    private void MultiLightATest()
    {
        GraphicsDevice.SetRenderTarget(_lightMap);
        GraphicsDevice.Clear(Color.Transparent);
        Point mousePosition = Mouse.GetState().Position;
        

        _multiLightA.Effect.Parameters["CanvasSize"].SetValue(new Vector2(_lightMap.Width, _lightMap.Height));
        _multiLightA.Effect.Parameters["Position"].SetValue([.. multiLightAData.Positions]);
        _multiLightA.Effect.Parameters["Radii"].SetValue([.. multiLightAData.Radii]);
        _multiLightA.Effect.Parameters["InnerRatios"].SetValue([.. multiLightAData.InnerRatios]);
        _multiLightA.Effect.Parameters["Intensities"].SetValue([.. multiLightAData.Intensities]);
        _multiLightA.Effect.Parameters["Colors"].SetValue([.. multiLightAData.Colors]);
        _multiLightA.Effect.Parameters["LightCount"].SetValue(3);
        _multiLightA.Effect.Parameters["Position"].SetValue([new Vector2(15, 15), new Vector2(60f, 60), new Vector2(mousePosition.X, mousePosition.Y)/5f]);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, effect: _multiLightA.Effect);

        _spriteBatch.Draw(_whiteDotTexture, new Rectangle(0, 0, 80, 80), new Rectangle(0, 0, 1, 1), Color.White);
        _spriteBatch.End();

    }

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
    public struct MultiLightAData
    {
        public List<Vector2> Positions = [];
        public List<float> Radii = [];
        public List<float> InnerRatios = [];
        public List<float> Intensities = [];
        public List<Vector4> Colors =[];

        public MultiLightAData()
        {
        }
    }


}
