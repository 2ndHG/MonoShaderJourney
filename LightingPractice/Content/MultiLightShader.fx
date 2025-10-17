#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

static const int MAX_LIGHTS = 4;

float innerRatio = 0.18;
float2 LightPositions[MAX_LIGHTS];
float LightRadii[MAX_LIGHTS];
float LightIntensities[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];
int LightCount;

float2 LightMapSize;    // render target size
int PositionsArePixels; // 0 or 1

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSOut
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(VSOut input) : COLOR
{
    float2 uv = input.TexCoord;
    float3 outLight = float3(0, 0, 0);

    for (int i = 0; i < LightCount; i++)
    {
        float2 lightUV = LightPositions[i];
        if (PositionsArePixels == 1)
        {
            lightUV = LightPositions[i] / LightMapSize;
        }

        float d = length(uv - lightUV);
        float radiusUV;
        if (PositionsArePixels == 1)
        {
            float maxDim = max(LightMapSize.x, LightMapSize.y);
            radiusUV = LightRadii[i] / maxDim;
        }
        else
        {
            radiusUV = LightRadii[i];
        }

        float innerUV = radiusUV * innerRatio;
        float atten = smoothstep(innerUV, radiusUV, d);

        float falloffPower = 1.5;
        atten = pow(atten, falloffPower);
        atten = saturate(atten);

        float3 contribution = LightColors[i].rgb * (1 - atten) * 1.5;

        outLight += contribution;
    }

    return float4(outLight, 1);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
