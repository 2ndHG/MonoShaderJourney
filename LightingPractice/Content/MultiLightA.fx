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
int LightCount = 0;
float2 CanvasSize = float2(1, 1);

float2 Position[MAX_LIGHTS]; // in pixel
float Radii[MAX_LIGHTS];
float InnerRatios[MAX_LIGHTS];
float Intensities[MAX_LIGHTS];
float4 Colors[MAX_LIGHTS];


Texture2D SpriteTexture;

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
	float3 finalColor = float3(0.0, 0.0, 0.0);
	// float widthHeightRatio = canvasSize.x / canvasSize.y;
	for(int i=0; i<LightCount; ++i) {
		// float2 lightCenterPosition = Position[i] / canvasSize;
		// float2 uvPosition = input.TextureCoordinates / canvasSize;
		// uvPosition.x *= widthHeightRatio;
		// float radius = Radii[i] * widthHeightRatio;
		
		float2 pixelPosition = input.TextureCoordinates * CanvasSize;
		float d = length(pixelPosition - Position[i]);
		float radius = max(Radii[i], 1);

		//d -= InnerRatios[i] * radius;
		d = saturate((radius-d)/radius);
		d = smoothstep(0, 1-InnerRatios[i], d);
        // float t = saturate(1.0 - d / radius);
		finalColor += Colors[i].rgb * d * Intensities[i];
	}
	return float4(finalColor, 1.0);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};