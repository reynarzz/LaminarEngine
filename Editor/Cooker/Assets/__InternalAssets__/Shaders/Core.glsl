
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
        case 8:  return texture(LAM_TEXTURE_ARRAY[8], uv);
        case 9:  return texture(LAM_TEXTURE_ARRAY[9], uv);
        case 10: return texture(LAM_TEXTURE_ARRAY[10], uv);
        case 11: return texture(LAM_TEXTURE_ARRAY[11], uv);
        case 12: return texture(LAM_TEXTURE_ARRAY[12], uv);
        case 13: return texture(LAM_TEXTURE_ARRAY[13], uv);
        case 14: return texture(LAM_TEXTURE_ARRAY[14], uv);
        /*case 15: return texture(LAM_TEXTURE_ARRAY[15], uv);
        case 16: return texture(LAM_TEXTURE_ARRAY[16], uv);
        case 17: return texture(LAM_TEXTURE_ARRAY[17], uv);
        case 18: return texture(LAM_TEXTURE_ARRAY[18], uv);
        case 19: return texture(LAM_TEXTURE_ARRAY[19], uv);
        case 20: return texture(LAM_TEXTURE_ARRAY[20], uv);
        case 21: return texture(LAM_TEXTURE_ARRAY[21], uv);
        case 22: return texture(LAM_TEXTURE_ARRAY[22], uv);
        case 23: return texture(LAM_TEXTURE_ARRAY[23], uv);
        case 24: return texture(LAM_TEXTURE_ARRAY[24], uv);
        case 25: return texture(LAM_TEXTURE_ARRAY [25], uv);
        case 26: return texture(LAM_TEXTURE_ARRAY [26], uv);
        case 27: return texture(LAM_TEXTURE_ARRAY [27], uv);
        case 28: return texture(LAM_TEXTURE_ARRAY [28], uv);
        case 29: return texture(LAM_TEXTURE_ARRAY [29], uv);*/
    }

    // fallback color if out of range
    return vec4(1.0, 0.0, 1.0, 1.0); 
}
#endif