// Copyright (c) 2025 Rafael Valoto. All rights reserved.
#pragma once

#ifdef _WIN32
#include "GCore/Types/DSCoreTypes.h"
#include "GCore/Types/Structs/Context/DeviceContext.h"
#include <Windows.h>
#include <string>
#include <vector>

enum class EPollResult
{
	ReadOk,
	NoIoThisTick,
	TransientError,
	Disconnected
};

class windows_device_info
{
public:
	virtual ~windows_device_info() = default;
	static void process_audio_haptic(FDeviceContext* Context);
	static void configure_features(FDeviceContext* Context);
	static void read(FDeviceContext* Context);
	static void write(FDeviceContext* Context);
	static void detect(std::vector<FDeviceContext>& Devices);
	static bool create_handle(FDeviceContext* Context);
	static void invalidate_handle(FDeviceContext* Context);
	static void invalidate_handle(HANDLE Handle);
	static bool ping_once(HANDLE Handle, std::int32_t* OutLastError = nullptr);
	static std::string get_container_id(const std::string& DevicePath);
	static std::string get_audio_container_id(const wchar_t* AudioDeviceId);
	static EPollResult poll_tick(HANDLE Handle, unsigned char* Buffer, std::int32_t Length, DWORD& OutBytesRead);
	static bool should_treat_as_disconnected(const std::int32_t Error)
	{
		switch (Error)
		{
			case ERROR_DEVICE_NOT_CONNECTED:
			case ERROR_GEN_FAILURE:
			case ERROR_INVALID_HANDLE:
			case ERROR_BAD_COMMAND:
			case ERROR_FILE_NOT_FOUND:
			case ERROR_ACCESS_DENIED: return true;
			default: return false;
		}
	}
};

#endif
