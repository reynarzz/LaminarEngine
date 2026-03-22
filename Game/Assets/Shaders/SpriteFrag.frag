#version 330 core

uniform sampler2D uTextures[8];
in vec2 fragUV;
in vec4 vColor;

flat in int fragTexIndex;
out vec4 fragColor;

vec4 SampleIndexedTexture(int index, vec2 uv)
{
    switch(index)
    {
        case 0:  return texture(uTextures[0], uv);
        case 1:  return texture(uTextures[1], uv);
        case 2:  return texture(uTextures[2], uv);
        case 3:  return texture(uTextures[3], uv);
        case 4:  return texture(uTextures[4], uv);
        case 5:  return texture(uTextures[5], uv);
        case 6:  return texture(uTextures[6], uv);
        case 7:  return texture(uTextures[7], uv);
    }

    // fallback color if out of range
    return vec4(1.0, 0.0, 1.0, 1.0); 
}

void main()
{
    fragColor = SampleIndexedTexture(fragTexIndex, fragUV) * vColor;

    if(fragColor.a <= 0.000001)
    {
        discard;
    }
}