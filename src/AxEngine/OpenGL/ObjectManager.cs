using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{

    public interface IObjectHandle
    {
        int Handle { get; }
    }

    public interface IObjectIdentifier : IObjectHandle
    {
        ObjectLabelIdentifier ObjectLabelIdentifier { get; }
    }

    public interface IObjectLabel : IObjectIdentifier
    {
        string ObjectLabel { get; set; }
    }

    public static class ObjectManager
    {

        private static int MaxLabelLength = -1;

        public static void SetLabel(IObjectLabel obj)
        {
            // if (MaxLabelLength == -1)
            //     MaxLabelLength = GL.GetInteger((GetIndexedPName)(int)All.MaxLabelLength);

            var name = obj.ObjectLabelIdentifier.ToString() + " " + obj.Handle.ToString() + " [" + obj.ObjectLabel + "]";
            //RenderContext.Current.LogInfoMessage("Label:" + name);
            //name = "xxx\0";
            GL.ObjectLabel(obj.ObjectLabelIdentifier, obj.Handle, -1, name);
        }

        public static void PushDebugGroup(string verb, string nome)
        {
            var name = $"{verb} {nome}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, -1, name.Length, name);
        }

        public static void PushDebugGroup(string verb, IGameObject obj)
        {
            var objName = obj.Name;
            if (string.IsNullOrEmpty(objName))
                objName = obj.GetType().Name;
            var name = $"{verb} GameObject {obj.Id} [{objName}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, obj.Id, name.Length, name);
        }

        public static void PushDebugGroup(string verb, IRenderPipeline obj)
        {
            var name = $"{verb} RenderPipeline {obj.GetType().Name}]";
            GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, -1, name.Length, name);
        }

        public static void PopDebugGroup()
        {
            GL.PopDebugGroup();
        }

    }

}