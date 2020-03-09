Shader "Instanced/DebugDraw" {
	SubShader{
		Pass{

		CGPROGRAM

#pragma target 5.0
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
	float3 _Center;
	int _AFRT;
	int _ARES;
	int _StartIndex;
	int _EndIndex;
	float _AngleZ;
	StructuredBuffer<float3> buf_Points;
	StructuredBuffer<float3> buf_Positions;
	StructuredBuffer<int> buf_Distances;

	float IndexToDeg(int index) {
		return (index - _AFRT) * 360.0 / float(_ARES) + 90.0;
	}

	int DegToIndex(float deg) {
		return int((deg - 90.0) * float(_ARES) / 360.0 + float(_AFRT));
	}

	struct ps_input
	{
		float4 pos : SV_POSITION;
	};

	ps_input vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
	{
		ps_input o;
		
		
		float deg = _AngleZ + IndexToDeg(_StartIndex + inst);
		float3 pos = float3(cos(radians(deg)), sin(radians(deg)), 0) * buf_Distances[inst] * 0.001f;
		float3 worldPos = _Center + pos * id;
		o.pos = mul(UNITY_MATRIX_VP, float4(worldPos,1.0f));
		
		return o;
	}

	float4 frag(ps_input i) : COLOR
	{
		return float4(1, 0.5f, 0.0f, 1);
	}

		ENDCG

	}
	}

		Fallback Off
}