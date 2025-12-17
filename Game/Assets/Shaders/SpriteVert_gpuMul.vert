  #version 330 core
  layout(location = 0) in vec3 position;
  layout(location = 1) in vec2 uv;
  layout(location = 2) in uint color; 
  layout(location = 3) in int texIndex; 
  layout(location = 4) in float rotation; 
  layout(location = 5) in vec2 scale; 
  
  out vec2 fragUV;
  flat out int fragTexIndex;            // flat = no interpolation between vertices
  out vec4 vColor;
  uniform mat4 uVP;
  out vec2 worldUV;

  vec4 unpackColor(uint c) 
  {
      float r = float((c >> 24) & 0xFFu) / 255.0;
      float g = float((c >> 16) & 0xFFu) / 255.0;
      float b = float((c >>  8) & 0xFFu) / 255.0;
      float a = float( c        & 0xFFu) / 255.0;
      return vec4(r,g,b,a);
  }
  
  void main() 
  {
      fragUV = uv;
      worldUV = position.xy * 0.1;
      fragTexIndex = texIndex; 
      vColor = unpackColor(color);

      vec3 p = position * vec3(scale, 1.0);

      // rotate
      float c = cos(rotation);
      float s = sin(rotation);
      p = vec3(
          p.x * c - p.y * s,
          p.x * s + p.y * c,
          0
      );

     // translate
     vec3 worldPos = p;


     gl_Position = uVP * vec4(position, 1.0);
  }