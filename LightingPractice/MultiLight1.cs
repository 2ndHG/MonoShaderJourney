using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightingPractice;

public class MultiLight1
{
    private RenderTarget2D _lightMap;
    private Material _multiLightA;
    private Texture2D _whiteDot;
    private MultiLightAData _data;
    private int _width;
    private int _height;

    public RenderTarget2D LightMap => _lightMap;

    public MultiLight1(int width = 80, int height = 80)
    {
        _width = width;
        _height = height;
        _lightMap = Game1.CreateRenderTarget2D(width, height);
    }

    public void LoadContent()
    {
        var content = Game1.GameContentManager;
        _multiLightA = content.WatchMaterial("MultiLightA");
        _whiteDot = content.Load<Texture2D>("WhiteDot");

        _data = new MultiLightAData
        {
            Colors = [Color.Gold.ToVector4(), Color.White.ToVector4(), Color.Aqua.ToVector4()],
            Radii = [60f, 20f, 25f],
            Positions = [new Vector2(20, 30), new Vector2(60, 60), new Vector2(30, 55)],
            InnerRatios = [0.1f, 0.3f, 0.4f],
            Intensities = [0f, 1f, 0f]
        };
    }

    public void Update(GameTime gameTime)
    {
        // allow materials to refresh in DEBUG via their internal logic
        _multiLightA?.Update();
    }

    public void Draw(GameTime gameTime)
    {
        var gd = Game1.GameInstance.GraphicsDevice;

        // render the light map
        gd.SetRenderTarget(_lightMap);
        gd.Clear(Color.Transparent);

        var mousePos = Mouse.GetState().Position;

        _multiLightA.Effect.Parameters["CanvasSize"]?.SetValue(new Vector2(_lightMap.Width, _lightMap.Height));
        if (_data.Positions != null)
            _multiLightA.Effect.Parameters["Position"]?.SetValue(_data.Positions.ToArray());
        if (_data.Radii != null)
            _multiLightA.Effect.Parameters["Radii"]?.SetValue(_data.Radii.ToArray());
        if (_data.InnerRatios != null)
            _multiLightA.Effect.Parameters["InnerRatios"]?.SetValue(_data.InnerRatios.ToArray());
        if (_data.Intensities != null)
            _multiLightA.Effect.Parameters["Intensities"]?.SetValue(_data.Intensities.ToArray());
        if (_data.Colors != null)
            _multiLightA.Effect.Parameters["Colors"]?.SetValue(_data.Colors.ToArray());

        // set LightCount and update the 3rd light to follow the mouse (scaled to map)
        _multiLightA.Effect.Parameters["LightCount"]?.SetValue(3);
        var scaledMouse = new Vector2(mousePos.X, mousePos.Y) / 5f; // match Game1 scaling used in original sample
        _multiLightA.Effect.Parameters["Position"]?.SetValue(new[] { new Vector2(15, 15), new Vector2(60f, 60f), scaledMouse });

        Game1.GameSpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, effect: _multiLightA.Effect);
        Game1.GameSpriteBatch.Draw(_whiteDot, new Rectangle(0, 0, _width, _height), new Rectangle(0, 0, 1, 1), Color.White);
        Game1.GameSpriteBatch.End();

        // restore backbuffer
        gd.SetRenderTarget(null);
    }
}


public struct MultiLightAData
{
    public List<Vector2> Positions = [];
    public List<float> Radii = [];
    public List<float> InnerRatios = [];
    public List<float> Intensities = [];
    public List<Vector4> Colors = [];

    public MultiLightAData()
    {
    }
}