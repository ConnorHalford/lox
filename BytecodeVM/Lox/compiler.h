#ifndef lox_compiler_h
#define lox_compiler_h

#include "object.h"
#include "vm.h"

ObjFunction* compile(const char* source);
void markCompilerRoots();

#endif	// lox_compiler_h