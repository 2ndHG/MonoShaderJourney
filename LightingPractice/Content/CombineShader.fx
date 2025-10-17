#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// sampler SpriteSampler : register(s0); // SpriteBatch.Draw傳進來的MainTexture
float AmbientStrength = 0.3; // 0~1 環境光比例，可從 C# 傳入
sampler SpriteSampler = sampler_state
{
    Texture = <SpriteTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D LightMapTexture : register(t1);
sampler LightMapSampler = sampler_state
{
    Texture = <LightMapTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 mainColor = tex2D(SpriteSampler, texCoord);    // 從SpriteBatch來的畫面
    float4 lightColor = tex2D(LightMapSampler, texCoord); // 燈光貼圖

    float3 additionalLight = lightColor * 0.2;
    mainColor.rgb *= (float3(0.7, 0.7, 0.5) + lightColor.rgb*0.8);
    mainColor.rgb += additionalLight;
    return mainColor;
}

technique SpriteDrawing{
    pass P0{
        PixelShader = compile PS_SHADERMODEL MainPS();
}
}
;
