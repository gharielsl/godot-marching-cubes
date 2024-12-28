shader_type spatial;

#define WORLD_HEIGHT 128
#define BORDER_SIZE 3
#define CHUNK_SIZE 10
#define CHUNK_SIZE_WITH_BORDER 16

varying vec3 position;

void vertex() {
	position = VERTEX;
}

void fragment() {
	if (position.x < float(BORDER_SIZE) || position.x > float(CHUNK_SIZE + BORDER_SIZE)) {
		discard;
	}
	if (position.z < float(BORDER_SIZE) || position.z > float(CHUNK_SIZE + BORDER_SIZE)) {
		discard;
	}
}
