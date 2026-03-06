#version 330 core
in vec2 screenUV;
in vec4 vColor;
out vec4 fragColor;
uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;
uniform float uScanlineIntensity;
uniform float uScanlineSpacing;
uniform float uPhosphorGlow;
uniform float uBrightness;
uniform float uContrast;
uniform vec3 uRGBBalance;
uniform float uJitterStrength;

void main()
{
    vec2 uv = screenUV;

    // Jitter
    uv.x += sin(uTime.x * 15.0 + uv.y * 120.0) * 0.0005 * uJitterStrength;

    // Scanlines 
    float line = mod(screenUV.y * uScreenSize.y, uScanlineSpacing);
    float scanlineFactor = mix(1.0, 1.0 - uScanlineIntensity, step(1.0, line));

    vec3 color = texture(uScreenGrabTex, uv).rgb * scanlineFactor;

    // Phosphor glow
    float texelY = 1.0 / uScreenSize.y;
    float halfGlow = uPhosphorGlow * 0.5;
    color += texture(uScreenGrabTex, uv + vec2(0.0, texelY)).rgb * halfGlow;
    color += texture(uScreenGrabTex, uv - vec2(0.0, texelY)).rgb * halfGlow;

    // Color grading
    color *= uRGBBalance;
    color = (color - 0.5) * uContrast + 0.5;
    color *= uBrightness;

    fragColor = vec4(color, 1.0) * vColor;
}