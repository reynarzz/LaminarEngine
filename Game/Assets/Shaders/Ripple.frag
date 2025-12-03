#version 330 core

in vec2 fragUV;
in vec2 screenUV;
in vec4 vColor;
flat in int fragTexIndex;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;

// Ripple settings
uniform vec2  uRippleCenter    = vec2(0.5);
uniform float uRippleSpeed     = 2.0;
uniform float uRippleAmplitude = 0.01;
uniform float uRippleFrequency = 30.0;
uniform float uEdgeStart       = 0.3;
uniform float uEdgeEnd         = 0.7;

uniform float uRippleTL = 1.0;
uniform float uRippleTR = 1.0;
uniform float uRippleBL = 0.1;
uniform float uRippleBR = 1.0;

void main()
{
    vec2 dir  = screenUV - uRippleCenter;
    float dist = length(dir);

    float edgeFactor = smoothstep(uEdgeStart, uEdgeEnd, dist);

    vec2 uv01 = screenUV;

    float wTL = (1.0 - uv01.x) * (1.0 - uv01.y);
    float wTR =      uv01.x    * (1.0 - uv01.y);
    float wBL = (1.0 - uv01.x) *      uv01.y;
    float wBR =      uv01.x    *      uv01.y;

    float cornerStrength = wTL * uRippleTL +
          wTR * uRippleTR +
          wBL * uRippleBL +
          wBR * uRippleBR;

    float ripple = sin(dist * uRippleFrequency - uTime.x * uRippleSpeed)
        * uRippleAmplitude
        * edgeFactor
        * cornerStrength;

    vec2 uv = screenUV + normalize(dir) * ripple;

    vec3 color = texture(uScreenGrabTex, uv).rgb;

    fragColor = vec4(color, 1.0) * vColor;
}
