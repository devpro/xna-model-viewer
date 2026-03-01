using System.Linq;
using DesktopApp.Engine.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Engine.Model;

public class SimpleModel
{
    private Microsoft.Xna.Framework.Graphics.Model _model;

    private Matrix[] _transforms;

    public Vector3 Position { get; private set; }

    public Vector3 Rotation { get; private set; }

    public float RotationY
    {
        get { return Rotation.Y; }
        set { Rotation = new Vector3(Rotation.X, value, Rotation.Z); }
    }

    public SimpleModel(string modelFileName, ContentManager content)
    {
        LoadModel(modelFileName, content);

        SetInitialPositionAndRotation();
    }

    public void Draw(SimpleCamera camera)
    {
        if (_model == null) { return; }

        var worldMatrix = Matrix.Identity
                          * Matrix.CreateRotationY(Rotation.Y)
                          * Matrix.CreateTranslation(Position);

        foreach (var mesh in _model.Meshes)
        {
            foreach (var effect in mesh.Effects.Cast<BasicEffect>())
            {
                effect.EnableDefaultLighting();
                effect.World = _transforms[mesh.ParentBone.Index] * worldMatrix;
                effect.View = camera.ViewMatrix;
                effect.Projection = camera.ProjectionMatrix;
            }
            mesh.Draw();
        }
    }

    private void LoadModel(string modelFileName, ContentManager content)
    {
        _model = content.Load<Microsoft.Xna.Framework.Graphics.Model>(modelFileName);

        _transforms = CreateTransformMatrices(_model);

        var completeBoundingSphere = new BoundingSphere();

        foreach (var mesh in _model.Meshes)
        {
            var origMeshSphere = mesh.BoundingSphere;
            var transMeshSphere = TransformBoundingSphere(origMeshSphere, _transforms[mesh.ParentBone.Index]);
            completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
        }

        _model.Tag = completeBoundingSphere;

        //_model = AutoScale(_model, 10.0f);
    }

    private void SetInitialPositionAndRotation()
    {
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
    }

    private static Matrix[] CreateTransformMatrices(Microsoft.Xna.Framework.Graphics.Model model)
    {
        var transforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(transforms);
        return transforms;
    }

    private static Matrix[] AutoScale(Microsoft.Xna.Framework.Graphics.Model model, float requestedSize)
    {
        var originalSize = GetModelSize(model);

        var scalingFactor = requestedSize / originalSize;
        model.Root.Transform = model.Root.Transform * Matrix.CreateScale(scalingFactor);
        var modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(modelTransforms);
        return modelTransforms;
    }

    private static float GetModelSize(Microsoft.Xna.Framework.Graphics.Model model)
    {
        var bSphere = (BoundingSphere)model.Tag;
        return bSphere.Radius * 2;
    }

    public float GetModelSize()
    {
        return _model == null ? 0.0f : GetModelSize(_model);
    }


    private static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
    {
        transformationMatrix.Decompose(out var scaling, out _, out _);

        var maxScale = scaling.X;
        if (maxScale < scaling.Y) maxScale = scaling.Y;
        if (maxScale < scaling.Z) maxScale = scaling.Z;

        var transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
        var transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

        var transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

        return transformedBoundingSphere;
    }
}
