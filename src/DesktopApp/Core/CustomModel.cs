using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

internal class CustomModel
{
    public Vector3 Position { get; init; }

    public Vector3 Rotation { get; init; }

    public Vector3 Scale { get; init; }

    private Model Model { get; set; }

    private readonly Matrix[] _modelTransforms;
    private GraphicsDevice _graphicsDevice;
    private BoundingSphere _boundingSphere;

    public BoundingSphere BoundingSphere
    {
        get
        {
            // No need for rotation, as this is a sphere
            var worldTransform = Matrix.CreateScale(Scale)
                                 * Matrix.CreateTranslation(Position);

            var transformed = _boundingSphere;
            transformed = transformed.Transform(worldTransform);

            return transformed;
        }
    }

    public CustomModel(Model Model, GraphicsDevice graphicsDevice)
    {
        this.Model = Model;

        _modelTransforms = new Matrix[Model.Bones.Count];
        Model.CopyAbsoluteBoneTransformsTo(_modelTransforms);

        buildBoundingSphere();

        this._graphicsDevice = graphicsDevice;
    }

    public void Draw(Matrix View, Matrix Projection)
    {
        // Calculate the base transformation by combining : translation, rotation and scaling
        var baseWorld = Matrix.CreateScale(Scale)
                        * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                        * Matrix.CreateTranslation(Position);

        foreach (ModelMesh mesh in Model.Meshes)
        {
            var localWorld = _modelTransforms[mesh.ParentBone.Index]
                             * baseWorld;

            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
                var effect = (BasicEffect)meshPart.Effect;
                effect.World = localWorld;
                effect.View = View;
                effect.Projection = Projection;
                effect.EnableDefaultLighting();
            }

            mesh.Draw();
        }
    }

    private void buildBoundingSphere()
    {
        var sphere = new BoundingSphere(Vector3.Zero, 0);

        // Merge all the model's built in bounding spheres
        foreach (var mesh in Model.Meshes)
        {
            var transformed = mesh.BoundingSphere.Transform(
                _modelTransforms[mesh.ParentBone.Index]);

            sphere = BoundingSphere.CreateMerged(sphere, transformed);
        }

        this._boundingSphere = sphere;
    }
}
