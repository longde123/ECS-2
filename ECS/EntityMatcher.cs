﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ECS
{
	// Will hold all the entities and return a specific pool of them, like a search engine of entities and components
	// this can be extended in much more methods
    public static partial class EntityMatcher
    {
		// All entities created
        public readonly static HashSet<Entity> subscribedEntities = new HashSet<Entity>();

        public delegate void EntityAdded(Entity ent);
        public static event EntityAdded OnEntityRegistered;

        // Entity constructor will subriscribe with this method
        public static void SubscribeEntity(Entity entity)
        {
            if (!subscribedEntities.Contains(entity))
            {
                subscribedEntities.Add(entity);
                if (OnEntityRegistered != null)
                    OnEntityRegistered(entity);
            }
            else Console.Write("Entity is already subscribed");
        }

		// Entity constructor will unsubriscribe with this method
        public static void UnsubscribeEntity(Entity entity)
        {
            if (subscribedEntities.Contains(entity)) subscribedEntities.Remove(entity);
        }

        public static bool MatchWithFilter(Filter request, Entity ent)
        {
            return ent.HasAllComponents(request.AllType.ToArray()) && ent.HasAnyComponent(request.AnyType.ToArray());
        }

        public static HashSet<Entity> FilterEntities(Filter request)
        {
            return FilterWithAnyComponent(request, 
                        FilterWithAllComponents(request, 
                            subscribedEntities
                        )
                    );
        }

        // Entities must have all specified matchers
        private static HashSet<Entity> FilterWithAllComponents(Filter request, HashSet<Entity> pool)
        {
            HashSet<Entity> result = new HashSet<Entity>();
            foreach(Entity ent in pool)
                if (ent.HasAllComponents(request.AllType.ToArray()))
                    result.Add(ent);

            return result;
        }

        // Entities can have at least one of specified matchers
        private static HashSet<Entity> FilterWithAnyComponent(Filter request, HashSet<Entity> pool)
        {
            HashSet<Entity> result = new HashSet<Entity>();
            foreach(Entity ent in pool)
                if (ent.HasAnyComponent(request.AnyType.ToArray()))
                    result.Add(ent);

            return result;
        }
    }
}