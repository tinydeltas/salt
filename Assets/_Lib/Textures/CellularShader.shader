Shader "Cellular"
{
    Properties
    {
        _Resolution ("Resolution", Range(2,64)) = 64
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Density;

            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv / _Resolution;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
	                // based on the paper 
				int x, y, n, h;
				float curX, curY; 

				reset (distArr);

				for (int a = -1; a < 2; a++) {
					for (int b = -1; b < 2; b++) {
						x = cubeX + a; 
						y = cubeY + b;

						// hash the cube
						// create the random number generator by seeding it with the hash 
						Random.InitState ((541 * x + 79 * y) % int.MaxValue);

						// determine the number of feature points 
						n = probLookup (Random.value);

						// for each feature point, place it in the cube 
						for (int i = 0; i < n; i++) {
							curX = x + Random.value; 
							curY = y + Random.value;

							// by inserting into sorted list 
						 insert (manhattan (pX, pY, curPt));
						}
					}
				}
					
				// color distance function 
				float color = randDiff (distArr); 
	            }
            ENDCG
        }
    }
}