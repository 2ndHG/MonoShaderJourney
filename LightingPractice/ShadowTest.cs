using System;
using LightingPractice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class ShadowTest
{
    private RenderTarget2D _resultRT;
    private RenderTarget2D _blockerRT;

    private Texture2D _blockerTexture;
    private Texture2D _whiteDot;

    private Material _shadowEffect;
    public ShadowTest()
    {

    }

    public void LoadContent()
    {
        _resultRT = Game1.CreateRenderTarget2D(160, 90);
        _blockerRT = Game1.CreateRenderTarget2D(160, 90);
        _blockerTexture = Game1.GameContentManager.Load<Texture2D>("Balloon");
        _whiteDot = Game1.GameContentManager.Load<Texture2D>("WhiteDot");
        _shadowEffect = Game1.GameContentManager.WatchMaterial("Shadow");

        _shadowEffect.Effect.Parameters["LightPosition"]?.SetValue(new Vector2(30, 40));
        foreach (var p in _shadowEffect.Effect.Parameters)
        {
            Console.WriteLine(p.Name);
        }
    }

    public void Update(GameTime gameTime)
    {
        _shadowEffect.Update();
    }

    public void Draw(GameTime gameTime)
    {
        Game1.GameGraphicsDevice.SetRenderTarget(_blockerRT);
        Game1.GameSpriteBatch.Begin();
        Game1.GameSpriteBatch.Draw(_blockerTexture, new Vector2(0, 0), Color.White);
        Game1.GameSpriteBatch.End();

        Game1.GameGraphicsDevice.SetRenderTarget(_resultRT);
        Point mousePos = Mouse.GetState().Position;
        _shadowEffect.Effect.Parameters["CanvasSize"]?.SetValue(new Vector2(480, 270));
        _shadowEffect.Effect.Parameters["BlockerTexture"]?.SetValue(_blockerTexture);
        _shadowEffect.Effect.Parameters["LightPosition"]?.SetValue(new Vector2(mousePos.X, mousePos.Y));
        Game1.GameGraphicsDevice.Clear(Color.Transparent);
        Game1.GameSpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _shadowEffect.Effect);
        Game1.GameSpriteBatch.Draw(_whiteDot, new Rectangle(0, 0, 160, 90), Color.Yellow);
        Game1.GameSpriteBatch.End();
        // Game1.GameSpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _shadowEffect.Effect);
        // _shadowEffect.Effect.Parameters["LightPosition"]?.SetValue(new Vector2(-mousePos.X, mousePos.Y));
        // Game1.GameSpriteBatch.Draw(_whiteDot, new Rectangle(80, 0, 80, 90), Color.Yellow);
        // Game1.GameSpriteBatch.End();

        Game1.GameGraphicsDevice.SetRenderTarget(null);
        Game1.GameGraphicsDevice.Clear(Color.Black);
        Game1.GameSpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
        Game1.GameSpriteBatch.Draw(_resultRT, new Rectangle(0, 0, 480, 270), Color.White);
        Game1.GameSpriteBatch.Draw(_blockerRT, new Rectangle(0, 0, 480, 270), Color.White);
        Game1.GameSpriteBatch.End();

    }
}