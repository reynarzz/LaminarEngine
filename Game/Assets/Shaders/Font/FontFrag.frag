#version 330 core

uniform sampler2D uTextures[15]; //uniform sampler2D uTextures[{32}]

in vec2 fragUV;
in vec4 vColor;

flat in int fragTexIndex;
out vec4 fragColor;

void main()
{
    vec4 col = texture(uTextures[0], fragUV);
    float a = step(0.1, col.a);

    fragColor = col.rgba * a * vColor;
}