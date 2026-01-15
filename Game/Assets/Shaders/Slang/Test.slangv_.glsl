#version 300 es

struct GlobalParams_std140
{
    mat4 uMVP;
    vec4 uColor;
};

uniform GlobalParams_std140 globalParams;

layout(location = 0) in vec3 input_position;

void main()
{
    gl_Position = globalParams.uMVP * vec4(input_position, 1.0);
}

