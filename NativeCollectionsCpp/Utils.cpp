#include "pch.h"
#include "Utils.h"

void* Malloc(size_t size, Flags flags)
{
	if (flags == Flags::ZERO_MEMORY) {
		return calloc(size, 1);
	}

	return malloc(size);
}

void* Realloc(void* block, size_t size, Flags flags)
{
	if (flags == Flags::ZERO_MEMORY) {
		return _recalloc(block, size, 1);
	}

	return realloc(block, size);
}

void Free(void* block) {
	free(block);
}