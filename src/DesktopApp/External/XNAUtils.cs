using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.External;

public static class XnaUtils
{
    public static BoundingBox TransformBoundingBox(BoundingBox origBox, Matrix matrix)
    {
        var origCorner1 = origBox.Min;
        var origCorner2 = origBox.Max;

        var transCorner1 = Vector3.Transform(origCorner1, matrix);
        var transCorner2 = Vector3.Transform(origCorner2, matrix);

        return new BoundingBox(transCorner1, transCorner2);
    }

    public static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
    {
        transformationMatrix.Decompose(out var scaling, out _, out _);

        var maxScale = scaling.X;
        if (maxScale < scaling.Y)
            maxScale = scaling.Y;
        if (maxScale < scaling.Z)
            maxScale = scaling.Z;

        var transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
        var transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

        var transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

        return transformedBoundingSphere;
    }

    public static Model LoadModelWithBoundingSphere(ref Matrix[] modelTransforms, string asset, ContentManager content)
    {
        var newModel = content.Load<Model>(asset);

        modelTransforms = new Matrix[newModel.Bones.Count];
        newModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

        var completeBoundingSphere = new BoundingSphere();
        foreach (var mesh in newModel.Meshes)
        {
            var origMeshSphere = mesh.BoundingSphere;
            var transMeshSphere = XnaUtils.TransformBoundingSphere(origMeshSphere, modelTransforms[mesh.ParentBone.Index]);
            completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
        }

        newModel.Tag = completeBoundingSphere;

        return newModel;
    }

    public static void DrawBoundingBox(BoundingBox bBox, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix,
        Matrix viewMatrix, Matrix projectionMatrix)
    {
        var v1 = bBox.Min;
        var v2 = bBox.Max;

        var cubeLineVertices = new VertexPositionColor[8];
        cubeLineVertices[0] = new VertexPositionColor(v1, Color.White);
        cubeLineVertices[1] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v1.Z), Color.Red);
        cubeLineVertices[2] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v2.Z), Color.Green);
        cubeLineVertices[3] = new VertexPositionColor(new Vector3(v1.X, v1.Y, v2.Z), Color.Blue);

        cubeLineVertices[4] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v1.Z), Color.White);
        cubeLineVertices[5] = new VertexPositionColor(new Vector3(v2.X, v2.Y, v1.Z), Color.Red);
        cubeLineVertices[6] = new VertexPositionColor(v2, Color.Green);
        cubeLineVertices[7] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v2.Z), Color.Blue);

        short[] cubeLineIndices = [0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7];

        basicEffect.World = worldMatrix;
        basicEffect.View = viewMatrix;
        basicEffect.Projection = projectionMatrix;
        basicEffect.VertexColorEnabled = true;
        device.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
        foreach (var pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                cubeLineVertices, 0, 8,
                cubeLineIndices, 0, 12
            );
        }
    }

    public static void DrawSphereSpikes(BoundingSphere sphere, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix,
        Matrix viewMatrix, Matrix projectionMatrix)
    {
        var up = sphere.Center + sphere.Radius * Vector3.Up;
        var down = sphere.Center + sphere.Radius * Vector3.Down;
        var right = sphere.Center + sphere.Radius * Vector3.Right;
        var left = sphere.Center + sphere.Radius * Vector3.Left;
        var forward = sphere.Center + sphere.Radius * Vector3.Forward;
        var back = sphere.Center + sphere.Radius * Vector3.Backward;

        var sphereLineVertices = new VertexPositionColor[6];
        sphereLineVertices[0] = new VertexPositionColor(up, Color.White);
        sphereLineVertices[1] = new VertexPositionColor(down, Color.White);
        sphereLineVertices[2] = new VertexPositionColor(left, Color.White);
        sphereLineVertices[3] = new VertexPositionColor(right, Color.White);
        sphereLineVertices[4] = new VertexPositionColor(forward, Color.White);
        sphereLineVertices[5] = new VertexPositionColor(back, Color.White);

        basicEffect.World = worldMatrix;
        basicEffect.View = viewMatrix;
        basicEffect.Projection = projectionMatrix;
        basicEffect.VertexColorEnabled = true;
        foreach (var pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.LineList, sphereLineVertices, 0, 3);
        }
    }

    public static VertexPositionColor[] VerticesFromVector3List(List<Vector3> pointList, Color color)
    {
        var vertices = new VertexPositionColor[pointList.Count];

        var i = 0;
        foreach (var p in pointList)
        {
            vertices[i++] = new VertexPositionColor(p, color);
        }

        return vertices;
    }

    public static BoundingBox CreateBoxFromSphere(BoundingSphere sphere)
    {
        var radius = sphere.Radius;
        var outerPoint = new Vector3(radius, radius, radius);

        var p1 = sphere.Center + outerPoint;
        var p2 = sphere.Center - outerPoint;

        return new BoundingBox(p1, p2);
    }
}
