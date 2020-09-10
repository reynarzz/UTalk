Shader "Unlit/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistorsionTex("DistorsionTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct output
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            uniform sampler2D _MainTex;
            uniform sampler2D _DistorsionTex;

            output vert (float4 vertex : POSITION, float2 uv : TEXCOORD0, float4 COLOR : COLOR)
            {
                output o;

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = uv;
                o.color = COLOR;

                return o;
            }

            fixed4 frag(output o) : SV_Target
            {
                // sample the texture
                float4 uv = tex2D(_DistorsionTex, o.uv); 
                 
                fixed4 col = tex2D(_MainTex, o.uv);

                return col * o.color;
            }
            ENDCG
        }
    }
}