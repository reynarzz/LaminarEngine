#include "../../EditorNativesDefines.h"
#include "FileDialog.h"
#include "../../Types.h"

namespace cfiledialog
{
	static const char* AllocStringCopy(const std::string& str)
	{
		char* out = new char[str.size() + 1];
		memcpy(out, str.c_str(), str.size() + 1);
		return out;
	}

	static const char** AllocStringArrayCopy(const std::vector<std::string>& strs)
	{
		u64 count = static_cast<u64>(strs.size());
		const char** out = new const char* [count];

		for (u64 i = 0; i < count; ++i)
		{
			out[i] = AllocStringCopy(strs[i]);
		}

		return out;
	}

	static std::vector<std::pair<std::string, std::string>> MakeFilterVector(const char** filterNames, const char** filterPatterns, u64 filterCount)
	{
		std::vector<std::pair<std::string, std::string>> filters;
		filters.reserve(filterCount);

		for (u64 i = 0; i < filterCount; ++i)
		{
			filters.emplace_back(filterNames[i], filterPatterns[i]);
		}

		return filters;
	}

	EDITOR_NATIVES_API const char* OpenFile(const char* openPath, const char** filterNames, const char** filterPatterns, u64 filterCount)
	{
		auto filters = MakeFilterVector(filterNames, filterPatterns, filterCount);
		return AllocStringCopy(FileDialog::OpenFile(openPath, filters));
	}

	EDITOR_NATIVES_API const char** OpenFiles(const char* openPath, u64* outCount)
	{
		std::vector<std::string> results = FileDialog::OpenFiles(openPath);
		*outCount = static_cast<u64>(results.size());
		return AllocStringArrayCopy(results);
	}

	EDITOR_NATIVES_API const char** OpenFilesWithFilters(const char* openPath, const char** filterNames, const char** filterPatterns, u64 filterCount, u64* outCount)
	{
		auto filters = MakeFilterVector(filterNames, filterPatterns, filterCount);
		std::vector<std::string> results = FileDialog::OpenFiles(openPath, filters);
		*outCount = static_cast<u64>(results.size());
		return AllocStringArrayCopy(results);
	}

	EDITOR_NATIVES_API const char* OpenFolder(const char* openPath)
	{
		return AllocStringCopy(FileDialog::OpenFolder(openPath));
	}

	EDITOR_NATIVES_API const char** OpenFolders(const char* openPath, u64* countOut)
	{
		const std::vector<std::string> foldersPath = FileDialog::OpenFolders(openPath);
		*countOut = static_cast<u64>(foldersPath.size());
		return AllocStringArrayCopy(foldersPath);
	}

	EDITOR_NATIVES_API void DisplayFolder(const char* path, bool highlight)
	{
		FileDialog::DisplayFolder(path, highlight);
	}

	EDITOR_NATIVES_API const char* SaveFile(const char* openPath, const char* defaultName)
	{
		if (defaultName == nullptr) 
		{
			defaultName = "";
		}
		return AllocStringCopy(FileDialog::SaveFile(openPath, defaultName));
	}

	EDITOR_NATIVES_API const char* SaveFileWithFilters(const char* openPath, const char* defaultName, const char** filterNames,
		const char** filterPatterns, u64 filterCount)
	{
		auto filters = MakeFilterVector(filterNames, filterPatterns, filterCount);
		return AllocStringCopy(FileDialog::SaveFile(openPath, defaultName, filters));
	}

	EDITOR_NATIVES_API void FreeAllocatedString(const char* str)
	{
		delete[] str;
	}

	EDITOR_NATIVES_API void FreeAllocatedStringArray(const char** strings, u64 count)
	{
		for (u64 i = 0; i < count; ++i)
		{
			FreeAllocatedString(strings[i]);
		}

		delete[] strings;
	}
}