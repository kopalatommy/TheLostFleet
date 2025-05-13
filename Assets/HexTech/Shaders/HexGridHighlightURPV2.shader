// Shader "Custom/DOTSUnlitBaseColor"
// This shader provides a simple unlit color, intended for use with DOTS and MaterialOverride.
// It uses a single color property "_BaseColor" and supports DOTS instancing.
Shader "HexTech/HexGridHighlightURPv2"
{
    Properties
    {
        [Header(Basic Appearance)]
        // _BaseColor: The main color of the material.
        // This value is used if not overridden by DOTS per-instance data.
        [MainTexture] _BaseMap ("Base Color", Color) = (1,1,1,1)
        // Default color for the hexagons
        [MainColor] _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1.0)

        [Header(Highlighting)]
        // The color for highlighting the hexagon (Default is yellow)
        _HighlightColor("Highlight Color", Color) = (1.0, 1.0, 0.0, 1.0)
        // Emission for highlight, good for bloom
        [HDR]_EmissionColor("Highlight Emission", Color) = (0, 0, 0, 1)
        // How much the highlight blends with the base color
        _HighlightBlend("Highlight Blend Factor", Range(0.0, 1.0)) = 0.5

        [Header(Data From Script)]
        // The axial coordinates of the hexagon to highlight
        _HighlightedAxialCoords("Highlighted Axial Coords (Q, R, _, _)", Vector) = (0, 0, 0, 0)

        [HideInInspector] _BaseMap_ST ("Tiling/Offset", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 100

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ DOTS_INSTANCING_ON

            // Enable standard instancing and specifically the DOTS_INSTANCING_ON variant.
            // This is crucial for BatchRendererGroup / Entities Graphics.
            #pragma multi_compile_instancing
            #pragma instancing_options DOTS_INSTANCING_ON // Ensure this is processed for DOTS_INSTANCING_ON define

            // Include URP's core shader library.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // This buffer holds properties that are uniform for all instances using this material,
            // unless overridden by DOTS per-instance data.
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST; // Texture tiling and offset for _BaseMap (Unity convention)
                float4 _BaseColor;
                float4 _HighlightColor;
                float4 _EmissionColor;
                float _HighlightBlend;
                float4 _HighlightedAxialCoords; // .xy will contain q,r
            CBUFFER_END

            UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                UNITY_DOTS_INSTANCED_PROP(float4, _BaseMap_ST)
                UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DOTS_INSTANCED_PROP(float4, _HighlightColor)
                UNITY_DOTS_INSTANCED_PROP(float4, _EmissionColor)
                UNITY_DOTS_INSTANCED_PROP(float, _HighlightBlend)
                UNITY_DOTS_INSTANCED_PROP(float4, _HighlightedAxialCoords)
            UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

            // Texture and Sampler for _BaseMap.
            // TEXTURE2D and SAMPLER are URP macros for texture handling.
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // 'struct Attributes' defines the input data that each vertex of your mesh will provide to the vertex shader.
            // The : SEMANTIC part tells Unity where to get this data from the mesh.
            struct Attributes
            {
                float4 positionOS   : POSITION;     // Vertex position in Object Space.
                float2 uv           : TEXCOORD0;    // Primary UV coordinates (for _BaseMap).
                float2 axialCoord   : TEXCOORD2;    // Our custom axial coordinates (q,r) stored in the mesh's 3rd UV set (TEXCOORD2).
                float3 normalOS     : NORMAL;       // Vertex normal in Object Space (for lighting)
                UNITY_VERTEX_INPUT_INSTANCE_ID      // Required for instancing in URP.
            };

            // 'struct Varyings' defines the data that will be passed from the vertex shader to the fragment shader.
            // This data is interpolated across the face of each triangle.
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;  // Vertex position in Clip Space (the final screen position).
                float2 uv           : TEXCOORD0;    // Interpolated primary UV coordinates.
                float2 axialCoord   : TEXCOORD1;    // Interpolated axial coordinates (passed to an available TEXCOORD slot).
                float3 normalWS     : TEXCOORD2;    // Interpolated normal in World Space.
                UNITY_VERTEX_INPUT_INSTANCE_ID      // Pass instance ID.
                UNITY_VERTEX_OUTPUT_STEREO          // For stereo rendering (VR/AR).
            };
            
            // The Vertex Shader ('vert') processes each vertex of the mesh.
            // Its main job is to transform vertex positions and pass data to the fragment shader.
            Varyings vert(Attributes IN)
            {
                Varyings OUT; // Create an output structure.
                UNITY_SETUP_INSTANCE_ID(IN); // Setup for instancing.
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT); // Transfer instance ID.
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT); // Initialize for stereo rendering.

                // Transform vertex position from object space to clip space.
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // Transform normal from object space to world space.
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                // Pass through the primary UV coordinates, applying tiling and offset.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                // Pass through our custom axial coordinates.
                OUT.axialCoord = IN.axialCoord;

                return OUT; // Return the processed data.
            }

            // half4 frag(Varyings IN) : SV_Target
            // {
            //     UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN); // Setup for stereo rendering.
            //     // UNITY_SETUP_INSTANCE_ID(IN) initializes instancing for the fragment shader,
            //     // using the instance ID passed from the vertex shader.
            //     UNITY_SETUP_INSTANCE_ID(IN); // Must be called in the fragment shader too if accessing instance data.

            //     // Access the per-instance _BaseColor property.
            //     #if defined(DOTS_INSTANCING_ON)
            //         // If DOTS_INSTANCING_ON is defined, this reads from the DOTS instance data buffer.
            //         return UNITY_ACCESS_DOTS_INSTANCED_PROP(float4, _BaseColor);
            //     #else
            //         // Otherwise, it falls back to reading the global _BaseColor (from UnityPerMaterial).
            //         // This ensures the shader also works for non-instanced rendering or when DOTS_INSTANCING_ON is not active.
            //         return _BaseColor;
            //     #endif
            // }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN); // Setup for stereo rendering.
                // UNITY_SETUP_INSTANCE_ID(IN) initializes instancing for the fragment shader,
                // using the instance ID passed from the vertex shader.
                UNITY_SETUP_INSTANCE_ID(IN); // Must be called in the fragment shader too if accessing instance data.

                // 1. Calculate Base Appearance
                #if defined(DOTS_INSTANCING_ON)
                    // Sample the base texture using the UV coordinates.
                    half4 baseTexColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                    // Multiply by the base color property.
                    half4 calculatedBaseColor = baseTexColor * UNITY_ACCESS_DOTS_INSTANCED_PROP(float4, _BaseColor);
                #else
                    // Sample the base texture using the UV coordinates.
                    half4 baseTexColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                    // Multiply by the base color property.
                    half4 calculatedBaseColor = baseTexColor * _BaseColor;
                #endif

                // For basic lighting (optional, remove if purely unlit is desired)
                // Light mainLight = GetMainLight(); // Get information about the main directional light
                // half3 diffuse = mainLight.color * Saturate(dot(IN.normalWS, mainLight.direction));
                // calculatedBaseColor.rgb *= diffuse;

                // 2. Highlighting Logic
                // Define a small epsilon for floating-point comparison of axial coordinates.
                // Interpolated values might not be perfectly exact.
                float epsilon = 0.01f;

                float4 highlightedCoords = UNITY_ACCESS_DOTS_INSTANCED_PROP(float4, _HighlightedAxialCoords);

                // Check if this fragment's hexagon (IN.axialCoord) matches the one to be highlighted (_HighlightedAxialCoords).
                bool isHighlighted = (abs(IN.axialCoord.x - highlightedCoords.x) < epsilon &&
                                      abs(IN.axialCoord.y - highlightedCoords.y) < epsilon);

                half4 finalColor = calculatedBaseColor;
                half3 finalEmission = half3(0,0,0);

                if (isHighlighted)
                {
                    // If highlighted, linearly interpolate (lerp) between the base color and the highlight color.
                    // _HighlightBlend controls the mix: 0 = base color, 1 = highlight color.
                    // finalColor = lerp(calculatedBaseColor, UNITY_ACCESS_DOTS_INSTANCED_PROP(float4, _HighlightColor), UNITY_ACCESS_DOTS_INSTANCED_PROP(float, _HighlightBlend));
                    finalColor = half4(1, 0, 0, 1);
                    finalEmission = _EmissionColor.rgb; // Apply emission color for highlighted hex
                }

                // Combine final color with emission for URP (if you want it to contribute to bloom etc.)
                // This is a simplified way; full PBR would involve more.
                // For an unlit shader, you might just return finalColor.
                // For a lit/emissive effect:
                return half4(finalColor.rgb + finalEmission, finalColor.a);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
