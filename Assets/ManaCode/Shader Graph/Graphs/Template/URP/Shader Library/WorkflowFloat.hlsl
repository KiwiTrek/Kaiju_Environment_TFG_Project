void WorkflowFloat_float(float metallic, float specular, out float Out)
{
	#ifdef _SPECULAR_SETUP
	Out = specular;
	#else
	Out = metallic;
	#endif
}