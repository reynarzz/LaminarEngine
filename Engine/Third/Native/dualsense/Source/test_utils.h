// Copyright (c) 2025 Rafael Valoto. All Rights Reserved.
#pragma once

#include "GCore/Interfaces/IPlatformHardwareInfo.h"
#include "GCore/Templates/TBasicDeviceRegistry.h"
#include <iostream>
#include <memory>
#include <vector>

#ifdef _WIN32
#include "Platform/windows/windows_hardware_policy.h"
using platform_hardware = windows_platform::windows_hardware;
#elif __linux__
#include "Platform/linux/linux_hardware_policy.h"
using platform_hardware = linux_platform::linux_hardware;
#endif

namespace test_utils
{
	/**
	 * @brief Registry policy for tests that just prints when a new gamepad is dispatched.
	 */
	struct test_registry_policy
	{
		using EngineIdType = uint32_t;
		struct Hasher
		{
			size_t operator()(const EngineIdType& id) const { return std::hash<EngineIdType>{}(id); }
		};

		static EngineIdType AllocEngineDevice()
		{
			static EngineIdType nextId = 0;
			return nextId++;
		}
		static void DisconnectDevice(EngineIdType id) {}
		static void DispatchNewGamepad(EngineIdType id)
		{
			std::cout << "[TestRegistry] Dispatched Gamepad ID: " << id << std::endl;
		}
	};

	class test_device_registry : public GamepadCore::TBasicDeviceRegistry<test_registry_policy>
	{
	public:
		using GamepadCore::TBasicDeviceRegistry<test_registry_policy>::TBasicDeviceRegistry;

		// Expose GetLibrary which is in the base class but we need it typed
		using GamepadCore::TBasicDeviceRegistry<test_registry_policy>::GetLibrary;
	};

	/**
	 * @brief Initializes the hardware and registry for tests.
	 * @param OutHardware Pointer to the hardware info interface.
	 * @param OutRegistry Pointer to the device registry.
	 */
	inline void initialize_test_environment(
	    std::unique_ptr<IPlatformHardwareInfo>& OutHardware,
	    std::unique_ptr<test_device_registry>& OutRegistry)
	{
		// TODO: implement macOs platform hardware.
#ifndef MACOS
		OutHardware = std::make_unique<platform_hardware>();
		IPlatformHardwareInfo::SetInstance(std::move(OutHardware));
		// Get it back for reference if needed, although it's now owned by the static instance
		// But for tests it's better to let the user have it if they asked for it.
		// Actually IPlatformHardwareInfo::SetInstance takes ownership.

		OutRegistry = std::make_unique<test_device_registry>();

		std::cout << "[test_utils] Environment initialized." << std::endl;
#endif
	}
	
} // namespace test_utils


