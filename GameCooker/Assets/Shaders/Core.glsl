
vec4 GFS_UnpackColor(uint c) 
{
    float r = float((c >> 24) & 0xFFu) / 255.0;
    float g = float((c >> 16) & 0xFFu) / 255.0;
    float b = float((c >>  8) & 0xFFu) / 255.0;
    float a = float( c        & 0xFFu) / 255.0;
    return vec4(r, g, b, a);
}

#ifdef GFS_TEXTURE_ARRAY
vec4 GFS_SampleTextureArray(int index, vec2 uv)
{
    switch(index)
    {
        case 0:  return texture(GFS_TEXTURE_ARRAY[0], uv);
        case 1:  return texture(GFS_TEXTURE_ARRAY[1], uv);
        case 2:  return texture(GFS_TEXTURE_ARRAY[2], uv);
        case 3:  return texture(GFS_TEXTURE_ARRAY[3], uv);
        case 4:  return texture(GFS_TEXTURE_ARRAY[4], uv);
        case 5:  return texture(GFS_TEXTURE_ARRAY[5], uv);
        case 6:  return texture(GFS_TEXTURE_ARRAY[6], uv);
        case 7:  return texture(GFS_TEXTURE_ARRAY[7], uv);
        case 8:  return texture(GFS_TEXTURE_ARRAY[8], uv);
        case 9:  return texture(GFS_TEXTURE_ARRAY[9], uv);
        case 10: return texture(GFS_TEXTURE_ARRAY[10], uv);
        case 11: return texture(GFS_TEXTURE_ARRAY[11], uv);
        case 12: return texture(GFS_TEXTURE_ARRAY[12], uv);
        case 13: return texture(GFS_TEXTURE_ARRAY[13], uv);
        case 14: return texture(GFS_TEXTURE_ARRAY[14], uv);
        /*case 15: return texture(GFS_TEXTURE_ARRAY[15], uv);
        case 16: return texture(GFS_TEXTURE_ARRAY[16], uv);
        case 17: return texture(GFS_TEXTURE_ARRAY[17], uv);
        case 18: return texture(GFS_TEXTURE_ARRAY[18], uv);
        case 19: return texture(GFS_TEXTURE_ARRAY[19], uv);
        case 20: return texture(GFS_TEXTURE_ARRAY[20], uv);
        case 21: return texture(GFS_TEXTURE_ARRAY[21], uv);
        case 22: return texture(GFS_TEXTURE_ARRAY[22], uv);
        case 23: return texture(GFS_TEXTURE_ARRAY[23], uv);
        case 24: return texture(GFS_TEXTURE_ARRAY[24], uv);
        case 25: return texture(GFS_TEXTURE_ARRAY [25], uv);
        case 26: return texture(GFS_TEXTURE_ARRAY [26], uv);
        case 27: return texture(GFS_TEXTURE_ARRAY [27], uv);
        case 28: return texture(GFS_TEXTURE_ARRAY [28], uv);
        case 29: return texture(GFS_TEXTURE_ARRAY [29], uv);*/
    }

    // fallback color if out of range
    return vec4(1.0, 0.0, 1.0, 1.0); 
}
#endif