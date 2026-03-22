VERTEX_SHADER
{
    #include "Core.glsl"

    layout(location = 0) in vec3 position;
    layout(location = 1) in vec2 uv;
    layout(location = 2) in uint color; 
    layout(location = 3) in int texIndex; 

    out vec2 fragUV;
    flat out int fragTexIndex;            // flat = no interpolation between vertices
    out vec4 vColor;
    uniform mat4 uVP;
    out vec2 worldUV;

    void main() 
    { 
        fragUV = uv;   
        worldUV = position.xy * 0.1;
        fragTexIndex = texIndex; 
        
        vColor = LAM_UnpackColor(color); 
        gl_Position = uVP * vec4(position, 1.0); 
    }
} 

FRAGMENT_SHADER
{
    uniform sampler2D uTextures[8];
    #define LAM_TEXTURE_ARRAY uTextures
    #include "Core.glsl"

    in vec2 fragUV; 
    in vec4 vColor;

    flat in int fragTexIndex; 
    out vec4 fragColor;
    uniform vec2 express;

    void main()  
    {
        fragColor = LAM_SampleTextureArray(fragTexIndex, fragUV) * vColor;

        if(fragColor.a <= 0.000001)
        {
            discard;
        }
    }
}
