#version 330 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 uv;      
layout(location = 2) in uint color;
layout(location = 5) in vec3 lineDir; 

out vec4 vColor;

uniform mat4 uVP;
uniform mat4 uViewMatrix;
 float uHalfWidth = 0.1f;
uniform mat4 uProjectionMatrix;
vec4 unpackColor(uint c)
{
    return vec4(
        float((c >> 24) & 0xFFu) / 255.0,
        float((c >> 16) & 0xFFu) / 255.0,
        float((c >>  8) & 0xFFu) / 255.0,
        float( c        & 0xFFu) / 255.0
    );
}

void main()
{
      vColor = unpackColor(color);

    // World View
    vec3 viewPos = (uViewMatrix * vec4(position, 1.0)).xyz;
    vec3 viewDir = normalize((uViewMatrix * vec4(lineDir, 0.0)).xyz);

    // Camera looks down -Z in view space
    vec3 side = normalize(cross(viewDir, vec3(0.0, 0.0, -1.0)));

    // Extrude in view space
    viewPos += side * uv.x * uHalfWidth;

    // View  Clip (projection ONLY)
    gl_Position = uProjectionMatrix * vec4(viewPos, 1.0);
}
