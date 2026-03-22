vec4 LAM_UnpackColor(uint c) 
{
    float r = float((c >> 24) & 0xFFu) / 255.0;
    float g = float((c >> 16) & 0xFFu) / 255.0;
    float b = float((c >>  8) & 0xFFu) / 255.0;
    float a = float( c        & 0xFFu) / 255.0;
    return vec4(r, g, b, a);
}
float LAM_Hash(float n) 
{
    return fract(fract(n * 0.1031) * 43758.5453123);
}
 
float LAM_Hash(vec2 p)
{
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float LAM_Noise(float x) 
{
    float i = floor(x);
    float f = fract(x);
    float u = f * f * (3.0 - 2.0 * f);
    return mix(LAM_Hash(i), LAM_Hash(i + 1.0), u);
}

float LAM_Noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = LAM_Hash(i);
    float b = LAM_Hash(i + vec2(1.0, 0.0));
    float c = LAM_Hash(i + vec2(0.0, 1.0));
    float d = LAM_Hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

vec3 LAM_Luminance(vec3 color)
{
    return vec3(dot(color, vec3(0.299, 0.587, 0.114)));
}

vec4 LAM_Luminance(vec4 color)
{
    return vec4(LAM_Luminance(color.rgb), color.a);
}

#ifdef LAM_TEXTURE_ARRAY
vec4 LAM_SampleTextureArray(int index, vec2 uv)
{
    switch(index)
    {
        case 0:  return texture(LAM_TEXTURE_ARRAY[0], uv);
        case 1:  return texture(LAM_TEXTURE_ARRAY[1], uv);
        case 2:  return texture(LAM_TEXTURE_ARRAY[2], uv);
        case 3:  return texture(LAM_TEXTURE_ARRAY[3], uv);
        case 4:  return texture(LAM_TEXTURE_ARRAY[4], uv);
        case 5:  return texture(LAM_TEXTURE_ARRAY[5], uv);
        case 6:  return texture(LAM_TEXTURE_ARRAY[6], uv);
        case 7:  return texture(LAM_TEXTURE_ARRAY[7], uv);
    }

    // fallback color if out of range
    return vec4(1.0, 0.0, 1.0, 1.0); 
}
#endif