#include "EditorNativesDefines.h"
#include "avir.h"
#include <Types.h>

EDITOR_NATIVES_API void Resize(unsigned char* inImageData, s32 channels, s32 inWidth, s32 inHeight, unsigned char* outImageData, s32 outWidth, s32 outHeight)
{
	avir::CImageResizer<> ImageResizer(8);
	ImageResizer.resizeImage(inImageData, inWidth, inHeight, 0, outImageData, outWidth, outHeight, channels, 0);
}