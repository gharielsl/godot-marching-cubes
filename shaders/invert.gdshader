shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 texture_color = texture(TEXTURE, UV);
	if (texture_color.a > 0.0) {
		COLOR = 1.0 - texture(SCREEN_TEXTURE, SCREEN_UV);
		COLOR.w = 1.0;
	} else {
		COLOR = vec4(0);
	}
}