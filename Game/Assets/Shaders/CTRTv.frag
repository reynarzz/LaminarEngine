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
uniform float uAberrationStrength;   // chromatic aberration
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
        float jitter = sin(uTime.x * 15.0 + uv.y * 120.0)
                      * 0.0005 * uJitterStrength;
        uv.x += jitter;
    }

    vec2 uv01 = (uv + 1.0) * 0.5;

    float wTL = (1.0 - uv01.x) * (1.0 - uv01.y);
    float wTR =      uv01.x    * (1.0 - uv01.y);
    float wBL = (1.0 - uv01.x) *      uv01.y;
    float wBR =      uv01.x    *      uv01.y;

    float cornerDistortion =
          wTL * uCornerTL +
          wTR * uCornerTR +
          wBL * uCornerBL +
          wBR * uCornerBR;

    float finalDistortion = uDistortionStrength * cornerDistortion;

    vec2 distortedUV = uv + uv * (length(uv) * finalDistortion);
    distortedUV = distortedUV * 0.5 + 0.5;

    vec2 aberrShift = uv * (pow(length(uv), 1.7) * 0.002 * uAberrationStrength);

    vec2 uvR = distortedUV + aberrShift;
    vec2 uvG = distortedUV;
    vec2 uvB = distortedUV - aberrShift;

    uvR = clamp(uvR, 0.0, 1.0);
    uvG = clamp(uvG, 0.0, 1.0);
    uvB = clamp(uvB, 0.0, 1.0);

    float blendX = smoothstep(0.0, uEdgeSoftness, distortedUV.x) *
                   (1.0 - smoothstep(1.0 - uEdgeSoftness, 1.0, distortedUV.x));
    float blendY = smoothstep(0.0, uEdgeSoftness, distortedUV.y) *
                   (1.0 - smoothstep(1.0 - uEdgeSoftness, 1.0, distortedUV.y));
    float edgeBlend = blendX * blendY;

    vec2 texel = 1.0 / uScreenSize;

    vec3 color;
    color.r = texture(uScreenGrabTex, uvR + vec2(uRGBOffset * texel.x, 0.0)).r;
    color.g = texture(uScreenGrabTex, uvG).g;
    color.b = texture(uScreenGrabTex, uvB - vec2(uRGBOffset * texel.x, 0.0)).b;

    vec2 pixelPos = screenUV * uScreenSize;
    float line = mod(pixelPos.y, uScanlineSpacing);
    float scanlineFactor = 1.0 - smoothstep(0.0, 1.0, line) * uScanlineIntensity;
    color *= scanlineFactor;

    vec3 glowAbove = texture(uScreenGrabTex,
                             clamp(distortedUV + vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    vec3 glowBelow = texture(uScreenGrabTex,
                             clamp(distortedUV - vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    color += (glowAbove + glowBelow) * 0.5;

    if (uMaskStrength > 0.0) 
    {
        vec3 mask = texture(uMaskTexture, pixelPos * uMaskScale).rgb;
        color = mix(color, color * mask, uMaskStrength);
    }

    color *= uRGBBalance;

    if (uGlassReflectStrength > 0.0) 
    {
        float curve = pow(length(uv), 2.5);
        vec3 glassTint = vec3(0.05, 0.08, 0.10);
        color = mix(color, color + curve * glassTint, uGlassReflectStrength);
    }

    if (uVignetteStrength > 0.0) 
    {
        float vignette = pow(length(uv), 2.2);
        color *= (1.0 - vignette * uVignetteStrength);
    }

    if (uNoiseStrength > 0.0) 
    {
        float noise = fract(sin(dot(pixelPos, vec2(12.34, 78.91))) * 43758.5453);
        color += (noise - 0.5) * uNoiseStrength;
    }

    color = mix(uBackgroundColor, color, edgeBlend);

    color = (color - 0.5) * uContrast + 0.5;
    color *= uBrightness;

    fragColor = vec4(color, 1.0) * vColor;
}
