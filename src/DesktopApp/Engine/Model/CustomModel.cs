using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Engine.Model;

internal class CustomModel
{
    public Vector3 Position { get; init; }

    public Vector3 Rotation { get; init; }

    public Vector3 Scale { get; init; }

    private Microsoft.Xna.Framework.Graphics.Model Model { get; set; }

    private readonly Matrix[] _modelTransforms;

    private GraphicsDevice _graphicsDevice;

    private BoundingSphere _boundingSphere;

    public BoundingSphere BoundingSphere
    {
        get
        {
            var worldTransform = Matrix.CreateScale(Scale)
                                 * Matrix.CreateTranslation(Position);

            var transformed = _boundingSphere;
            transformed = transformed.Transform(worldTransform);

            return transformed;
        }
    }

    public CustomModel(Microsoft.Xna.Framework.Graphics.Model model, GraphicsDevice graphicsDevice)
    {
        Model = model;

        _modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(_modelTransforms);

        BuildBoundingSphere();

        _graphicsDevice = graphicsDevice;
    }

    public void Draw(Matrix view, Matrix projection)
    {
        // calculates the base transformation by combining translation, rotation and scaling
        var baseWorld = Matrix.CreateScale(Scale)
                        * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                        * Matrix.CreateTranslation(Position);

        foreach (var mesh in Model.Meshes)
        {
            var localWorld = _modelTransforms[mesh.ParentBone.Index]
                             * baseWorld;

            foreach (var meshPart in mesh.MeshParts)
            {
                var effect = (BasicEffect)meshPart.Effect;
                effect.World = localWorld;
                effect.View = view;
                effect.Projection = projection;
                effect.EnableDefaultLighting();
            }

            mesh.Draw();
        }
    }

    private void BuildBoundingSphere()
    {
        var sphere = new BoundingSphere(Vector3.Zero, 0);

        // merges all the model's built in bounding spheres
        sphere = Model.Meshes
            .Select(mesh => mesh.BoundingSphere.Transform(_modelTransforms[mesh.ParentBone.Index]))
            .Aggregate(sphere, BoundingSphere.CreateMerged);

        _boundingSphere = sphere;
    }
}
