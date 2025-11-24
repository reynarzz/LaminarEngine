#version 330 core

in vec2 screenUV;
in vec4 vColor;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;

uniform vec3 uBackgroundColor = vec3(0.07);
uniform float uDistortionStrength = 0.08;
uniform float uEdgeSoftness = 0.001;
uniform float uScanlineIntensity = 0.45;
uniform float uScanlineSpacing = 4.0;
uniform float uPhosphorGlow = 0.4;
uniform float uRGBOffset = 1.0;
uniform float uBrightness = 1.0;
uniform float uContrast = 1.0;
uniform vec3 uRGBBalance = vec3(1.0, 0.8, 0.8);

void main()
{
    vec2 uv = screenUV * 2.0 - 1.0;

    vec2 distortedUV = uv + uv * (length(uv) * uDistortionStrength);
    distortedUV = distortedUV * 0.5 + 0.5;

    float blendX = smoothstep(0.0, uEdgeSoftness, distortedUV.x) *
                   (1.0 - smoothstep(1.0 - uEdgeSoftness, 1.0, distortedUV.x));
    float blendY = smoothstep(0.0, uEdgeSoftness, distortedUV.y) *
                   (1.0 - smoothstep(1.0 - uEdgeSoftness, 1.0, distortedUV.y));
    float edgeBlend = blendX * blendY;

    vec2 texel = 1.0 / uScreenSize;

    vec3 color;
    color.r = texture(uScreenGrabTex, clamp(distortedUV + vec2(uRGBOffset * texel.x, 0.0), 0.0, 1.0)).r;
    color.g = texture(uScreenGrabTex, clamp(distortedUV, 0.0, 1.0)).g;
    color.b = texture(uScreenGrabTex, clamp(distortedUV - vec2(uRGBOffset * texel.x, 0.0), 0.0, 1.0)).b;

    vec2 pixelPos = screenUV * uScreenSize;
    float line = mod(pixelPos.y, uScanlineSpacing);
    float scanlineFactor = 1.0 - smoothstep(0.0, 1.0, line) * uScanlineIntensity;
    color *= scanlineFactor;

    vec3 glowAbove = texture(uScreenGrabTex, clamp(distortedUV + vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    vec3 glowBelow = texture(uScreenGrabTex, clamp(distortedUV - vec2(0.0, texel.y), 0.0, 1.0)).rgb * uPhosphorGlow;
    color += (glowAbove + glowBelow) * 0.5;

    color *= uRGBBalance;

    color = mix(uBackgroundColor, color, edgeBlend);

    color = (color - 0.5) * uContrast + 0.5;
    color *= uBrightness;

    fragColor = vec4(color, 1.0) * vColor;
}
