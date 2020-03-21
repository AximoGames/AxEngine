using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Aximo.Render;
using OpenTK;
using System.Collections.Concurrent;

namespace Aximo.Engine
{

    public class GameTexture
    {
        private Texture Texture;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2i Size => new Vector2i(Width, Height);

        private GameTexture(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public string SourcePath { get; private set; }
        public string Label { get; private set; }

        public static GameTexture GetFromFile(string sourcePath)
        {
            Console.WriteLine($"Loading: {sourcePath}");
            var imagePath = DirectoryHelper.GetAssetsPath(sourcePath);
            Bitmap bitmap;

            if (sourcePath.ToLower().EndsWith(".tga"))
                bitmap = TgaDecoder.FromFile(imagePath);
            else
                bitmap = new Bitmap(imagePath);

            var txt = new GameTexture(bitmap.Width, bitmap.Height);
            txt.SourcePath = sourcePath;
            txt.Label = Path.GetFileName(sourcePath);
            txt.AutoDisposeBitmap = true;
            txt.SetData(bitmap);
            return txt;
        }

        private bool AutoDisposeBitmap = false;

        public static GameTexture GetFromBitmap(Bitmap bitmap, string name, bool autoDisposeBitmap = false)
        {
            var txt = new GameTexture(bitmap.Width, bitmap.Height);
            txt.Label = name;
            txt.AutoDisposeBitmap = true;
            txt.SetData(bitmap);
            return txt;
        }

        private Bitmap Bitmap;

        private bool HasChanges;
        private bool BitmapChanged;

        public void SetData(Bitmap bitmap)
        {
            if (bitmap.Width != Width || bitmap.Height != Height)
                throw new InvalidOperationException();

            Bitmap = bitmap;
            BitmapChanged = true;
            HasChanges = true;
        }

        internal Texture InternalTexture;

        public void Sync()
        {
            if (!HasChanges)
                return;
            HasChanges = false;

            if (InternalTexture == null)
            {
                InternalTexture = new Texture(Bitmap);
                InternalTexture.ObjectLabel = Label;
            }
            else
            {
                if (BitmapChanged)
                {
                    BitmapChanged = false;
                    InternalTexture.SetData(Bitmap);
                }
            }
        }

    }

    public static class MaterialManager
    {

        public static GameMaterial _DefaultMaterial;
        public static GameMaterial DefaultMaterial
        {
            get
            {
                if (_DefaultMaterial == null)
                {
                    _DefaultMaterial = new GameMaterial
                    {
                        DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                        SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                    };
                }
                return _DefaultMaterial;
            }
        }

        public static GameMaterial CreateNewMaterial()
        {
            return new GameMaterial();
        }

    }

    public class GameMaterial
    {

        public static GameMaterial Default => MaterialManager.DefaultMaterial;

        private static int LastMaterialId = 0;
        public int MaterialId { get; private set; }
        internal Material InternalMaterial;

        public GameTexture DiffuseTexture;
        public GameTexture SpecularTexture;

        // private Shader Shader;
        // private Shader DefGeometryShader;
        // private Shader ShadowShader;
        // private Shader CubeShadowShader;

        public GameMaterial()
        {
            MaterialId = Interlocked.Increment(ref LastMaterialId);
        }

        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; }
        public float SpecularStrength { get; set; }
        public MaterialColorBlendMode ColorBlendMode { get; set; }

        private Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();

        public void AddParameter(string name, Vector3 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Vector3));
        }

        public void AddParameter(Parameter parameter)
        {
            Parameter param;
            if (Parameters.TryGetValue(parameter.Name, out param))
            {
                if (object.Equals(parameter.Value, param.Value))
                    return;

                if (parameter.Type == param.Type)
                    return;

                param.Value = parameter.Value;
                param.HasChanges = true;
            }
            else
            {
                parameter.HasChanges = true;
                Parameters.Add(parameter.Name, parameter);
            }
        }

        internal virtual void PropagateChanges()
        {
        }

        internal virtual void SyncChanges()
        {
            // if (Shader == null)
            //     Shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            // if (DefGeometryShader == null)
            //     DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");
            // if (ShadowShader == null)
            //     ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag", "Shaders/shadow-directional.geom");
            // if (CubeShadowShader == null)
            //     CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");

            DiffuseTexture.Sync();
            SpecularTexture.Sync();

            if (InternalMaterial == null)
            {
                InternalMaterial = new Material();
                InternalMaterial.CreateShaders();
            }

            var mat = InternalMaterial;
            mat.txt0 = DiffuseTexture.InternalTexture;
            mat.txt1 = SpecularTexture.InternalTexture;

            foreach (var param in Parameters.Values)
            {
                if (param.HasChanges)
                {
                    param.HasChanges = false;
                    switch (param.Type)
                    {
                        case ParamterType.Bool:
                            mat.Shader.SetBool(param.Name, (bool)param.Value);
                            mat.DefGeometryShader.SetBool(param.Name, (bool)param.Value);
                            break;
                        case ParamterType.Int:
                            mat.Shader.SetInt(param.Name, (int)param.Value);
                            mat.DefGeometryShader.SetInt(param.Name, (int)param.Value);
                            break;
                        case ParamterType.Float:
                            mat.Shader.SetFloat(param.Name, (float)param.Value);
                            mat.DefGeometryShader.SetFloat(param.Name, (float)param.Value);
                            break;
                        case ParamterType.Vector2:
                            mat.Shader.SetVector2(param.Name, (Vector2)param.Value);
                            mat.DefGeometryShader.SetVector2(param.Name, (Vector2)param.Value);
                            break;
                        case ParamterType.Vector3:
                            mat.Shader.SetVector3(param.Name, (Vector3)param.Value);
                            mat.DefGeometryShader.SetVector3(param.Name, (Vector3)param.Value);
                            break;
                        case ParamterType.Vector4:
                            mat.Shader.SetVector4(param.Name, (Vector4)param.Value);
                            mat.DefGeometryShader.SetVector4(param.Name, (Vector4)param.Value);
                            break;
                        case ParamterType.Matrix4:
                            mat.Shader.SetMatrix4(param.Name, (Matrix4)param.Value);
                            mat.DefGeometryShader.SetMatrix4(param.Name, (Matrix4)param.Value);
                            break;
                    }
                }
            }
        }

        public class Parameter
        {
            public string Name;
            public object Value;
            public ParamterType Type;
            public bool HasChanges;

            public Parameter(string name, object value, ParamterType type)
            {
                Name = name;
                Value = value;
                Type = type;
            }
        }

        public enum ParamterType
        {
            Vector2,
            Vector3,
            Vector4,
            Matrix4,
            Float,
            Int,
            Bool,
            Texture,
        }

    }

}