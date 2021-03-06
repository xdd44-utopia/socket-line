Shader "Custom/ClipShader"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
		_StepSize ("Step Size", float) = 0.01
		_OffsetX ("OffsetX", float) = 0
		_OffsetY ("OffsetY", float) = 0
		_OffsetZ ("OffsetZ", float) = 0
		_RotateY ("RotateY", float) = 0
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
			float _OffsetX;
			float _OffsetY;
			float _OffsetZ;
			float _RotateY;

			v2f vert (appdata v)
			{
				v2f o;

				o.objectVertex = v.vertex;
				o.objectVertex.x -= _WorldSpaceCameraPos.x;
				o.objectVertex.z -= _WorldSpaceCameraPos.y;

				o.worldVertex = mul(unity_ObjectToWorld, o.objectVertex).xyz;

				float3 camPos = _WorldSpaceCameraPos;
				o.vectorToSurface = o.worldVertex - camPos;

				o.clippingPos = mul(unity_ObjectToWorld, v.vertex).z - 0.5f;

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
				float inputX = 0.5f + o.x;
				float inputY = 0.5f - o.z;
				float inputZ = - o.y;
				inputX += -_OffsetX;
				inputY += _OffsetY;
				inputZ += -_OffsetZ;
				float resultX = (inputX - 0.5f) * cos(_RotateY) - (inputZ - 0.5f) * sin(_RotateY) + 0.5f;
				float resultY = inputY;
				float resultZ = (inputX - 0.5f) * sin(_RotateY) + (inputZ - 0.5f) * cos(_RotateY) + 0.5f;
				return float3(resultX, resultY, resultZ);
			}

			fixed4 frag(v2f iv) : SV_Target
			{
				// Start raymarching at the front surface of the object
				float3 rayOrigin = iv.objectVertex;

				// Use vector from camera to object surface to get ray direction
				float3 rayDirection = mul(unity_WorldToObject, float4(normalize(iv.vectorToSurface), 1));

				float4 color = float4(0, 0, 0, 0);
				float3 samplePosition = rayOrigin;
				samplePosition.y += iv.clippingPos;

				// Raymarch through object space
				for (int i = 0; i < MAX_STEP_COUNT; i++)
				{
					float3 transformedPos = TransformedPosition(samplePosition);
					if(max(abs(transformedPos.x), max(abs(transformedPos.y), abs(transformedPos.z))) < 1 + EPSILON) {
						float4 sampledColor = tex3D(_MainTex, transformedPos);
						if (sampledColor.r == sampledColor.g && sampledColor.g == sampledColor.b && sampledColor.a < 0.5f && sampledColor.a > 0.05f) {
							sampledColor.a *= 1.2f + samplePosition.y - rayOrigin.y - iv.clippingPos;
							sampledColor.a *= sampledColor.a;
						}
						color = BlendUnder(color, sampledColor);
						if (color.a > 0.5f) {
							break;
						}
						samplePosition += rayDirection * _StepSize;
					}
				}

				return color;
			}
			ENDCG
		}
	}
}