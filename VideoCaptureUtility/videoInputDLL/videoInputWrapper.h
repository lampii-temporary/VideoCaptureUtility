#pragma once
#include <vector>
#include <string>
#include <videoInput.h>
#include <stdio.h>
#pragma comment(lib, "ole32.lib")
#include <objbase.h>
extern "C"
{
#define _VI_API __declspec(dllexport) 

	namespace VIApi
	{
		static unsigned char* frameBuffer[1920 * 1080 * 3];
		static videoInput VI;

		static bool isInitialized = false;
		
		_VI_API  int ListDevices()
		{
			int a = VI.listDevices();
			std::vector<std::string> list = VI.getDeviceList();
			
			for (std::vector<std::string>::iterator it = list.begin(); it != list.end(); ++it)
			{

			}
			return VI.listDevices();;
		}
		_VI_API char* GetDeviceName(int devId)
		{
			char *name = VI.getDeviceName(devId);
			ULONG ulSize = strlen(name) + sizeof(char);
			char* result = NULL;

			result = (char*)::CoTaskMemAlloc(ulSize);
			std::strcpy(result, name);
				return result;
		}
		_VI_API int GetWidth(int devId)
		{
			return VI.getWidth(devId);
		}
		_VI_API int GetHeight(int devId)
		{
			return VI.getHeight(devId);
		}
		_VI_API int GetBufferSize(int devId)
		{
			return VI.getSize(devId);
		}
		_VI_API bool SetFormat(int devId, int format)
		{
			return VI.setFormat( devId,  format);
		}

		_VI_API bool IsFrameReady(int devId)
		{
			return VI.isFrameNew(devId);
		}
		_VI_API void SetupDevice(int devId)
		{
			VI.setupDevice(devId);
		}

		_VI_API  void SetupDevice1(int devId, int connectionType)
		{
			VI.setupDevice(devId, connectionType);
		}

		_VI_API  void SetupDevice2(int devId, int width, int height)
		{
			VI.setupDevice(devId, width, height);
		}
		_VI_API  void SetupDevice3(int devId, int width, int height, int connectionType)
		{
			VI.setupDevice(devId, width, height, connectionType);

		}

		_VI_API void ShowSettingsWindow(int devId)
		{
			VI.showSettingsWindow(devId);
		}
		_VI_API void StopDevice(int devId)
		{
			VI.stopDevice(devId);
		}
		_VI_API void SetRequestedMediaSubType(int mediaType)
		{
			VI.setRequestedMediaSubType(mediaType);
		}

		_VI_API void SetIdealFramerate(int devId, int fps)
		{
			VI.setIdealFramerate(devId, fps);
		}

		_VI_API void SetUseCallback(bool useCallback)
		{
			VI.setUseCallback(useCallback);
		}
		_VI_API void GetImage(int devId,  unsigned char ** outputBuffer, bool color, bool flip)
		{
			//char *name = VI.getDeviceName(devId);
			//ULONG ulSize = strlen(name) + sizeof(char);
			//char* result = NULL;

			//result = (char*)::CoTaskMemAlloc(ulSize);
			//std::strcpy(result, name);
			 *outputBuffer = VI.getPixels(devId, color, flip);;

		//	 *data = VI.getPixels(devId, color, flip);
		/*	ULONG ulSize = VI.getSize(devId) * sizeof(unsigned char);

			char *result = NULL;
			result = ( char*)::CoTaskMemAlloc(ulSize);
			memcpy(result, data, ulSize);
			memcpy(*outputBuffer, data, ulSize);*/

		//	
	//		return result;
		}

	}
}