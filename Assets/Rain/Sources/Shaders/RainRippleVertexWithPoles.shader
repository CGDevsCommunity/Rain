Shader "CGDevs/Rain/Ripple Vertex with Pole"
{
    Properties
    {
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Normal ("Bump Map", 2D) = "white" {}
         _Roughness ("Metallic", 2D) = "white" {}
         _Occlusion ("Occlusion", 2D) = "white" {}

        _PoleTexture("PoleTexture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0
        _WaveMaxHeight("Wave Max Height", float) = 1
        _WaveMaxLength("Wave Length", float) = 1
        _Frequency("Frequency", float) = 1
        _Timer("Timer", Range(0,1)) = 0
        
        
    }
    SubShader
    {
        Tags {
        "IgnoreProjector" = "True"
            "RenderType" = "Opaque"}
        LOD 200
        CGPROGRAM
       
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        
        sampler2D _PoleTexture, _MainTex, _Normal, _Roughness, _Occlusion;       
        half _Glossiness, _WaveMaxHeight, _Frequency, _Timer, _WaveMaxLength, _RefractionK;
        fixed4 _Color;
        
        struct Input 
        {
            float2 uv_MainTex;
        };
        
        half getHeight(half x, half y, half offetX, half offetY, half radius, half phase)
        {
            const float PI = 3.14159;
            half timer = _Timer + phase;
            half rad = sqrt((x - offetX) * (x - offetX) + (y - offetY) * (y - offetY));
            half A = _WaveMaxHeight 
                    * sin(_Timer * PI) * (1 - _Timer)
                    * (1 - timer) * radius;
            half wavefunc = cos(2 * PI * (_Frequency * timer - rad / _WaveMaxLength));
            return A * wavefunc;
        }
        
        void vert (inout appdata_full v)  
        { 
            float4 poleParams = tex2Dlod (_PoleTexture, float4(v.texcoord.xy, 0, 0));
            v.vertex.z += getHeight(v.vertex.x, v.vertex.y, (poleParams.r - 0.5) * 2, (poleParams.g - 0.5) * 2, poleParams.b , poleParams.a);
        }
         
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
            o.Metallic = tex2D(_Roughness, IN.uv_MainTex).rgb;
            o.Occlusion = tex2D(_Occlusion, IN.uv_MainTex).rgb;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        

        ENDCG
    }
    FallBack "Diffuse"
}
