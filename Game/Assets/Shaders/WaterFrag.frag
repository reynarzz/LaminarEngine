#version 330 core

in vec2 fragUV;
in vec2 worldUV;

out vec4 fragColor;

uniform vec3  uTime;                     
uniform float uWaveAmplitude;
uniform float uWaveFrequency;
uniform float uWaveSpeed;
uniform float uNoiseScale;
uniform float uNoiseStrength;
uniform vec3  uWaterColor;
uniform float uOutlineHeight;
uniform vec3  uOutlineColor;
uniform float uOutlineThickness;
uniform sampler2D uParticles;

// Stable hash function — avoids NaN / denormal issues
float hash(float n) {
    return fract(fract(n * 0.1031) * 43758.5453123);
}

// Stable 1D noise
float noise(float x) {
    float i = floor(x);
    float f = fract(x);
    float u = f * f * (3.0 - 2.0 * f);
    return mix(hash(i), hash(i + 1.0), u);
}

void main()
{
    float randomAmp = uWaveAmplitude * mix(0.5, 1.5, noise(fragUV.x * uNoiseScale));

    float wave = sin(fragUV.x * uWaveFrequency + uTime.y * uWaveSpeed)
               * randomAmp;

    float wobble = (noise(fragUV.x * uNoiseScale * 0.3 + uTime.y * 0.5) - 0.5)
                 * uNoiseStrength;

    float topEdge = uOutlineHeight + wave + wobble;

    if (fragUV.y > topEdge)
        discard;

    float distanceToEdge = abs(fragUV.y - topEdge);
    float outlineFactor = step(uOutlineThickness, distanceToEdge);

    vec3 color = mix(uOutlineColor, uWaterColor, outlineFactor);
    //float particlesAlpha = texture(uParticles, worldUV * 3 - vec2(0, uTime.x * 0.2)).a;
    //vec3 pColor = vec3(1,1,0) * particlesAlpha;

    fragColor = vec4(color, 1.0);
}
