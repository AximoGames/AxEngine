using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

public class Actor
{
    public int ActorId { get; private set; }

    private List<GComponent> _Components;
    public ICollection<GComponent> Components { get; private set; }

    private static int LastGameObjectId;

    private static int GetNewGameObject()
    {
        return Interlocked.Increment(ref LastGameObjectId);
    }

    public Actor()
    {
        ActorId = GetNewGameObject();
        _Components = new List<GComponent>();
        Components = new ReadOnlyCollection<GComponent>(_Components);
    }

    public void AddComponent(GComponent component)
    {
        Components.Add(component);
        component.AddActor(this);
    }

}

public class GComponent
{
    public int ComponentId { get; private set; }

    private List<Actor> _Actors;

    public ICollection<GComponent> Actors { get; private set; }

    internal void AddActor(Actor actor)
    {
        _Actors.Add(actor);
    }

}

public abstract class GLightComponent : GComponent
{

}

public class GPointLightComponent : GLightComponent
{

}

public class GDirectionalLightComponent : GLightComponent
{

}

public class GGeometryComponent : GComponent
{

}

public class GStaticMeshComponent : GGeometryComponent
{

}
