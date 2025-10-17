#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float2 LightPosition = float2(.8f, .8f);
float LightRadius = .25f;

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
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates)  * input.Color;
	
 	float2 PixelPos = input.TextureCoordinates;
    float distance = length(PixelPos - LightPosition);
 	float attenuation = saturate(1.0 - (distance / 0.8f));
	color *= attenuation;

	// float4 lightMultiplier = float4(attenuation, attenuation, attenuation, .7f);
    float2 anotherLightCoordinates = float2(.75f, .25f);
    float distance2 = length(PixelPos - anotherLightCoordinates);
 	float attenuation2 = saturate(1.0 - (distance2 / 0.8f));
    float4 anotherLightColor = float4(1, 1, 0, 1) * attenuation2;
    color += anotherLightColor;

	return color;
	
	// return color;
    // return finalLightColor; 
}

technique SpriteDrawing{
    pass P0{
        PixelShader = compile PS_SHADERMODEL MainPS();
}
}
;
