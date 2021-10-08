#ifndef lox_compiler_h
#define lox_compiler_h

#include "object.h"
#include "vm.h"

ObjFunction* compile(const char* source);

#endif	// lox_compiler_h