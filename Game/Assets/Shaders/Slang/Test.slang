struct VertexInput
{
    float3 position : POSITION;
    float2 uv       : TEXCOORD0;
};

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 uv       : TEXCOORD0;
    nointerpolation int texIndex : TEXCOORD1; 
};

uniform float4x4 uMVP;
uniform float4 uColor;

uniform Sampler2D gTex;          // Texture

VertexOutput vertexMain(VertexInput input)
{
    VertexOutput output;
    output.position = mul(float4(input.position, 1.0), uMVP);
    output.uv = input.uv;
    output.texIndex = 0; // Example integer value
    return output;
}

float4 fragmentMain(VertexOutput input) : SV_Target
{
    return gTex.Sample(input.uv);
}
