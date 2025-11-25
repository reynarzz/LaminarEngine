#version 330 core

in vec2 screenUV;
out vec4 fragColor;

uniform sampler2D uScreenGrabTex;
uniform float Amount;
uniform vec3 Color;

void main()
{
    vec3 color = mix(texture(uScreenGrabTex, screenUV).rgb, Color.rgb, Amount);

    fragColor = vec4(color, 1.0);
}
