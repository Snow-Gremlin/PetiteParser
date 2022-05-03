uniform mat4 objMat;
uniform mat4 viewMat;
uniform mat4 projMat;
uniform sampler2D bumpTxt;
uniform float offsetScalar;

attribute vec3 posAttr;
attribute vec3 normAttr;
attribute vec3 binmAttr;
attribute vec2 txtAttr;
attribute float weightAttr;

varying vec3 color;

// Given a color from a normal map texture find the normal vector.
vec3 bumpyNormal(vec3 color)
{
   vec3 n = normalize(objMat*vec4(normAttr, 0.0)).xyz;
   vec3 b = normalize(objMat*vec4(binmAttr, 0.0)).xyz;
   vec3 c = cross(b, n);
   b = cross(n, c);
   mat3 mat = mat3( b.x,  b.y,  b.z,
                   -c.x, -c.y, -c.z,
                    n.x,  n.y,  n.z);
   return mat * normalize(2.0*color - 1.0);
}

// This takes in a bunch of degenerate lines (line with start and stop the same)
// The start point has weight 0.0 and the end point has weight 1.0.
// The end point is extended out in the direction determined by the normal vector.
// The result is like a pin cushion showing the normal vectors for debugging. 
void main()
{
   color = texture2D(bumpTxt, txtAttr).rgb;
   vec4 pnt = projMat*viewMat*objMat*vec4(posAttr, 1.0);
   if (weightAttr > 0.5)
   {
      pnt += vec4(bumpyNormal(color)*offsetScalar, 0.0); 
   }
   gl_Position = pnt;                                   
}                                                       
