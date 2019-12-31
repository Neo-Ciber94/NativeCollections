#pragma once

#include <stdlib.h> 

enum class Flags : unsigned int {
	UNNITIALIZATED = 0,
	ZERO_MEMORY = 1
};

extern "C" 
{
	__declspec(dllexport) void* Malloc(size_t size, Flags flags);

	__declspec(dllexport) void* Realloc(void* block, size_t size, Flags flags);

	__declspec(dllexport) void Free(void* block);
}