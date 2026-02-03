#pragma once
#include <string>
#include <vector>

class FileDialog 
{
public:
	static std::string OpenFile(const std::string& openPath, const std::vector<std::pair<std::string, std::string>>& filters = {});
	static std::vector<std::string> OpenFiles(const std::string& openPath, const std::vector<std::pair<std::string, std::string>>& filters = {});


	static std::string OpenFolder(const std::string& openPath = {});
	static std::vector<std::string> OpenFolders(const std::string& openPath = {});

	static void DisplayFolder(const std::string& path, bool highlight = false);

	static std::string SaveFile(const std::string& openPath, const std::string& defaultName, 
                                 const std::vector<std::pair<std::string, std::string>>& filters = {});

};