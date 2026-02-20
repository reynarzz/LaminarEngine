// Copyright (c) 2025 Rafael Valoto. All Rights Reserved.
#pragma once
#ifdef BUILD_GAMEPAD_CORE_TESTS

#include "GCore/Types/Structs/Context/DeviceContext.h"
#include <memory>
#include <string>
#include <vector>

enum class EPollResult
{
	ReadOk,
	NoIoThisTick,
	TransientError,
	Disconnected
};

class linux_device_info
{
public:
	virtual ~linux_device_info() = default;
	static void process_audio_haptic(FDeviceContext* Context);
	static bool configure_features(FDeviceContext* Context);
	static void read(FDeviceContext* Context);
	static void write(FDeviceContext* Context);
	static void detect(std::vector<FDeviceContext>& Devices);
	static bool create_handle(FDeviceContext* Context);
	static void invalidate_handle(FDeviceContext* Context);
	static std::string get_container_id(const std::string& DevicePath);
	static std::string get_audio_container_id(const std::string& AudioDeviceId);
	static EPollResult poll_tick(FPlatformDeviceHandle Handle, unsigned char* Buffer, std::int32_t Length, std::int32_t& OutBytesRead);
	static bool should_treat_as_disconnected(const std::int32_t Error)
	{
		// No Linux via SDL_hidapi, erros negativos geralmente indicam desconexão ou erro fatal
		return Error < 0;
	}
};
#endif
