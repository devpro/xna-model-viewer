using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Core;

class CustomModel
{
    public Vector3         Position { get; set; }
    public Vector3         Rotation { get; set; }
    public Vector3         Scale    { get; set; }

    public Model           Model    { get; private set; }

    private Matrix[]       modelTransforms;
    private GraphicsDevice graphicsDevice;
    private BoundingSphere boundingSphere;

    public BoundingSphere  BoundingSphere
    {
        get
        {
            // No need for rotation, as this is a sphere
            var worldTransform = Matrix.CreateScale(Scale)
                                 * Matrix.CreateTranslation(Position);

            var transformed = boundingSphere;
            transformed = transformed.Transform(worldTransform);

            return transformed;
        }
    }

    public CustomModel(Model Model, GraphicsDevice graphicsDevice)
    {
        this.Model = Model;

        modelTransforms = new Matrix[Model.Bones.Count];
        Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

        buildBoundingSphere();

        this.graphicsDevice = graphicsDevice;
    }

    public void Draw(Matrix View, Matrix Projection)
    {
        // Calculate the base transformation by combining : translation, rotation and scaling
        var baseWorld = Matrix.CreateScale(Scale)
                        * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                        * Matrix.CreateTranslation(Position);

        foreach (ModelMesh mesh in Model.Meshes) {
            var localWorld = modelTransforms[mesh.ParentBone.Index]
                             * baseWorld;

            foreach (ModelMeshPart meshPart in mesh.MeshParts) {
                var effect         = (BasicEffect)meshPart.Effect;
                effect.World       = localWorld;
                effect.View        = View;
                effect.Projection  = Projection;
                effect.EnableDefaultLighting();
            }

            mesh.Draw();
        }
    }

    private void buildBoundingSphere()
    {
        var sphere = new BoundingSphere(Vector3.Zero, 0);

        // Merge all the model's built in bounding spheres
        foreach (var mesh in Model.Meshes) {
            var transformed = mesh.BoundingSphere.Transform(
                modelTransforms[mesh.ParentBone.Index]);

            sphere = BoundingSphere.CreateMerged(sphere, transformed);
        }

        this.boundingSphere = sphere;
    }
}
