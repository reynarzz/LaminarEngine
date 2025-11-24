#version 330 core

in vec2 fragUV;
in vec2 screenUV;
in vec4 vColor;
in vec2 worldUV;

flat in int fragTexIndex;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform vec2 uScreenSize;
uniform vec3 uTime;

// Amount of wobble distortion
uniform float uDistortionAmount = 0.0003;
uniform float uColorSplit = 0.0017;
uniform float uPixelationAmount = 0.0f;

void main()
{
    // Wobble distortion
    float wave1 = sin((screenUV.y * 3.1 + uTime.x * 0.3) * 40.0);
    float wave2 = cos((screenUV.x * 3.1 + uTime.x * 0.2) * 35.0);
    float wave3 = sin((screenUV.x + screenUV.y) * 10.0 + uTime.x * 0.5);

    vec2 wobble;
    wobble.x = wave1 * uDistortionAmount + wave3 * uDistortionAmount * 0.2;
    wobble.y = wave2 * uDistortionAmount + wave3 * uDistortionAmount * 0.1;

    // Pixelation
    // uPixelationAmount = how many pixels across the screen (e.g. 160 = heavy pixelation)
    vec2 pixelatedUV = screenUV;
    if (uPixelationAmount > 1.0)
    {
        vec2 pixelSize = vec2(1.0) / (uScreenSize / uPixelationAmount);
        pixelatedUV = floor(screenUV / pixelSize) * pixelSize + pixelSize * 0.5;
    }

    // Apply wobble and glitch to pixelated UVs
    vec2 baseUV = pixelatedUV + wobble;

    // Color channel split
    vec2 rUV = baseUV + vec2(uColorSplit, 0.0);
    vec2 gUV = baseUV;
    vec2 bUV = baseUV - vec2(uColorSplit, 0.0);

    vec3 base = vec3(
        texture(uScreenGrabTex, rUV).r,
        texture(uScreenGrabTex, gUV).g,
        texture(uScreenGrabTex, bUV).b
    );

    float aspect = uScreenSize.x / uScreenSize.y;

    fragColor = vec4(base, 1.0) * vColor;
}

