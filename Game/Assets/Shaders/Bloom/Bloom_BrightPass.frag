#version 330 core

in vec2 screenUV;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform float uThreshold; 
uniform float uKnee;      

void main()
{
    vec3 color = texture(uScreenGrabTex, screenUV).rgb;

    float brightness = dot(color, vec3(0.2126, 0.7152, 0.0722));

    float soft = clamp((brightness - uThreshold) / uKnee, 0.0, 1.0);

    vec3 bloom = color * soft;

    fragColor = vec4(bloom, 1.0);
}
