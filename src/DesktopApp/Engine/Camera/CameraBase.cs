using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesktopApp.Engine.Camera;

public abstract class CameraBase
{
    private Matrix _view;

    private Matrix _projection;

    public Matrix Projection
    {
        get { return _projection; }
        private set
        {
            _projection = value;
            GenerateFrustum();
        }
    }

    public Matrix View
    {
        get { return _view; }
        protected set
        {
            _view = value;
            GenerateFrustum();
        }
    }

    private BoundingFrustum Frustum { get; set; }

    private GraphicsDevice GraphicsDevice { get; }

    protected CameraBase(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;

        GeneratePerspectiveProjectionMatrix(MathHelper.PiOver4);
    }

    private void GeneratePerspectiveProjectionMatrix(float fieldOfView)
    {
        var pp = GraphicsDevice.PresentationParameters;

        var aspectRatio = (float)pp.BackBufferWidth / pp.BackBufferHeight;

        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000000.0f);
    }

    public virtual void Update()
    {
    }

    private void GenerateFrustum()
    {
        var viewProjection = View * Projection;
        Frustum = new BoundingFrustum(viewProjection);
    }

    public bool BoundingVolumeIsInView(BoundingSphere sphere)
    {
        return Frustum.Contains(sphere) != ContainmentType.Disjoint;
    }

    public bool BoundingVolumeIsInView(BoundingBox box)
    {
        return Frustum.Contains(box) != ContainmentType.Disjoint;
    }
}
