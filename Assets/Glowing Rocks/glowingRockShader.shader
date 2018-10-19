Shader "Custom/glowingRockShader" {

    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _GlowColor("Glow Color", Color) = (1,1,1,1)
        _GlowStrength("Glow Strength", Range(1.0, 10.0)) = 5.0

    }


    SubShader {

        Tags {"RenderType" = "Opaque"}

        CGPROGRAM
        #pragma surface surf Lambert

        float4 _Color;
        float4 _GlowColor;
        float _GlowStrength;
        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input {
            float4 color : Color;
            float2 mainTex;
            float2 bumpMap;
            float3 viewDir;
        };

        void surf (Input i, inout SurfaceOutput o) {

            i.color = _Color;
            o.Albedo = tex2D (_MainTex, i.mainTex).rgb * i.color;
            o.Normal = UnpackNormal(tex2D(_BumpMap,i.bumpMap));
            half glow = 1.0 - saturate(dot(normalize(i.viewDir), o.Normal));
            o.Emission = _GlowColor.rgb * pow(glow, _GlowStrength);

        }
    
        ENDCG
        
    }

    FallBack "Diffuse"
}