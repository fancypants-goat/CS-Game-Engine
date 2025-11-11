#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

out vec2 texCoord;
out vec3 normal;

uniform mat4 model;

void main()
{
    gl_Position = model * vec4(aPosition, 1);
    texCoord = aTexCoord;
    normal = aNormal;
}