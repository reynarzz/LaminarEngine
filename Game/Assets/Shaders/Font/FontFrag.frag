#version 330 core

uniform sampler2D uTextures[15]; //uniform sampler2D uTextures[{32}]

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
        case 8:  return texture(uTextures[8], uv);
        case 9:  return texture(uTextures[9], uv);
        case 10: return texture(uTextures[10], uv);
        case 11: return texture(uTextures[11], uv);
        case 12: return texture(uTextures[12], uv);
        case 13: return texture(uTextures[13], uv);
        case 14: return texture(uTextures[14], uv);
    }

    // fallback color if out of range
    return vec4(1.0, 0.0, 1.0, 1.0); 
}


void main()
{
    vec4 col = SampleIndexedTexture(fragTexIndex, fragUV);
    float a = step(0.1, col.a);

    fragColor = col.rgba * a * vColor;
}