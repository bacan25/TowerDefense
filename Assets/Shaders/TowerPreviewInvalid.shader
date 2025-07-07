Shader "Custom/TowerPreviewInvalid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 0, 0, 0.5)
        _RimPower ("Rim Power", Float) = 2.0
        _PulseSpeed ("Pulse Speed", Float) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _RimPower;
            float _PulseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Efecto rim para el contorno
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float rim = 1.0 - saturate(dot(viewDir, i.worldNormal));
                rim = pow(rim, _RimPower);
                
                // Efecto de pulso
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                
                col.rgb = lerp(col.rgb, _Color.rgb, 0.8);
                col.rgb += rim * _Color.rgb * (1.0 + pulse);
                col.a = _Color.a + rim * 0.3 + pulse * 0.1;
                
                return col;
            }
            ENDCG
        }
    }
}
