// simple_es.slang

struct VertexInput
{
    float3 position : POSITION;
};

struct VertexOutput
{
    float4 position : SV_Position;
};

// Loose uniforms 
uniform float4x4 uMVP;
uniform float4   uColor;
 
VertexOutput vertexMain(VertexInput input)
{
    VertexOutput output;
    output.position = mul(uMVP, float4(input.position, 1.0));
    return output;
}

float4 fragmentMain(VertexOutput input) : SV_Target
{
    return uColor;
}
