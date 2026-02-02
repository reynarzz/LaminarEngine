#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image_write.h"

#include <EditorNativesDefines.h>
#include <Types.h>

enum EditorImageWriteFormat
{
    Png,
    Jpg,
    Tga,
	Bmp,
	hdr
};

EDITOR_NATIVES_API void STBI_Write(const char* path, s32 width, s32 height, s32 channels, void* data, EditorImageWriteFormat format, s32 bitsPerChannel, u8 filpVertical)
{
	stbi_flip_vertically_on_write(filpVertical);
	s32 stride = width * channels * (bitsPerChannel / 8);
	s32 quality = 90;
	switch (format)
	{
	case Png:
		stbi_write_png(path, width, height, channels, data, stride);
		break;
	case Jpg:
		stbi_write_jpg(path, width, height, channels, data, 90);
		break;
	case Tga:
		stbi_write_tga(path, width, height, channels, data);
		break;
	case Bmp:
		stbi_write_bmp(path, width, height, channels, data);
		break;
	case hdr:
		stbi_write_bmp(path, width, height, channels, data);
		break;
	default:
		break;
	}
}
