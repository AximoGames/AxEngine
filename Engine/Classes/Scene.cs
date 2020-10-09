using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class Scene
    {

        public Scene()
        {
            Actors = new List<Actor>();
        }

        public bool Enabled { get; internal set; }

        public List<Actor> GetRootGameObjects()
        {
            throw new NotImplementedException();
        }

        public string? Name { get; set; }

        public IList<Actor> GetActors()
        {
            return Actors;
        }

        private readonly IList<Actor> Actors;

        public void AddActor(Actor act)
        {
            lock (Actors)
            {
                Actors.Add(act);
                act.Scene = this;
            }

            act.OnAddedToScene();
        }

    }

}
