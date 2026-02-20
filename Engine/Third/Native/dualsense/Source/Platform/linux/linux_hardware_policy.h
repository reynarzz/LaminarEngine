// Copyright (c) 2025 Rafael Valoto. All Rights Reserved.
#pragma once
#ifdef BUILD_GAMEPAD_CORE_TESTS
#include "GCore/Templates/TGenericHardwareInfo.h"
#include "GCore/Types/Structs/Context/DeviceContext.h"
#if GAMEPAD_CORE_HAS_AUDIO
#include "miniaudio.h"
#endif
#include "linux_device_info.h"
#include <string>
#include <vector>

namespace linux_platform
{
	struct linux_hardware_policy;
	using linux_hardware = GamepadCore::TGenericHardwareInfo<linux_hardware_policy>;

	struct linux_hardware_policy
	{
		linux_hardware_policy() = default;

		static void Read(FDeviceContext* Context)
		{
			linux_device_info::read(Context);
		}

		static void Write(FDeviceContext* Context)
		{
			linux_device_info::write(Context);
		}

		static void Detect(std::vector<FDeviceContext>& Devices)
		{
			linux_device_info::detect(Devices);
		}

		static bool CreateHandle(FDeviceContext* Context)
		{
			return linux_device_info::create_handle(Context);
		}

		static void InvalidateHandle(FDeviceContext* Context)
		{
			linux_device_info::invalidate_handle(Context);
		}

		static void ProcessAudioHaptic(FDeviceContext* Context)
		{
			linux_device_info::process_audio_haptic(Context);
		}

		static void InitializeAudioDevice(FDeviceContext* Context)
		{
#if GAMEPAD_CORE_HAS_AUDIO
			if (!Context)
			{
				return;
			}

			// Initialize miniaudio context for device enumeration
			ma_context maContext;
			if (ma_context_init(nullptr, 0, nullptr, &maContext) != MA_SUCCESS)
			{
				return;
			}

			// Get playback devices
			ma_device_info* pPlaybackInfos;
			ma_uint32 playbackCount;
			ma_device_info* pCaptureInfos;
			ma_uint32 captureCount;

			if (ma_context_get_devices(&maContext, &pPlaybackInfos, &playbackCount, &pCaptureInfos, &captureCount) != MA_SUCCESS)
			{
				ma_context_uninit(&maContext);
				return;
			}

			// Search for DualSense audio device
			ma_device_id* pFoundDeviceId = nullptr;
			ma_device_id foundDeviceId;

			for (ma_uint32 i = 0; i < playbackCount; i++)
			{
				std::string deviceName(pPlaybackInfos[i].name);

				// Check if device name contains DualSense identifiers
				// On Linux, the audio device name might vary (e.g., "DualSense Wireless Controller Analog Stereo")
				if (deviceName.find("DualSense") != std::string::npos ||
				    deviceName.find("Wireless Controller") != std::string::npos ||
				    deviceName.find("Sony") != std::string::npos)
				{
					foundDeviceId = pPlaybackInfos[i].id;
					pFoundDeviceId = &foundDeviceId;
					std::cout << "[InitializeAudioDevice] Found potential audio device: " << deviceName << std::endl;
					break;
				}
			}

			// Initialize audio context with found device (or default if not found)
			Context->AudioContext = std::make_shared<FAudioDeviceContext>();

			if (pFoundDeviceId)
			{
				// DualSense haptics use 4 channels at 48000 Hz
				Context->AudioContext->InitializeWithDeviceId(pFoundDeviceId, 48000, 4);
			}

			ma_context_uninit(&maContext);
#endif
		}
	};
} // namespace linux_platform
#endif
