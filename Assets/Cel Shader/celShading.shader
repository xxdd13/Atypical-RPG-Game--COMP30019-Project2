Shader "Custom/celShading" {

	Properties {
        _Color("Diffuse Material Color", Color) = (1,1,1,1)
        _UnlitColor ("Unlit Color", Color) = (0.5,0.5,0.5,1)
        _DiffuseThreshold("Lighting Threshold", Range(-1.1,1)) = 0.1
        _SpecularColor("Specular Material Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.5,1)) = 1
        _OutlineThickness("Outline Thickness", Range(0,1)) = 0.1
		_MainTex ("Main Texture", 2D) = "white" {}
	}
	SubShader {
		
        // Tags { "LightMode"="ForwardBase" }
		Pass {
        
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            uniform float4 _Color;
            uniform float4 _UnlitColor;
            uniform float _DiffuseThreshold;
            uniform float4 _SpecularColor;
            uniform float _Shininess;
            uniform float _OutlineThickness;

            uniform float4 _LightColor0;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            struct vertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texCoord : TEXCOORD0;

            };

            struct vertexOutput {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD1;
                float4 lightDir : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float2 uv : TEXCOORD0;

            };


            // Source:  https://www.youtube.com/watch?v=3qBDTh9zWrQ

            vertexOutput vert(vertexInput i){
                vertexOutput o;

                // Get normal direction
                o.normalDir = normalize(mul(float4(i.normal, 0.0), unity_WorldToObject).xyz);

                // Get world position 
                float4 posWorld = mul(unity_ObjectToWorld, i.vertex);

                // Get View Direction (vector from object to camera)
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);

                // light direction 
                float3 fragmentToLightSource = (_WorldSpaceCameraPos.xyz - posWorld.xyz);
                o.lightDir = float4(
                    normalize(lerp(_WorldSpaceLightPos0.xyz, fragmentToLightSource, _WorldSpaceLightPos0.w)),
                    lerp(1.0, 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w));

                     o.pos = UnityObjectToClipPos(i.vertex);
                     o.uv = i.texCoord;

                 return o; 
            }

            float4 frag(vertexOutput i) : COLOR {

                int offset = 10;

                float nDotL = saturate(dot(i.normalDir, i.lightDir.xyz));

                // Diffuse threshold calculation
                float diffuseCutoff = saturate((max(_DiffuseThreshold, nDotL) - _DiffuseThreshold) * offset);


                // Specular Thresholod Calculation
                float specularCutoff = saturate(max(_Shininess, dot(reflect(-i.lightDir.xyz, i.normalDir), i.viewDir)) - _Shininess) *offset;

                // Calculate outlines
                float outlineStrength = saturate((dot(i.normalDir, i.viewDir) - _OutlineThickness) * offset);

                // General ambient illumination
                float3 ambientLight = (1-diffuseCutoff) * _UnlitColor.xyz;
                float3 diffuseReflection = (1-specularCutoff) * _Color.xyz * diffuseCutoff;
                float3 specularReflection = _SpecularColor.xyz * specularCutoff;

                float3 combinedLight = (ambientLight + diffuseReflection) * outlineStrength + specularReflection;


                return float4(combinedLight, 1.0) + tex2D(_MainTex, i.uv);
            }




			ENDCG
		}
	}
}
