#version 330 core

in vec2 fragUV;
in vec2 screenUV;
in vec4 vColor;
in vec2 worldUV;

flat in int fragTexIndex;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform sampler2D uStarsTex;
uniform sampler2D uFrameTex;

uniform vec2  uScreenSize;
uniform vec3  uTime;

uniform float uDistortionAmount;

uniform vec3  uOutlineColor;
uniform float uOutlineThickness; 
uniform bool  uDotted;
uniform float uDotSpacing;

uniform float uPixelationAmount;

void main()
{
    float Ty = uTime.y;

    float a = (screenUV.y * 3.1 + Ty * 0.3);
    float b = (screenUV.x * 3.1 + Ty * 0.2);
    float c = (screenUV.x + screenUV.y) * 10.0 + Ty * 0.5;

    float wave1 = sin(a * 40.0);
    float wave2 = cos(b * 35.0);
    float wave3 = sin(c);

    float da = uDistortionAmount;

    vec2 wobble;
    wobble.x = wave1 * da + wave3 * da * 0.2;
    wobble.y = wave2 * da + wave3 * da * 0.1;

    vec2 baseUV = screenUV + wobble;

    if (uPixelationAmount > 1.0)
    {
        vec2 pix = uScreenSize / uPixelationAmount;
        baseUV = (floor(screenUV * pix) + 0.5) / pix + wobble;
    }

    vec3 base = texture(uScreenGrabTex, baseUV).rgb;
    vec4 outline = texture(uFrameTex, fragUV);

    vec3 stars = vec3(texture(uStarsTex, worldUV * 2.5 + vec2(0, Ty * 0.2)).a);

    vec3 rgb = base.rgb + outline.rgb * outline.a + vec3(stars);

    float alpha = step(0.1, rgb.r);

    fragColor = vec4(rgb, alpha) * vColor;
}
