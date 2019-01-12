Shader "CGDevs/Rain/RainRipple"
{
    Properties
    {
        _WaveHeight("Wave Height", float) = 1
        _WaveLength("Wave Length", float) = 1
        _Frequency("Frequency", float) = 1
        _Radius("Radius", float) = 1
        _Timer("Timer", Range(0,1)) = 0
        _Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"= "Opaque" }
        LOD 200
        
        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input 
        {
            float2 uv_MainTex;
        };
        half _Glossiness, _Metallic, _Frequency, _Timer, _WaveLength, _WaveHeight, _Radius;
        fixed4 _Color;
        
        half getHeight(half x, half y)
        {
            const float PI = 3.14159;
            half rad = sqrt(x * x + y * y);
            half wavefunc = _WaveHeight * sin(_Timer * PI) * (1 - _Timer) * clamp(_Radius - rad, 0, 1)
                * cos(2 * PI * (_Frequency * _Timer - rad / _WaveLength));
            return wavefunc;
        }
        void vert (inout appdata_full v)  
        {
             v.vertex.z -= getHeight(v.vertex.x, v.vertex.y);
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
