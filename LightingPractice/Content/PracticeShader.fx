#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float mousePosition = .5f;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);

    float4 manipulated = color * input.Color;

    float distanceToMouse = abs(input.TextureCoordinates - mousePosition);


    // manipulated.r = 1;
    float bright = (1 - abs(input.TextureCoordinates - mousePosition)) * .8f;
    manipulated.r *= bright;
    manipulated.g *= bright;
    manipulated.b *= bright;
    
    return manipulated;
}

technique SpriteDrawing{
    pass P0{
        PixelShader = compile PS_SHADERMODEL MainPS();
}
}
;
