shader_type spatial;

render_mode cull_disabled;

uniform sampler2D color: repeat_disable;
uniform bool animated;
uniform bool glow;

varying vec2 uv;
varying vec3 position;
varying vec3 worldPosition;
varying vec3 instanceColor;
varying float noiseValue;

const mat2 myt = mat2(vec2(.12121212, .13131313), vec2(-.13131313, .12121212));
const vec2 mys = vec2(1e4, 1e6);

vec2 rhash(vec2 uv1) {
  uv1 *= myt;
  uv1 *= mys;
  return fract(fract(uv1 / mys) * uv1);
}

vec3 hash(vec3 p) {
  return fract(sin(vec3(dot(p, vec3(1.0, 57.0, 113.0)),
                        dot(p, vec3(57.0, 113.0, 1.0)),
                        dot(p, vec3(113.0, 1.0, 57.0)))) *
               43758.5453);
}

float voronoi(const in vec2 point) {
  vec2 p = floor(point);
  vec2 f = fract(point);
  float res = 0.0;
  for (float j = -1.; j <= 1.; j++) {
    for (float i = -1.; i <= 1.; i++) {
      vec2 b = vec2(i, j);
      vec2 r = vec2(b) - f + rhash(p + b);
      res += 1. / pow(dot(r, r), 8.);
    }
  }
  return pow(1. / res, 0.0625);
}

void vertex() {
	uv = UV;
	position = VERTEX;
	instanceColor = INSTANCE_CUSTOM.rgb;
	worldPosition = (MODEL_MATRIX * vec4(position, 1.0)).xyz;
	noiseValue = voronoi(worldPosition.xy);
	VERTEX *= min(noiseValue, 0.5);
	if (animated) {
		float dist = length(CAMERA_POSITION_WORLD - worldPosition);
		vec3 directionFromCamera = normalize(CAMERA_POSITION_WORLD - worldPosition);
		float waveIntensity = 0.5;
	    float waveFrequency = 2.0;
	    float speedFactor = 1.0;
		float heightFactor = position.y;
		float waveXOffset = sin(noiseValue + TIME * speedFactor + position.x * waveFrequency) * waveIntensity * heightFactor;
	    float waveZOffset = cos(noiseValue + TIME * speedFactor + position.z * waveFrequency) * waveIntensity * heightFactor;
		vec3 animatedPosition = position;
	    animatedPosition.x += waveXOffset;
	    animatedPosition.z += waveZOffset;
		if (dist < 1.5) {
			animatedPosition.xz -= 2.0 * animatedPosition.y * directionFromCamera.zx * (1.5 - dist);
		}
		VERTEX = animatedPosition;
		worldPosition = (MODEL_MATRIX * vec4(animatedPosition, 1.0)).xyz;
	}
}

void fragment() {
	vec4 col = texture(color, uv);
	if (col.a < 0.5) {
		discard;
	}
	col.x *= noiseValue;
	ALBEDO = instanceColor * col.rgb;
	if (glow) {
		ALBEDO *= 10.0;
	}
}

void light() {
    DIFFUSE_LIGHT += (clamp(dot(NORMAL, LIGHT), 0.2, 1.0) * ATTENUATION * LIGHT_COLOR) * clamp(position.y, 0.2, 1.0);
}
