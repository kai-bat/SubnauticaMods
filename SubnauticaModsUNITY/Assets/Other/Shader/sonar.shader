Shader "Shader Workshop/Chromadepth"
{
    Properties
    {
        // Ramp texture for the pulse wave, 
        // with a small bright spot & long tail.
        _MainTex ("Texture", 2D) = "white" {}
        // The speed at which the pulse travels, in cycles per second.
        // So 0.25 = 1 pulse every 4 seconds.
		_Pulse("Pulse Speed", float) = 0.25
			// Brightness of the edge detection lines.
		_Edge("Edge Highlight", float) = 1.0

		_Falloff("Alpha Falloff", float) = 1.0
    }
    SubShader
    {
        // Render after all opaque geometry.
        Tags {"Queue"="Transparent"}


        Pass
        {
			// Don't write to the depth buffer.
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"


            // Capture depth buffer as a readable texture.
            sampler2D _CameraDepthTexture;


            // Define a convenience method for sampling from this buffer.
            float sampleDepth(float2 uv) {
                // Sample depth buffer, linearized into the 0...1 range,
                // where 0 is the near plane and 1 is the far.
                return Linear01Depth(
                        UNITY_SAMPLE_DEPTH(
                            tex2D(_CameraDepthTexture, uv)));
            }


            // Access properties set in Material inspector.
            sampler2D _MainTex;
            float _Pulse;
            float _Edge;
			float _Falloff;

            // Project object vertices to clip space. 
            // Using an out parameter for SV_POSITION beacuse the VPOS
            // semantic we use below doesn't like living alongside SV_POSITION.
            void vert (
                            float4  vertex  : POSITION,
                        out float4  outpos  : SV_POSITION )
            {
                outpos = UnityObjectToClipPos(vertex);
            }


            // Take in the position of this fragment on the screen/render 
            // target using the VPOS semantic, so we can line up exactly 
            // with the rendered scene in the depth buffer we're sampling.
            fixed4 frag (
                    UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
                // Convert this screen position (in pixels)
                // to a UV coordinate (in the range 0...1).
                // (_ScreenParams.zw is (1/width + 1, 1/height + 1))
                float2 uv = screenPos.xy * (_ScreenParams.zw - 1.0f);


                // For Macs, you may need to set (y = 1.0f - y)
                // ...there's a way to do this cross-platform automatically,
                // but I don't have access to a Mac atm to test it. Sorry!


                // Use our depth sampling method above to get the depth
                // of the scene directly behind the pixel we're drawing.
                float depth = sampleDepth(uv);
				if (depth == 1) {
					return float4(0, 0, 0, 0);
				}


                // Initialize the edge detection to zero. Skip calculating
                // an edge detection if we're not going to use it.
                float edge = 0.0f;
                if (_Edge > 0.0f) {


                    // Calculate a 1-pixel offset in x+ y+, x- y- directions.
                    float4 offset = float4(1, 1, -1, -1)
                                    * (_ScreenParams.zwzw - 1.0f);


                    // Read the depth buffer 4 more times at offset positions
                    // to estimate the "average" depth near this pixel.
                    float average = 0.25f * (
                          sampleDepth(uv + offset.xy)
                        + sampleDepth(uv + offset.zy)
                        + sampleDepth(uv + offset.xw)
                        + sampleDepth(uv + offset.zw));


                    // The diference between the depth here and the average
                    // depth nearby will be zero in a flat area, and large
                    // where there's a depth cliff like a silhouette edge.
                    // We'll also get small non-zero values at crests and
                    // valley folds within a silhouette, and taking a square
                    // root helps emphasize this local detail.
                    edge = sqrt(abs(depth - average)) * _Edge;
                }


                // Here I make the depth values non-linear for style.
                // First inverting them so the far z is close to zero.
                depth = 1.0f - depth;
                // Then I square the result so far values get compressed
                // in a narrower range, and we stretch the range of
                // values in the near range (more detail close to the camera)
                depth *= depth;
                // Then finally I invert it back so zero values are near.
                depth = 1.0f - depth;


                // Change our depth value into a lookup into our warve texture.
                // This gives exactly one pulse at a time - the front wave needs
                // to reach the far plane before wrapping around to near again.
                // Increase that 1.0f * if you want multiple concurrent pulses.
                float samplePos = float4(1.0f * depth.xx, 0.0f, 0.0f);
                // Shift this sample window over time. (Set texture to repeat)
                samplePos.x -= _Pulse * _Time.y;


                // Sample the wave ramp texture at our computed sample position.
                fixed4 colour = tex2Dlod(_MainTex, samplePos);


                // Add edge brightening, tinted by the colour from the wave.
                colour *= (colour * (2.0f + edge * 30.0f) + edge * 5.0f);
                

                // Output our finished colour.
				float mult = 1 - (depth * _Falloff);
				if (mult < 0) {
					mult = 0;
				}

                return colour * mult;
            }       
            ENDCG
        }
    }
}