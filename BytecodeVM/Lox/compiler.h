#ifndef lox_compiler_h
#define lox_compiler_h

#include "vm.h"

bool compile(const char* source, Chunk* chunk);

#endif	// lox_compiler_h