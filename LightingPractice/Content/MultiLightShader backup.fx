#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


static const int MAX_LIGHTS = 4;

float2 LightPositions[MAX_LIGHTS]; 
float  LightRadii[MAX_LIGHTS]; 
float  LightIntensities[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];
int LightCount;


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
    float2 pixelPos = input.TexCoord;
    float3 result = float3(0, 0, 0);

    for(int i = 0; i < LightCount; i++)
    {
        float d = length(pixelPos - LightPositions[i]);
        // float atten = smoothstep(LightRadii[i], 0, d);
        float atten = saturate(1.0/( 400 * d * d *1.0) * (1 / LightRadii[i])) ;
        atten = smoothstep(0, LightRadii[i], atten);
        result += LightColors[i].rgb * atten;
    }

    return float4(result, 1);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
