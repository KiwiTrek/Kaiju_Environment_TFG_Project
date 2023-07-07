void WorkflowColor_float4(float4 metallic, float4 specular, out float4 Out)
{
	#ifdef _SPECULAR_SETUP
	Out = specular;
	#else
	Out = metallic;
	#endif
}