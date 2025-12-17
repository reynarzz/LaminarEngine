#version 330 core

in vec2 screenUV;
in vec4 vColor;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;

// CRT TV Controls
uniform vec3 uBackgroundColor;
uniform float uDistortionStrength;//0.03;
uniform float uCornerTL;
uniform float uCornerTR;
uniform float uCornerBL;
uniform float uCornerBR;
uniform float uEdgeSoftness;
uniform float uScanlineIntensity;
uniform float uScanlineSpacing;
uniform float uPhosphorGlow;
uniform float uRGBOffset;
uniform float uBrightness;
uniform float uContrast;
uniform vec3 uRGBBalance;

uniform float uGlassReflectStrength;   // curved CRT glass reflection
uniform float uMaskStrength;   // aperture grille / shadow mask
uniform float uMaskScale;   // scaling of CRT mask pattern
uniform float uNoiseStrength;   // analog noise level
uniform float uVignetteStrength;   // CRT glass vignette
uniform float uJitterStrength;   // horizontal beam jitter
uniform sampler2D uMaskTexture;              // CRT mask texture (optional)

void main()
{
    vec2 uv = screenUV * 2.0 - 1.0;

    if (uJitterStrength > 0.0) 
    {
        float jitter = sin(uTime.x * 15.0 + uv.y * 120.0) * 0.0005 * uJitterStrength;
        uv.x += jitter;
    }

    uv = uv * 0.5 + 0.5;

    vec2 pixelPos = screenUV * uScreenSize;
    float line = mod(pixelPos.y, uScanlineSpacing);
    float scanlineFactor = 1.0 - smoothstep(0.0, 1.0, line) * uScanlineIntensity;
    vec3 color = texture(uScreenGrabTex, uv).rgb * scanlineFactor;

    vec2 texel = 1.0 / uScreenSize;
    vec3 glowAbove = texture(uScreenGrabTex, clamp(uv + vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    vec3 glowBelow = texture(uScreenGrabTex, clamp(uv - vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    color += (glowAbove + glowBelow) * 0.5;
    color *= uRGBBalance;

    color = (color - 0.5) * uContrast + 0.5;
    color *= uBrightness;

    fragColor = vec4(color, 1.0) * vColor;
}
