namespace AxEngine
{
    public class ScreenPipeline : RenderPipeline
    {


        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

}