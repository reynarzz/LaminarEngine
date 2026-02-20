// Copyright (c) 2025 Rafael Valoto. All Rights Reserved.
#pragma once
#include "GCore/Templates/TGenericHardwareInfo.h"
#include "GCore/Types/Structs/Context/DeviceContext.h"
#include "miniaudio.h"
#include "windows_device_info.h"
#include <cwchar>
#include <initguid.h>
#include <mmdeviceapi.h>
#include <mutex>
#include "GCore/Utils/SoDefines.h"
#include <propsys.h>
#include <set>
#include <string>

namespace windows_platform
{
	struct windows_hardware_policy;
	using windows_hardware = GamepadCore::TGenericHardwareInfo<windows_hardware_policy>;

	struct audio_device_registry
	{
		static audio_device_registry& Get()
		{
			static audio_device_registry Instance;
			return Instance;
		}

		void RegisterDevice(const ma_device_id& DeviceId)
		{
			gc_lock::lock_guard<gc_lock::mutex> Lock(Mutex);
			UsedDevices.insert(DeviceId);
		}

		void UnregisterDevice(const ma_device_id& DeviceId)
		{
			gc_lock::lock_guard<gc_lock::mutex> Lock(Mutex);
			UsedDevices.erase(DeviceId);
		}

		bool IsDeviceInUse(const ma_device_id& DeviceId)
		{
			gc_lock::lock_guard<gc_lock::mutex> Lock(Mutex);
			return UsedDevices.find(DeviceId) != UsedDevices.end();
		}

	private:
		struct DeviceIdCompare
		{
			bool operator()(const ma_device_id& lhs, const ma_device_id& rhs) const
			{
				return std::memcmp(&lhs, &rhs, sizeof(ma_device_id)) < 0;
			}
		};

		gc_lock::mutex Mutex;
		std::set<ma_device_id, DeviceIdCompare> UsedDevices;
	};

	struct windows_hardware_policy
	{
		windows_hardware_policy() = default;

		void Read(FDeviceContext* Context)
		{
			windows_device_info::read(Context);
		}

		void Write(FDeviceContext* Context)
		{
			windows_device_info::write(Context);
		}

		void Detect(std::vector<FDeviceContext>& Devices)
		{
			windows_device_info::detect(Devices);
		}

		bool CreateHandle(FDeviceContext* Context)
		{
			return windows_device_info::create_handle(Context);
		}

		void InvalidateHandle(FDeviceContext* Context)
		{
			if (Context && Context->AudioContext && Context->AudioContext->bInitialized)
			{
				audio_device_registry::Get().UnregisterDevice(Context->AudioContext->DeviceId);
			}
			windows_device_info::invalidate_handle(Context);
		}

		void ProcessAudioHaptic(FDeviceContext* Context)
		{
			windows_device_info::process_audio_haptic(Context);
		}

		void InitializeAudioDevice(FDeviceContext* Context)
		{
			if (!Context)
			{
				return;
			}

			ma_context maContext;
			if (ma_context_init(nullptr, 0, nullptr, &maContext) != MA_SUCCESS)
			{
				return;
			}

			ma_device_info* pPlaybackInfos;
			ma_uint32 playbackCount;
			ma_device_info* pCaptureInfos;
			ma_uint32 captureCount;

			if (ma_context_get_devices(&maContext, &pPlaybackInfos, &playbackCount, &pCaptureInfos, &captureCount) != MA_SUCCESS)
			{
				ma_context_uninit(&maContext);
				return;
			}

			std::string TargetContainerId = windows_device_info::get_container_id(Context->Path);

			ma_device_id* pFoundDeviceId = nullptr;
			ma_device_id foundDeviceId;

			for (ma_uint32 i = 0; i < playbackCount; i++)
			{
				std::string AudioContainerId = windows_device_info::get_audio_container_id(pPlaybackInfos[i].id.wasapi);

				if (!AudioContainerId.empty() && AudioContainerId == TargetContainerId)
				{
					if (!audio_device_registry::Get().IsDeviceInUse(pPlaybackInfos[i].id))
					{
						foundDeviceId = pPlaybackInfos[i].id;
						pFoundDeviceId = &foundDeviceId;
						break;
					}
				}
			}

			if (!pFoundDeviceId)
			{
				for (ma_uint32 i = 0; i < playbackCount; i++)
				{
					std::string deviceName(pPlaybackInfos[i].name);
					if (deviceName.find("DualSense") != std::string::npos ||
					    deviceName.find("Wireless Controller") != std::string::npos)
					{
						if (!audio_device_registry::Get().IsDeviceInUse(pPlaybackInfos[i].id))
						{
							foundDeviceId = pPlaybackInfos[i].id;
							pFoundDeviceId = &foundDeviceId;
							break;
						}
					}
				}
			}

			Context->AudioContext = std::make_shared<FAudioDeviceContext>();

			if (pFoundDeviceId)
			{
				audio_device_registry::Get().RegisterDevice(*pFoundDeviceId);
				Context->AudioContext->InitializeWithDeviceId(pFoundDeviceId, 48000, 4);
			}

			ma_context_uninit(&maContext);
		}
	};
} // namespace windows_platform
