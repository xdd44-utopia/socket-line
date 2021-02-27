Shader "Custom/ClipShader"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
		_StepSize ("Step Size", float) = 0.01
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend One OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members clippingPos)
#pragma exclude_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Maximum amount of raymarching samples
			#define MAX_STEP_COUNT 128

			// Allowed floating point inaccuracy
			#define EPSILON 0.00001f

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 objectVertex : TEXCOORD0;
				float3 worldVertex : TEXCOORD1;
				float3 vectorToSurface : TEXCOORD2;
				float clippingPos : TEXCOORD3;
			};

			sampler3D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;
			float _StepSize;

			RWStructuredBuffer<float3> buffer : register(u1);

			v2f vert (appdata v)
			{
				v2f o;

				// Vertex in object space this will be the starting point of raymarching
				o.objectVertex = v.vertex;
				o.objectVertex.x -= _WorldSpaceCameraPos.x;
				o.objectVertex.z -= _WorldSpaceCameraPos.y;

				// Calculate vector from camera to vertex in world space
				o.worldVertex = mul(unity_ObjectToWorld, o.objectVertex).xyz;

				float3 camPos = _WorldSpaceCameraPos;
				o.vectorToSurface = o.worldVertex - camPos;

				o.clippingPos = mul(unity_ObjectToWorld, v.vertex).z;

				buffer[0] = o.clippingPos;

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 BlendUnder(float4 color, float4 newColor)
			{
				color.rgb += (1.0 - color.a) * newColor.a * newColor.rgb;
				color.a += (1.0 - color.a) * newColor.a;
				return color;
			}

			float3 TransformedPosition(float3 o) {
				return float3(o.x, 1-o.z, -o.y);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// Start raymarching at the front surface of the object
				float3 rayOrigin = i.objectVertex;

				// Use vector from camera to object surface to get ray direction
				float3 rayDirection = mul(unity_WorldToObject, float4(normalize(i.vectorToSurface), 1));

				float4 color = float4(0, 0, 0, 0);
				float3 samplePosition = rayOrigin;
				samplePosition.y += i.clippingPos;

				// Raymarch through object space
				for (int i = 0; i < MAX_STEP_COUNT; i++)
				{
					// Accumulate color only within unit cube bounds
					//if(max(abs(samplePosition.x), max(abs(samplePosition.y), abs(samplePosition.z))) < 0.5f + EPSILON)
					//{
						float4 sampledColor = tex3D(_MainTex, TransformedPosition( samplePosition + float3(0.5f, 0.5f, 0.5f)));
						//sampledColor.a *= _Alpha;
						color = BlendUnder(color, sampledColor);
						samplePosition += rayDirection * _StepSize;
					//}
				}

				return color;
			}
			ENDCG
		}
	}
}