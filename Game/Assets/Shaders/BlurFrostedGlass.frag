#version 330 core

in vec2 fragUV;
in vec2 screenUV;
in vec4 vColor;

flat in int fragTexIndex;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;
uniform float uPixelate;  

// Simple hash noise (no texture needed)
float hash(vec2 p)
{
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// 2D pseudo-random noise
float noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

void main()
{
    vec2 uv = screenUV;
    if (uPixelate > 0.0) 
    {
        vec2 pixelSize = uPixelate / uScreenSize; 
        uv = floor(uv / pixelSize) * pixelSize;
    }

    // Noise distortion
    float n = noise(uv * 20.0 + uTime.x * 0.1);
    float angle = n * 6.2831; 
    float amount = 0.006;
    vec2 offset = vec2(cos(angle), sin(angle)) * amount;

    // Multi-sample glass effect on pixelated UV
    vec3 color = vec3(0.0);
    color += texture(uScreenGrabTex, uv + offset * 0.5).rgb;
    color += texture(uScreenGrabTex, uv - offset * 0.5).rgb;
    color += texture(uScreenGrabTex, uv + offset * 1.0).rgb;
    color += texture(uScreenGrabTex, uv - offset * 1.0).rgb;
    color += texture(uScreenGrabTex, uv + offset.yx * 0.8).rgb;
    color += texture(uScreenGrabTex, uv - offset.yx * 0.8).rgb;
    color /= 6.0;

    fragColor = vec4(color, 1.0) * vColor;
}
