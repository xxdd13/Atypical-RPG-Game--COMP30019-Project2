Shader "Custom/BigWaterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Strength("Strength", Range(0,2)) = 1.0
		_Speed("Speed", Range(-100,100)) = 100
	}

	SubShader {
		Tags { "RenderType"="Transparent" }
		
		Pass {

			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			float4 _Color;
			float _Strength;
			float _Speed;

			struct vertexInput {
				float4 vertex : POSITION;
			
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput input){
				vertexOutput output;

				float4 worldPos = mul(unity_ObjectToWorld, input.vertex);

				float4 displacement = (cos(worldPos.y) + cos(worldPos.x + (_Speed * _Time)/2.0 ));

				worldPos.y = worldPos.y + (displacement * _Strength);

				output.pos = mul(UNITY_MATRIX_VP, worldPos);

				return output;
			}

			float4 frag(vertexOutput output) : COLOR {
				return _Color;
			}

			ENDCG
		}

	}
	

}
