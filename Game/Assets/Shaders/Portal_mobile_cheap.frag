#version 330 core

in vec2 fragUV;
in vec2 screenUV;
in vec4 vColor;
in vec2 worldUV;

flat in int fragTexIndex;
out vec4 fragColor;

uniform sampler2D uFrameTex;
uniform sampler2D uStarsTex;

uniform vec3 uTime;

void main()
{
    vec4 base = texture(uFrameTex, fragUV);

    float star = texture(uStarsTex, worldUV * 2.5 + vec2(0.0, uTime.y * 0.2)).a;

    vec3 rgb = base.rgb * base.a + vec3(star);

    float alpha = step(0.1, rgb.r);

    fragColor = vec4(rgb, alpha) * vColor;
}