#ifndef lox_debug_h
#define lox_debug_h

#include "chunk.h"

void disassembleChunk(Chunk* chunk, const char* name);
int dissassembleInstruction(Chunk* chunk, int offset);

#endif	// lox_debug_h