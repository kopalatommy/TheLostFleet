Shader "Custom/HexagonOutline"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderThickness ("Border Thickness", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard vertex:vert

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        fixed4 _MainColor;
        fixed4 _BorderColor;
        float _BorderThickness;

        // Vertex shader to pass local position
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
        }

        // Fragment shader to draw the border
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calculate distance from the hexagon's edge (assuming hexagon is centered at 0,0)
            float2 pos = IN.localPos.xz;
            float distToEdge = 1.0 - max(abs(pos.x), abs(pos.y * 1.1547)); // Hexagon edge function
            
            if (distToEdge < _BorderThickness)
                o.Albedo = _BorderColor.rgb;
            else
                o.Albedo = _MainColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}