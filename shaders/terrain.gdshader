shader_type spatial;

//#define DEBUG

#define WORLD_HEIGHT 128
#define BORDER_SIZE 3
#define CHUNK_SIZE 10
#define CHUNK_SIZE_WITH_BORDER 16

varying vec3 position;
varying vec3 normal;

uniform sampler2D grass_c;
uniform sampler2D grass_n;
uniform sampler2D grass_h;
uniform sampler2D dirt_c;
uniform sampler2D dirt_n;
uniform sampler2D dirt_h;
uniform sampler2D stone_c;
uniform sampler2D stone_n;
uniform sampler2D stone_h;
uniform sampler2D sand_c;
uniform sampler2D sand_n;
uniform sampler2D sand_h;
uniform sampler2D moss_stone_c;
uniform sampler2D moss_stone_n;
uniform sampler2D moss_stone_h;
uniform sampler2D obsidian_c;
uniform sampler2D obsidian_n;
uniform sampler2D obsidian_h;
uniform sampler2D chunkData;

vec4 sampleTex(sampler2D sampler, sampler2D samplerWall, float scale, vec3 vertex, vec3 faceNormal) {
    vec3 pWeight = abs(faceNormal);
    pWeight /= pWeight.x + pWeight.y + pWeight.z;
    vec2 yz = vertex.yz / scale;
    vec2 xz = vertex.xz / scale;
    vec2 xy = vertex.xy / scale;
    vec4 xP = texture(samplerWall, yz) * pWeight.x;
    vec4 yP = texture(sampler, xz) * pWeight.y;
    vec4 zP = texture(samplerWall, xy) * pWeight.z;
    return xP + yP + zP;
}

int getVoxelId(ivec3 voxelPosition) {
	float f = texelFetch(chunkData, ivec2(voxelPosition.z * CHUNK_SIZE_WITH_BORDER + voxelPosition.x, voxelPosition.y), 0).r;
	return int(f * 255.0);
}

float getVoxelHeight(int voxelId, vec3 vertex, vec3 faceNormal) {
	if (voxelId == 1) {
		return sampleTex(dirt_h, dirt_h, 1.0, vertex, faceNormal).x;
	}
	if (voxelId == 2) {
		return sampleTex(grass_h, dirt_h, 1.0, vertex, faceNormal).x;
	}
	return 0.0;
}

float getHeight(vec3 vertex, vec3 faceNormal) {
	if (vertex.x < float(BORDER_SIZE) || vertex.x > float(CHUNK_SIZE + BORDER_SIZE)) {
		return 0.0;
	}
	if (vertex.z < float(BORDER_SIZE) || vertex.z > float(CHUNK_SIZE + BORDER_SIZE)) {
		return 0.0;
	}
	float accumulatedHeight = 0.0;
	float totalHeightWeight = 0.0;
	for (int x = -1; x <= 2; x++) {
		for (int y = -1; y <= 2; y++) {
			for (int z = -1; z <= 2; z++) {
				ivec3 currentPosition = ivec3(vertex) + ivec3(x, y, z);
				int voxelId = getVoxelId(currentPosition);
				if (voxelId == 0 || voxelId == 5) continue; // air or water
				accumulatedHeight += getVoxelHeight(voxelId, vertex, faceNormal);
				totalHeightWeight++;
			}
		}
	}
	return (accumulatedHeight / totalHeightWeight) / 3.0;
}

void vertex() {
	// VERTEX += getHeight(VERTEX, NORMAL) * NORMAL; // doesnt work well
	position = VERTEX;
	normal = NORMAL;
}

vec3 getVoxelColor(int voxelId, out vec3 normalTex, out bool hasNormal) {
	if (voxelId == 1) {
		hasNormal = true;
		normalTex = sampleTex(dirt_n, dirt_n, 1.0, position, normal).xyz;
		return sampleTex(dirt_c, dirt_c, 1.0, position, normal).xyz;
	}
	if (voxelId == 2) {
		hasNormal = true;
		normalTex = sampleTex(grass_n, dirt_n, 1.0, position, normal).xyz;
		return sampleTex(grass_c, dirt_c, 1.0, position, normal).xyz;
	}
	if (voxelId == 3) {
		hasNormal = true;
		normalTex = sampleTex(stone_n, stone_n, 1.0, position, normal).xyz;
		return sampleTex(stone_c, stone_c, 1.0, position, normal).xyz;
	}
	if (voxelId == 4) {
		hasNormal = true;
		normalTex = sampleTex(sand_n, sand_n, 1.0, position, normal).xyz;
		return sampleTex(sand_c, sand_c, 1.0, position, normal).xyz;
	}
	if (voxelId == 6) {
		hasNormal = true;
		normalTex = sampleTex(moss_stone_n, moss_stone_n, 1.0, position, normal).xyz;
		return sampleTex(moss_stone_c, moss_stone_c, 1.0, position, normal).xyz;
	}
	if (voxelId == 7) {
		hasNormal = true;
		normalTex = sampleTex(obsidian_n, obsidian_n, 1.0, position, normal).xyz;
		return sampleTex(obsidian_c, obsidian_c, 1.0, position, normal).xyz;
	}
	normalTex = normal;
	hasNormal = false;
	return vec3(0, 0, 0);
}

void fragment() {
	bool isBorder = false; // debug
	if (position.x < float(BORDER_SIZE) || position.x > float(CHUNK_SIZE + BORDER_SIZE)) {
#ifdef DEBUG
		isBorder = true;
#else
		discard;
#endif
	}
	if (position.z < float(BORDER_SIZE) || position.z > float(CHUNK_SIZE + BORDER_SIZE)) {
#ifdef DEBUG
		isBorder = true;
#else
		discard;
#endif
	}
	vec3 localPosition = position - vec3(ivec3(position));
	vec3 accumulatedColor = vec3(0.0);
	float totalColorWeight = 0.0;
	vec3 accumulatedNormal = vec3(0.0);
	float totalNormalWeight = 0.0;
	for (int x = -1; x <= 2; x++) {
		for (int y = -1; y <= 2; y++) {
			for (int z = -1; z <= 2; z++) {
				ivec3 currentPosition = ivec3(position) + ivec3(x, y, z);
				int voxelId = getVoxelId(currentPosition);
				if (voxelId == 0 || voxelId == 5) continue; // air or water
				bool hasNormal;
				vec3 normalTex;
				vec3 color = getVoxelColor(voxelId, normalTex, hasNormal);
				float dist = length(localPosition - vec3(float(x), float(y), float(z)));
				float weight = smoothstep(1.5, 0.0, dist);
				if (weight <= 0.0) continue;
				accumulatedColor += color * weight;
				totalColorWeight += weight;
				if (!hasNormal) continue;
				accumulatedNormal += normalTex * weight;
				totalNormalWeight += weight;
			}
		}
	}
	if (totalColorWeight > 0.0) {
		accumulatedColor /= totalColorWeight;
	}
	if (totalNormalWeight > 0.0) {
		accumulatedNormal /= totalNormalWeight;
	}
	NORMAL_MAP = accumulatedNormal;
	NORMAL_MAP_DEPTH = 4.0;
	ALBEDO = isBorder ? vec3(1, 0, 0) : accumulatedColor;
	//ROUGHNESS = 0.7;
}
