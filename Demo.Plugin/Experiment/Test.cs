using System;
using System.Linq;
using System.Collections.Generic;
using Aximo.Render;
using Aximo.Engine;
using OpenToolkit.Mathematics;

namespace Aximo
{

    // some hints: https://www.sciencedirect.com/science/article/pii/S1474034616304037

    public class EntityAttribute
    {

    }

    public class EntityType
    {
        public string Name;
        public List<EntityTypeContraints> Contraints = new List<EntityTypeContraints>();

        public void Add(EntityTypeContraints contraint)
        {
            Contraints.Add(contraint);
        }

    }

    public class EntityTypeContraints
    {
        public string Name;
    }

    public class RequiredChildTypes : EntityTypeContraints
    {
        public EntityType Type;

        public int Minimum;
        public int? DefaultCount;
        public int? Maximum;
    }

    public class Entity
    {
        public string Name;
        public List<EntityAttribute> Attributes = new List<EntityAttribute>();
        public Entity Parent;
        public List<Entity> Childs = new List<Entity>();

        public List<EntityType> Classes = new List<EntityType>();

        public void Add(Entity child)
        {
            Childs.Add(child);
        }

        public void AddRange(params Entity[] childs)
        {
            foreach (var child in childs)
                Add(child);
        }

        public bool CanAdd(EntityType entityClass)
        {
            return true;
        }

        public bool CanAdd(Entity child)
        {
            return true;
        }

    }

    public class Test
    {

        private List<EntityType> Classes;

        public void DefineClasses()
        {
            var room = new EntityType() { Name = "Room" };
            var door = new EntityType() { Name = "Door" };
            var window = new EntityType() { Name = "Window" };

            room.Add(new RequiredChildTypes() { Type = door, Minimum = 1, DefaultCount = 1 }); // Rooms needs at least one door
        }

        public void Run()
        {
            var room = new Entity() { Name = "Room1" };
            var door = new Entity() { Name = "Door" };
            var win1 = new Entity() { Name = "Window1" };
            var win2 = new Entity() { Name = "Window1" };

            room.AddRange(door, win1, win2);
        }
    }
}