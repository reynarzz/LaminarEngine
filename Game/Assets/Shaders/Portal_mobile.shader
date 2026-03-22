VERTEX_SHADER
{
    #include "Core.glsl"

    layout(location = 0) in vec3 position;
    layout(location = 1) in vec2 uv;  
    layout(location = 2) in uint color;
    layout(location = 3) in int texIndex;
    layout(location = 4) in int vertIndex;

    out vec2 fragUV;
    out vec2 screenUV;
    out vec2 worldUV;

    flat out int fragTexIndex;            
    out vec4 vColor;
    uniform mat4 uVP;
    uniform mat4 uProjectionMatrix;

    void main() 
    {
        fragUV = uv;
        fragTexIndex = texIndex; 
        worldUV = position.xy * 0.1;
        vColor = LAM_UnpackColor(color);

        // Transform to clip space
        vec4 posEnd = uVP * vec4(position, 1.0);
        // Convert clip space NDC (divide by w)
        vec3 ndc = posEnd.xyz / posEnd.w;

        // NDC [-1,1] screen UV [0,1]
        screenUV = ndc.xy * 0.5 + 0.5;

        gl_Position = posEnd;
    }
} 

FRAGMENT_SHADER
{
    #define LAM_TEXTURE_ARRAY
    #include "Core.glsl"
     
    in vec2 fragUV;
    in vec2 screenUV;
    in vec4 vColor;
    in vec2 worldUV;

    flat in int fragTexIndex;
    out vec4 fragColor;

    uniform sampler2D uFrameTex;
    uniform sampler2D uStarsTex;

    uniform vec3 uTime;

    void main()
    {
        vec4 base = texture(uFrameTex, fragUV);

        float star = texture(uStarsTex, worldUV * 2.5 + vec2(0.0, uTime.y * 0.2)).a;

        vec3 rgb = base.rgb * base.a + vec3(star);

        float alpha = step(0.1, rgb.r);

        fragColor = vec4(rgb, alpha) * vColor;
    }
}
