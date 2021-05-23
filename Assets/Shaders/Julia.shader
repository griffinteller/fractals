Shader "Hidden/julia" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Aspect ("Aspect Ratio", float) = 1
        _ZoomScale ("Zoom Scale", float) = 1  // 1 means left and right edge are -1 and 1, lower val -> more zoomed in
        _ZoomTrans ("Zoom Translation", Vector) = (0, 0, 0, 0)
        _DivergeLimit ("Diverge Limit", float) = 2
        _IterLimit ("Iteration Limit", float) = 100
        _CParameter ("C Parameter", Vector) = (0, 0, 0, 0)
        _ColorDecay ("Color Decay", float) = 10
    }
    SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define DIVERGE_LIMIT 2
            #define ITER_LIMIT 100

            uniform sampler2D _MainTex;
            uniform float _Aspect;
            uniform float _ZoomScale;
            uniform float4 _ZoomTrans;
            uniform float _DivergeLimit;
            uniform float _IterLimit;
            uniform float4 _CParameter;
            uniform float _ColorDecay;

            float2 complexMul(const float2 a, const float2 b)
            {
                return float2(a.x * b. x - a.y * b.y, a.x * b.y + a.y * b.x);
            }

            float4 frag(const v2f_img i) : COLOR {
                const float3 adjustedUv = float3(i.uv.x * 2 - 1, (i.uv.y * 2 - 1) / _Aspect, 1);
                float2 coord = mul(
                    float3x3(_ZoomScale, 0, _ZoomTrans.x, 0, _ZoomScale, _ZoomTrans.y, 0, 0, 1),
                    adjustedUv);

                for (int j = 0; j < _IterLimit; j++)
                {
                    coord = complexMul(coord, coord);
                    coord += _CParameter.xy;

                    if (length(coord) > _DivergeLimit)
                        return float4(sin(j / _ColorDecay) / 2 + 0.5, cos(j / _ColorDecay) / 2 + 0.5, 0, 1);
                }

                return float4(0, 0, 0, 1);
            }

            ENDCG
        }
    }
}
