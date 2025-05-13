// Shader "Custom/DOTSUnlitBaseColor"
// This shader provides a simple unlit color, intended for use with DOTS and MaterialOverride.
// It uses a single color property "_BaseColor" and supports DOTS instancing.
Shader "Custom/DOTSUnlitBaseColor"
{
    Properties
    {
        // _BaseColor: The main color of the material.
        // This value is used if not overridden by DOTS per-instance data.
        _BaseColor ("Base Color", Color) = (1,1,1,1)
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
            // For Hybrid Renderer V2 / Entities Graphics 0.50+ DOTS_INSTANCING_ON is often automatically handled
            // by the #pragma multi_compile_instancing and URP's setup.
            // However, explicitly defining instancing_options can be necessary for some setups or older versions.
            
            // Include URP's core shader library.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Crucial include for DOTS instancing macros:
            // UNITY_DOTS_INSTANCING_BUFFER_START/END, UNITY_DOTS_INSTANCED_PROP, UNITY_ACCESS_DOTS_INSTANCED_PROP.
            // This path assumes the Entities Graphics package is installed.
            // If you still get errors, verify this path matches your project's package structure for Entities Graphics.
            // #if defined(DOTS_INSTANCING_ON) // Only include if DOTS_INSTANCING_ON is actually defined by the compile options
            //     #include "Packages/com.unity.entities.graphics/Unity.Entities.Graphics/Utilities/EntityGraphicsInstancing.hlsl"
            // #endif


            // This buffer holds properties that are uniform for all instances using this material,
            // unless overridden by DOTS per-instance data.
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor; // Default color, used if not DOTS instanced or no override.
            CBUFFER_END

            // Define a buffer for DOTS per-instance properties.
            // This block will only be effective if DOTS_INSTANCING_ON is defined and the include above is successful.
            // #if defined(DOTS_INSTANCING_ON)
            // UNITY_DOTS_INSTANCING_BUFFER_START(MyDOTSInstancedProps) // Name of the CBuffer for DOTS instanced properties
            //     // Declare _BaseColor as a per-instance property for DOTS.
            //     // The type (float4) and name (_BaseColor) must match the C# component's
            //     // [MaterialProperty] attribute and the shader's Properties block.
            //     UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
            // UNITY_DOTS_INSTANCING_BUFFER_END(MyDOTSInstancedProps)
            // #endif

            UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
            UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // UNITY_VERTEX_INPUT_INSTANCE_ID is required for instancing.
                // It adds the necessary instance ID input to the vertex shader.
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                // We need to pass the instance ID to the fragment shader
                // to access per-instance properties there.
                UNITY_VERTEX_INPUT_INSTANCE_ID // Propagate instance ID
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // UNITY_SETUP_INSTANCE_ID(IN) initializes the instancing system for the vertex shader,
                // making per-instance data accessible (e.g. for transformations).
                UNITY_SETUP_INSTANCE_ID(IN); // This macro must be called before accessing instance data.
                // UNITY_TRANSFER_INSTANCE_ID(IN, OUT) passes the instance ID from vertex input to varyings.
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT); // This macro copies the instance ID.

                // Transform the vertex position from object space to clip space.
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(IN) initializes instancing for the fragment shader,
                // using the instance ID passed from the vertex shader.
                UNITY_SETUP_INSTANCE_ID(IN); // Must be called in the fragment shader too if accessing instance data.

                // Access the per-instance _BaseColor property.
                #if defined(DOTS_INSTANCING_ON)
                    // If DOTS_INSTANCING_ON is defined, this reads from the DOTS instance data buffer.
                    return UNITY_ACCESS_DOTS_INSTANCED_PROP(float4, _BaseColor);
                #else
                    // Otherwise, it falls back to reading the global _BaseColor (from UnityPerMaterial).
                    // This ensures the shader also works for non-instanced rendering or when DOTS_INSTANCING_ON is not active.
                    return _BaseColor;
                #endif
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
