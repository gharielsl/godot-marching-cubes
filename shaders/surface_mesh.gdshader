shader_type spatial;

render_mode cull_disabled;

uniform sampler2D color: repeat_disable;
uniform bool animated;
uniform bool glow;

varying vec2 uv;
varying vec3 position;
varying vec3 worldPosition;
varying vec3 instanceColor;

float hash(vec2 p) {
    return fract(sin(dot(p.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void vertex() {
	uv = UV;
	position = VERTEX;
	instanceColor = INSTANCE_CUSTOM.rgb;
	if (animated) {
		float dist = length(CAMERA_POSITION_WORLD - worldPosition);
		float waveIntensity = 0.05;
	    float waveFrequency = 2.0;
	    float speedFactor = hash(position.xy) * 5.0;
		float heightFactor = position.y;
		float waveXOffset = sin(TIME * speedFactor + position.x * waveFrequency) * waveIntensity * heightFactor;
	    float waveZOffset = cos(TIME * speedFactor + position.z * waveFrequency) * waveIntensity * heightFactor;
		vec3 animatedPosition = position;
	    animatedPosition.x += waveXOffset;
	    animatedPosition.z += waveZOffset;
		worldPosition = (MODEL_MATRIX * vec4(animatedPosition, 1.0)).xyz;
		if (length(CAMERA_POSITION_WORLD - worldPosition) < 16.0) {
			VERTEX = animatedPosition;
		} else {
			worldPosition = (MODEL_MATRIX * vec4(position, 1.0)).xyz;
		}
	} else {
		worldPosition = (MODEL_MATRIX * vec4(position, 1.0)).xyz;
	}
}

void fragment() {
	vec4 col = texture(color, uv);
	if (col.a < 0.5) {
		discard;
	}
	ALBEDO = instanceColor * col.rgb;
	if (glow) {
		ALBEDO *= 10.0;
	}
}

void light() {
    DIFFUSE_LIGHT += (clamp(dot(NORMAL, LIGHT), 0.2, 1.0) * ATTENUATION * LIGHT_COLOR) * clamp(position.y, 0.2, 1.0);
}
