#version 300 es
precision mediump float;
precision highp int;

struct GlobalParams_std140
{
    highp mat4 uMVP;
    highp vec4 uColor;
};

uniform GlobalParams_std140 globalParams;

layout(location = 0) out highp vec4 entryPointParam_fragmentMain;

void main()
{
    entryPointParam_fragmentMain = globalParams.uColor;
}

