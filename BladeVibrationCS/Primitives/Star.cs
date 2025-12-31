using OpenTK.Graphics.OpenGL4;

namespace BladeVibrationCS.Primitives;
public abstract class APrimitive {
	public const int DEF_MAT_ID = 0;
	public readonly int starVBO;
	public readonly int starVAO;
	public readonly int starEBO;

	public readonly int IndicesN;

	public APrimitive ( float[] vertices, int[] indices ) {
		IndicesN = indices.Length;
		( starVBO, starVAO, starEBO ) = ModelHolder.PushModelToGPU ( vertices, indices );
	}

	public virtual void Render () {
		GL.BindVertexArray ( starVAO );
		GL.DrawElements ( PrimitiveType.Triangles, IndicesN, DrawElementsType.UnsignedInt, 0 );
	}
}

public class Star : APrimitive {
	public const float QuadSide = 0.3f;
	public const float QuadBase = -QuadSide / 2;
	public const float QuadTop = QuadBase + QuadSide;

	public readonly static float[] starVertices = {
			QuadBase,  QuadTop, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadTop,  QuadTop, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadTop,  QuadBase, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadBase,  QuadBase, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			0, 0,  QuadBase,			 1, 0, 0,  0, 0,  DEF_MAT_ID,
			};
	public readonly static int[] starIndices = {
			0, 1, 2,
			0, 1, 3,
			0, 1, 4,
			1, 2, 4,
			2, 3, 4,
			3, 0, 4,

			1, 2, 0,
			1, 3, 0,
			1, 4, 0,
			2, 4, 1,
			3, 4, 2,
			0, 4, 3,
			};

	public Star () : base ( starVertices, starIndices ) { }
}

public class Plane : APrimitive {
	public const float QuadSide = 999f;
	public const float QuadBase = -QuadSide / 2;
	public const float QuadTop = QuadBase + QuadSide;
	public readonly static float[] planeVertices = {
			QuadBase,  QuadTop, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadTop,  QuadTop, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadTop,  QuadBase, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			QuadBase,  QuadBase, 0,	 1, 0, 0,  0, 0,  DEF_MAT_ID,
			};
	public readonly static int[] planeIndices = {
			0, 1, 2,   1, 2, 0,
			2, 3, 0,   3, 0, 2,
			};

	public Plane () : base ( planeVertices, planeIndices ) { }
}

/// <summary>3 crossed planes centered at the origin.</summary>
public class Cross3 : APrimitive {
	public const float QuadSide = 999f;
	public const float QuadBase = -QuadSide / 2;
	public const float QuadTop = QuadBase + QuadSide;

	public readonly static float[] cross3Vertices = {
			// Plane XY
			QuadBase,  QuadTop, 0,
			QuadTop,  QuadTop, 0,
			QuadTop,  QuadBase, 0,
			QuadBase,  QuadBase, 0,
			// Plane XZ
			QuadBase, 0,  QuadTop,
			QuadTop, 0,  QuadTop,
			QuadTop, 0,  QuadBase,
			QuadBase, 0,  QuadBase,
			// Plane YZ
			0,  QuadBase,  QuadTop,
			0,  QuadTop,  QuadTop,
			0,  QuadTop,  QuadBase,
			0,  QuadBase,  QuadBase,
			};

	public readonly static int[] cross3Indices = {
		// Plane XY
		0, 1, 2,   1, 2, 0,		0, 0, 1,  0, 0,  DEF_MAT_ID,
		2, 3, 0,   3, 0, 2,		0, 0, 1,  0, 0,  DEF_MAT_ID,
		// Plane XZ
		4, 5, 6,   5, 6, 4,		0, 1, 0,  0, 0,  DEF_MAT_ID + 1,
		6, 7, 4,   7, 4, 6,		0, 1, 0,  0, 0,  DEF_MAT_ID + 1,
		// Plane YZ
		8, 9, 10,  9, 10, 8,		1, 0, 0,  0, 0,  DEF_MAT_ID + 2,
		10, 11, 8,  11, 8, 10,	1, 0, 0,  0, 0,  DEF_MAT_ID + 2,
		};

	public Cross3 () : base ( cross3Vertices, cross3Indices ) { }
}

public class SkyBox : APrimitive {
	public const float Size = 19f;
	public readonly static float[] skyboxVertices = {
		-Size, -Size, -Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		+Size, -Size, -Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		+Size, +Size, -Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		-Size, +Size, -Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		-Size, -Size, +Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		+Size, -Size, +Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		+Size, +Size, +Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
		-Size, +Size, +Size,  1, 0, 0,  0, 0,  DEF_MAT_ID,
	};
	public readonly static int[] skyboxIndices = {
		0, 1, 2,
		2, 3, 0,

		4, 5, 6,
		6, 7, 4,

		0, 1, 5,
		5, 4, 0,

		1, 2, 6,
		6, 5, 1,

		2, 3, 7,
		7, 6, 2,

		3, 0, 4,
		4, 7, 0,
	};
	public SkyBox () : base ( skyboxVertices, skyboxIndices ) { }
}