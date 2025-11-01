#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

static const int MAX_LIGHTS = 4;

// Configs
// int LightCount = 0;
float2 CanvasSize = float2(1, 1);

float2 LightPosition; 	// in pixel
// float Radii[MAX_LIGHTS];		// in pixel
// float InnerRatios[MAX_LIGHTS];	// 0 ~ 1, the pixel has distance  has full color of Colors[i]
// float Intensities[MAX_LIGHTS];
// float4 Colors[MAX_LIGHTS];

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

Texture2D BlockerTexture;

sampler BlockerSampler = sampler_state
{
    Texture = <BlockerTexture>;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 lightSourceUV = LightPosition / CanvasSize;
	float2 pixelUV = input.TextureCoordinates;

	// direction and distance
	float2 dir = pixelUV - lightSourceUV;
	float distanceToSource = length(dir);

	float2 middlePoint = input.TextureCoordinates * 0.8 + lightSourceUV * 0.2;
	float blockerAlpha = tex2D(BlockerSampler, middlePoint).a;
	

	return float4(float3(0.0, 0.0, 0.0), blockerAlpha * (1.0-distanceToSource) *0.5) + tex2D(SpriteTextureSampler, input.TextureCoordinates) * 0.0000001;
}

float4 MainPS2(VertexShaderOutput input) : COLOR
{
    float2 lightSourceUV = LightPosition / CanvasSize;
    float2 pixelUV = input.TextureCoordinates;

    float2 dir = pixelUV - lightSourceUV;
    float distanceToSource = length(dir);

    float k = 0.5;
    float scale = 1.0 + k * distanceToSource;

    float2 blockerUV = lightSourceUV + dir / scale - dir *0.4;

    float blockerAlpha = tex2D(BlockerSampler, blockerUV).a * distanceToSource ;

    float attenuation = 1.0- distanceToSource;
	attenuation = smoothstep(0.5, 1.0, attenuation);
    float shadow = blockerAlpha * attenuation;

    return float4(float3(0.0, 0.0, 0.0), shadow) 
         + tex2D(SpriteTextureSampler, input.TextureCoordinates) * 0.0000001;
}

technique SpriteDrawing{
	pass P0{
		PixelShader = compile PS_SHADERMODEL MainPS2();
}
}
;