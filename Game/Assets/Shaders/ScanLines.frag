#version 330 core

in vec2 screenUV;
in vec4 vColor;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;

uniform float uScanlineIntensity;
uniform float uScanlineSpacing;

void main()
{
    vec2 texel = 1.0 / uScreenSize;

    // Scanlines in screen space
    vec2 pixelPos = screenUV * uScreenSize;
    float line = mod(pixelPos.y, uScanlineSpacing);
    float scanlineFactor = 1.0 - smoothstep(0.0, 1.0, line) * uScanlineIntensity;

    vec3 color = texture(uScreenGrabTex, screenUV).rgb;
    color *= scanlineFactor;

    fragColor = vec4(color, 1.0) * vColor;
}
