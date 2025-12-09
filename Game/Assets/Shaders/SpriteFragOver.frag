#version 330 core

uniform sampler2D uTextures[15]; 
in vec2 fragUV;
in vec4 vColor;

flat in int fragTexIndex;
out vec4 fragColor;

void main()
{
    vec4 color = texture(uTextures[fragTexIndex], fragUV) * vColor;
    color.rgb *= 0.7f;
    fragColor = color;

    if(fragColor.a <= 0.000001)
    {
        discard;
    }
}