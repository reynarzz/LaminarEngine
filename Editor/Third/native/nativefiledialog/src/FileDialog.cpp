#include "FileDialog.h"
#define NFD_MACOS_ALLOWEDCONTENTTYPES 1
#include <nfd.h>
#include <cstdlib>   // for system()

static bool _isInitialized =false;

static void init() 
{
	if (!_isInitialized) { _isInitialized = true; NFD_Init(); }
}
static void openFIle()
{
	NFD_Init();

	nfdu8char_t* outPath;
    nfdu8filteritem_t filters[2] = { { "Source code", "c,cpp,cc" }, { "Headers", "h,hpp" } };
	nfdopendialogu8args_t args = { 0 };
	args.filterList = filters;
	args.filterCount = 2;
    
	nfdresult_t result = NFD_OpenDialogU8_With(&outPath, &args);

	//NFD_SaveDialogU8_With();

	if (result == NFD_OKAY)
	{
		puts("Success!");
		puts(outPath);
		NFD_FreePathU8(outPath);
	}
	else if (result == NFD_CANCEL)
	{
		puts("User pressed cancel.");
	}
	else
	{
		printf("Error: %s\n", NFD_GetError());
	}

	NFD_Quit();
}

static std::vector<nfdu8filteritem_t> to_internal_filter(const std::vector<std::pair<std::string, std::string>>& filter)
{
	std::vector<nfdu8filteritem_t> filters{};
	for (const auto& pair : filter)
	{
		filters.push_back({ pair.first.c_str(), pair.second.c_str() });
	}

	return filters;
}

std::string FileDialog::OpenFile(const std::string& openPath, const std::vector<std::pair<std::string, std::string>>& filters)
{
	init();

	auto internalFilters = to_internal_filter(filters);

	nfdu8char_t* outPath;
	nfdresult_t result = NFD_OpenDialog(&outPath, internalFilters.data(), internalFilters.size(), openPath.data());
	std::string outPathStr{};

	if (result == NFD_OKAY)
	{
		outPathStr = outPath;
		NFD_FreePathU8(outPath);
	}
	else if (result == NFD_CANCEL)
	{
	}
	else
	{
	}
	NFD_Quit();

	return outPathStr;
}

std::vector<std::string> FileDialog::OpenFiles(const std::string& openPath, const std::vector<std::pair<std::string, std::string>>& filters)
{
	init();

	auto internalFilters = to_internal_filter(filters);

	const nfdpathset_t* outPaths;
	nfdresult_t result = NFD_OpenDialogMultiple(&outPaths, internalFilters.data(), internalFilters.size(), openPath.data());
	std::vector<std::string> outPathStr{};

	if (result == NFD_OKAY)
	{
		nfdpathsetsize_t numPaths;
		NFD_PathSet_GetCount(outPaths, &numPaths);

		outPathStr.resize(numPaths);
		for (size_t i = 0; i < numPaths; i++)
		{
			nfdchar_t* path;
			NFD_PathSet_GetPath(outPaths, i, &path);
			outPathStr[i] = path;
			NFD_PathSet_FreePath(path);
		}

		NFD_PathSet_Free(outPaths);
	}
	else if (result == NFD_CANCEL)
	{
	}
	else
	{
	}
	NFD_Quit();

	return outPathStr;
}

std::string FileDialog::OpenFolder(const std::string& openPath)
{
	init();

	std::string outPathStr{};

	nfdchar_t* outPath;

	// show the dialog
	nfdresult_t result = NFD_PickFolder(&outPath, openPath.data());
	if (result == NFD_OKAY) {
		
		outPathStr = outPath;
		NFD_FreePath(outPath);
	}
	else if (result == NFD_CANCEL)
	{
	}
	else 
	{
	}

	NFD_Quit();

	return outPathStr;
}

std::vector<std::string> FileDialog::OpenFolders(const std::string& openPath)
{
	init();

	const nfdpathset_t* outPaths;

	// show the dialog
	nfdresult_t result = NFD_PickFolderMultiple(&outPaths, openPath.data());
	std::vector<std::string> outPathStr{};

	if (result == NFD_OKAY)
	{
		nfdpathsetsize_t numPaths;
		NFD_PathSet_GetCount(outPaths, &numPaths);
		outPathStr.resize(numPaths);

		for (nfdpathsetsize_t i = 0; i < numPaths; ++i)
		{
			nfdchar_t* path;
			NFD_PathSet_GetPath(outPaths, i, &path);

			outPathStr[i] = path;

			// remember to free the pathset path with NFD_PathSet_FreePath (not NFD_FreePath!)
			NFD_PathSet_FreePath(path);
		}

		// remember to free the pathset memory (since NFD_OKAY is returned)
		NFD_PathSet_Free(outPaths);
	}
	else if (result == NFD_CANCEL) 
	{
		//puts("User pressed cancel.");
	}
	else 
	{
		//printf("Error: %s\n", NFD_GetError());
	}

	// Quit NFD
	NFD_Quit();


	return outPathStr;
}

std::string FileDialog::SaveFile(const std::string& openPath, const std::string& defaultName, const std::vector<std::pair<std::string, std::string>>& filters)
{
	init();
	auto internalFilters = to_internal_filter(filters);

	nfdchar_t* savePath;

	// show the dialog
	nfdresult_t result = NFD_SaveDialog(&savePath, internalFilters.data(), internalFilters.size(), openPath.data(), defaultName.data());
	std::string outPathStr{};

	if (result == NFD_OKAY)
	{
		outPathStr = savePath;
		NFD_FreePath(savePath);
	}
	else if (result == NFD_CANCEL) 
	{
	}
	else 
	{
	}

	// Quit NFD
	NFD_Quit();

	return outPathStr;
}


void FileDialog::DisplayFolder(const std::string& path, bool highlight)
{
#if defined(_WIN32)

	std::string fixedPath = path;
	for (auto& ch : fixedPath)
	{
		if (ch == '/')
			ch = '\\';
	}
	std::string command = (highlight ? "explorer /select,\"": "explorer \"") + fixedPath + "\"";

#elif defined(__APPLE__)
    std::string command = (highlight ? "open -R \"": "open \"") + path + "\""; // -R reveals in Finder
#else
    std::string command = (highlight ? "xdg-open \"":"xdg-open \"") + filePath + "\""; // fallback, won't highlight
#endif
	std::system(command.c_str());
}