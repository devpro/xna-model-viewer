using DesktopApp.Core;
using DesktopApp.External;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp;

public class SimpleModel
  {
    #region Private Fields

    private Model _model;
    private Matrix[] _transforms;

    #endregion // Private Fields

    public Vector3 Position { get; set; }

    public Vector3 Rotation { get; set; }

    public float RotationY
    {
      get { return Rotation.Y; }
      set { Rotation = new Vector3(Rotation.X, value, Rotation.Z); }
    }

    public SimpleModel(string modelFileName, ContentManager Content)
    {
      LoadModel(modelFileName, Content);

      SetInitialPositionAndRotation();
    }

    public void Draw(SimpleCamera camera)
    {
      if (_model == null) { return; }

      Matrix worldMatrix = Matrix.Identity
          * Matrix.CreateRotationY(Rotation.Y)
          * Matrix.CreateTranslation(Position);

      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          effect.EnableDefaultLighting();
          effect.World = _transforms[mesh.ParentBone.Index] * worldMatrix;
          effect.View = camera.ViewMatrix;
          effect.Projection = camera.ProjectionMatrix;
        }
        mesh.Draw();
      }
    }

    private void LoadModel(string modelFileName, ContentManager Content)
    {
      _model = Content.Load<Model>(string.Format("{0}\\{1}", GameBase.ModelsPath, modelFileName));

      _transforms = CreateTransformMatrices(_model);

      BoundingSphere completeBoundingSphere = new BoundingSphere();
      foreach (ModelMesh mesh in _model.Meshes)
      {
        BoundingSphere origMeshSphere = mesh.BoundingSphere;
        BoundingSphere transMeshSphere = XnaUtils.TransformBoundingSphere(origMeshSphere, _transforms[mesh.ParentBone.Index]);
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

    private static Matrix[] CreateTransformMatrices(Model model)
    {
      Matrix[] transforms = new Matrix[model.Bones.Count];
      model.CopyAbsoluteBoneTransformsTo(transforms);
      return transforms;
    }

    private static Matrix[] AutoScale(Model model, float requestedSize)
    {
      float originalSize = GetModelSize(model);

      float scalingFactor = requestedSize / originalSize;
      model.Root.Transform = model.Root.Transform * Matrix.CreateScale(scalingFactor);
      Matrix[] modelTransforms = new Matrix[model.Bones.Count];
      model.CopyAbsoluteBoneTransformsTo(modelTransforms);
      return modelTransforms;
    }

    private static float GetModelSize(Model model)
    {
      BoundingSphere bSphere = (BoundingSphere)model.Tag;
      return bSphere.Radius * 2;
    }

    public float GetModelSize()
    {
      if (_model == null) { return 0.0f; }
      return GetModelSize(_model);
    }
  }
