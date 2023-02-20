#ifndef MY_PROCEDURAL_TILING_FUNCS
#define MY_PROCEDURAL_TILING_FUNCS
 
void GetChunkCenter_float(float2 pos, float cellSize, out float2 outCenterChunk)
{
	 outCenterChunk = round(pos / cellSize) * cellSize;
}
void GetChunkCenter_half(half2 pos, half cellSize, out half2 outCenterChunk)
{
	 outCenterChunk = round(pos / cellSize) * cellSize;
}

#endif

