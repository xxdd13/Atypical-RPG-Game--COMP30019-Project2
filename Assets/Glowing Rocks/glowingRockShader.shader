Shader "Custom/glowingRockShader" {

    Properties {
        
        _ColorTint("Color Tint", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(1.0, 10.0)) = 5.0

    }


    SubShader {

        Tags {"RenderType" = "Opaque"}


        CGPROGRAM
        #pragma surface surf Lambert
        
 
        struct Input {
            float4 color : Color;
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        float4 _ColorTint;
        sampler2D _MainTex;
        sampler2D _BumpMap;
        float4 _RimColor;
        float _RimPower;

        void surf (Input i, inout SurfaceOutput o) {

            i.color = _ColorTint;
            o.Albedo = tex2D (_MainTex, i.uv_MainTex).rgb * i.color;
            o.Normal = UnpackNormal(tex2D(_BumpMap,i.uv_BumpMap));

            half rim = 1.0 - saturate(dot(normalize(i.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow(rim, _RimPower);


        }
    
        ENDCG
        
    }

    FallBack "Diffuse"
}