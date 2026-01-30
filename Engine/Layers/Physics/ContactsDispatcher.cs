using Box2D.NET;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Engine.Layers
{
    public struct Collision2D
    {
        public Collider2D Collider { get; internal set; }
        public Collider2D OtherCollider { get; internal set; }
        public int PointsCount { get; internal set; }
        public Transform Transform => OtherCollider.Transform;
        public Actor Actor => OtherCollider.Actor;

        internal B2ShapeId CurrentShapeId; // Remove
        internal B2ShapeId OtherShapeId; // Remove


        public void GetContacts(ref List<ContactPoint2D> contacts)
        {
            if (contacts == null)
            {
                contacts = new List<ContactPoint2D>();
            }
            else
            {
                contacts.Clear();
            }

            // TODO: refactor to implement multiple shapes
            throw new Exception(); // Remove

            var capacity = B2Shapes.b2Shape_GetContactCapacity(CurrentShapeId);

            var contactsInternal = new B2ContactData[capacity];
            var validCount = B2Shapes.b2Shape_GetContactData(CurrentShapeId, contactsInternal, contactsInternal.Length);

            for (int i = 0; i < validCount; i++)
            {
                var contact = contactsInternal[i];

                if ((B2Ids.B2_ID_EQUALS(contact.shapeIdA, CurrentShapeId) && B2Ids.B2_ID_EQUALS(contact.shapeIdB, OtherShapeId)) ||
                    (B2Ids.B2_ID_EQUALS(contact.shapeIdA, OtherShapeId) && B2Ids.B2_ID_EQUALS(contact.shapeIdB, CurrentShapeId)))
                {
                    for (int j = 0; j < contact.manifold.pointCount; j++)
                    {
                        var point = contact.manifold.points[j];
                        var id = point.id;

                        // TODO: fix adding extra contacts
                        contacts.Add(new ContactPoint2D()
                        {
                            Position = point.point.ToVec2(),
                            NormalImpulse = point.normalImpulse,
                            TangentImpulse = point.tangentImpulse,
                            Normal = contact.manifold.normal.ToVec2(),
                            NormalVelocity = point.normalVelocity,
                        });
                    }
                }
            }
        }

        public Collision2D()
        {
        }
    }

    // TODO: this class need cleanup asap
    internal class ContactsDispatcher
    {
        private readonly Action<ScriptBehavior, Collision2D> _onCollisionEnter = (x, y) => x.OnCollisionEnter2D(y);
        private readonly Action<ScriptBehavior, Collision2D> _onCollisionExit = (x, y) => x.OnCollisionExit2D(y);
        private readonly Action<ScriptBehavior, Collision2D> _onCollisionStay = (x, y) => x.OnCollisionStay2D(y);

        private readonly Action<ScriptBehavior, Collider2D> _onTriggerEnter = (x, y) => x.OnTriggerEnter2D(y);
        private readonly Action<ScriptBehavior, Collider2D> _onTriggerExit = (x, y) => x.OnTriggerExit2D(y);
        private readonly Action<ScriptBehavior, Collider2D> _onTriggerStay = (x, y) => x.OnTriggerStay2D(y);
        private readonly Action<Action<ScriptBehavior, Collision2D>, Collider2D, Collider2D> _collisionFuncEvent;
        private readonly Action<Action<ScriptBehavior, Collider2D>, Collider2D, Collider2D> _triggerFuncEvent;

        private OrderedDictionary<CollisionKey, CollisionValue> _contactEnter;
        private OrderedDictionary<CollisionKey, CollisionValue> _contactExit;
        private OrderedDictionary<CollisionKey, CollisionValue> _triggerEnter;
        private OrderedDictionary<CollisionKey, CollisionValue> _triggerExit;

        private Collision2D _collisionData;

        public ContactsDispatcher()
        {
            _contactEnter = new();
            _contactExit = new();
            _triggerEnter = new();
            _triggerExit = new();

            _collisionFuncEvent = OnCollision;
            _triggerFuncEvent = OnTrigger;

            _contactEnter.EnsureCapacity(200);
            _contactExit.EnsureCapacity(200);
            _triggerEnter.EnsureCapacity(100);
            _triggerExit.EnsureCapacity(100);
        }

        private bool GetCollisionKey(B2ShapeId shapeA, B2ShapeId shapeB, out CollisionKey key, out Collider2D colA, out Collider2D colB)
        {
            key = default;
            colA = null;
            colB = null;

            if (!(B2Worlds.b2Shape_IsValid(shapeA) && B2Worlds.b2Shape_IsValid(shapeB)))
                return false;

            colA = B2Shapes.b2Shape_GetUserData(shapeA) as Collider2D;
            colB = B2Shapes.b2Shape_GetUserData(shapeB) as Collider2D;

            if (!(colA && colB))
                return false;

            key = new CollisionKey(colA, colB);
            return true;
        }

        internal void Update()
        {
            // Contacts
            var contactsEvent = B2Worlds.b2World_GetContactEvents(PhysicWorld.WorldID);
            for (int i = 0; i < contactsEvent.beginCount; ++i)
            {
                var evt = contactsEvent.beginEvents[i];
                BeginContact(evt.shapeIdA, evt.shapeIdB);
            }

            for (int i = 0; i < contactsEvent.endCount; ++i)
            {
                var evt = contactsEvent.endEvents[i];
                EndContact(evt.shapeIdA, evt.shapeIdB);
            }

            // Sensor
            var sensorEvents = B2Worlds.b2World_GetSensorEvents(PhysicWorld.WorldID);
            for (int i = 0; i < sensorEvents.beginCount; ++i)
            {
                var evt = sensorEvents.beginEvents[i];
                BeginContact(evt.sensorShapeId, evt.visitorShapeId);
            }

            for (int i = 0; i < sensorEvents.endCount; ++i)
            {
                var evt = sensorEvents.endEvents[i];
                EndContact(evt.sensorShapeId, evt.visitorShapeId);
            }
        }

        public static void BeginContact(B2ShapeId shapeA, B2ShapeId shapeB)
        {
            var colA = B2Shapes.b2Shape_GetUserData(shapeA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeB) as Collider2D;

            if (colA == null || colB == null) return;

            bool isTrigger = colA.IsTrigger || colB.IsTrigger;

            colA.OnContactBegin(colB, isTrigger);
            colB.OnContactBegin(colA, isTrigger);
        }

        public static void EndContact(B2ShapeId shapeA, B2ShapeId shapeB)
        {
            var colA = B2Shapes.b2Shape_GetUserData(shapeA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeB) as Collider2D;

            if (colA == null || colB == null) return;

            bool isTrigger = colA.IsTrigger || colB.IsTrigger;

            colA.OnContactEnd(colB, isTrigger);
            colB.OnContactEnd(colA, isTrigger);
        }

        internal void OldUpdate()
        {

            // Contacts
            var contactsEvent = B2Worlds.b2World_GetContactEvents(PhysicWorld.WorldID);
            for (int i = 0; i < contactsEvent.beginCount; ++i)
            {
                var evt = contactsEvent.beginEvents[i];

                if (GetCollisionKey(evt.shapeIdA, evt.shapeIdB, out var key, out var colA, out var colB))
                {
                    if (_contactEnter.TryGetValue(key, out var current))
                    {
                        current.CollisionsCount++;
                        //Debug.Log("Contact ++ collision count: " + current.CollisionsCount);
                    }
                    else
                    {
                        _contactEnter.Add(key, new CollisionValue(colA, colB));
                    }

                    //var added = _contactEnter.Add(current);

                    //if (added)
                    //{
                    //    // Debug.Error($"Added: '{current.colliderA.Name}' and {current.colliderB.Name}\n");
                    //}
                }

            }

            for (int i = 0; i < contactsEvent.endCount; ++i)
            {
                var evt = contactsEvent.endEvents[i];
                if (GetCollisionKey(evt.shapeIdA, evt.shapeIdB, out var key, out _, out _))
                {
                    if (_contactEnter.TryGetValue(key, out var value))
                    {
                        if (value.CollisionsCount - 1 <= 0)
                        {
                            _contactExit.Add(key, value);
                        }
                        else
                        {
                            value.CollisionsCount--;
                        }
                    }
                }
            }

            // Sensor
            var sensorEvents = B2Worlds.b2World_GetSensorEvents(PhysicWorld.WorldID);
            for (int i = 0; i < sensorEvents.beginCount; ++i)
            {
                var evt = sensorEvents.beginEvents[i];

                if (GetCollisionKey(evt.sensorShapeId, evt.visitorShapeId, out var key, out var colA, out var colB))
                {
                    if (_triggerEnter.TryGetValue(key, out var current))
                    {
                        current.CollisionsCount++;
                        //Debug.Log("Trigger ++ collision count: " + current.CollisionsCount);
                    }
                    else
                    {
                        _triggerEnter.Add(key, new CollisionValue(colA, colB));
                    }
                }
            }

            for (int i = 0; i < sensorEvents.endCount; ++i)
            {
                var evt = sensorEvents.endEvents[i];
                if (GetCollisionKey(evt.sensorShapeId, evt.visitorShapeId, out var key, out var colA, out var colB))
                {
                    if (_triggerEnter.TryGetValue(key, out var value))
                    {
                        if (value.CollisionsCount - 1 <= 0)
                        {
                            _triggerExit.Add(key, value);
                        }
                        else
                        {
                            value.CollisionsCount--;
                        }
                    }
                }
            }

            RaiseEvents();
        }
        private void RaiseEvents()
        {
            OnEnter(_contactEnter, _collisionFuncEvent, _onCollisionEnter, _onCollisionStay);
            OnExit(_contactExit, _contactEnter, _collisionFuncEvent, _onCollisionExit);

            OnEnter(_triggerEnter, _triggerFuncEvent, _onTriggerEnter, _onTriggerStay);
            OnExit(_triggerExit, _triggerEnter, _triggerFuncEvent, _onTriggerExit);
        }

        private void OnEnter<T>(OrderedDictionary<CollisionKey, CollisionValue> enterCollisions, Action<Action<ScriptBehavior, T>, Collider2D, Collider2D> eventForwarder,
                                Action<ScriptBehavior, T> onEnterFunc, Action<ScriptBehavior, T> onStayFunc)
        {
            foreach (var (key, collision) in enterCollisions)
            {
                if (!collision.WasEnterEventRaised)
                {
                    collision.WasEnterEventRaised = true;

                    eventForwarder(onEnterFunc, collision.colliderA, collision.colliderB);
                    eventForwarder(onEnterFunc, collision.colliderB, collision.colliderA);
                }
                else
                {
                    eventForwarder(onStayFunc, collision.colliderA, collision.colliderB);
                    eventForwarder(onStayFunc, collision.colliderB, collision.colliderA);
                }
            }
        }

        private void OnExit<T>(OrderedDictionary<CollisionKey, CollisionValue> exitCollisions, OrderedDictionary<CollisionKey, CollisionValue> enterCollisions,
                               Action<Action<ScriptBehavior, T>, Collider2D, Collider2D> eventForwarder,
                               Action<ScriptBehavior, T> exitEvent)
        {
            foreach (var (key, val) in exitCollisions)
            {
                OnExit(key, enterCollisions, eventForwarder, exitEvent);
            }

            exitCollisions.Clear();
        }

        private void OnExit<T>(CollisionKey exitCollision, OrderedDictionary<CollisionKey, CollisionValue> enterCollisions,
                               Action<Action<ScriptBehavior, T>, Collider2D, Collider2D> eventForwarder,
                               Action<ScriptBehavior, T> exitEvent)
        {
            var found = enterCollisions.TryGetValue(exitCollision, out var enterCollision);

#if SHOW_ENGINE_MESSAGES
            if (!found)
            {
                Debug.Warn($"Collision exit already handled due deletion or disabling. A:{exitCollision.colliderA?.GetID()}, B:{exitCollision.colliderB?.GetID()}");
            }
#endif
            if (enterCollision.WasEnterEventRaised)
            {
                if (enterCollision.CollisionsCount - 1 <= 0)
                {
                    enterCollisions.Remove(exitCollision);
                    eventForwarder(exitEvent, enterCollision.colliderA, enterCollision.colliderB);
                    eventForwarder(exitEvent, enterCollision.colliderB, enterCollision.colliderA);
                }
                else if (enterCollision.colliderA && enterCollision.colliderB)
                {
                    enterCollision.CollisionsCount--;
                }
            }
        }

        /* Note: OnCollisionExit/OnTriggerExit can't be called automatically after a shape is
               destroyed when an actor/collider is destroyed because doing so could send invalid/destroyed actors.
               This function takes care of collecting all the OnCollisionExit/OnTriggerExit from collisions
               before the actors become invalid, so they can be called in the same frame.
        */
        internal void NotifyColliderToRemove(Collider2D currentCollider)
        {
            void AddToExit<T>(OrderedDictionary<CollisionKey, CollisionValue> enter,
                              Action<Action<ScriptBehavior, T>, Collider2D, Collider2D> eventForwarder,
                              Action<ScriptBehavior, T> exitEvent)
            {
                var values = enter.ToArray().Reverse();

                //for (int i = enter.Count - 1; i >= 0; i--)
                foreach (var kvp in values)
                {
                    if (kvp.Value.colliderA == currentCollider ||
                        kvp.Value.colliderB == currentCollider)
                    {
                        OnExit(kvp.Key, enter, eventForwarder, exitEvent);
                        enter.Remove(kvp.Key);
                    }
                }
            }

            if (!currentCollider.IsTrigger)
            {
                AddToExit(_contactEnter, _collisionFuncEvent, _onCollisionExit);
            }
            else
            {
                AddToExit(_triggerEnter, _triggerFuncEvent, _onTriggerExit);
            }
            //Debug.Log("Collision removed: ");
            //RemovePairsContaining(_contactEnter, currentCollider);
            //Debug.Log("Trigger removed: ");
            //RemovePairsContaining(_triggerEnter, currentCollider);
            //RemovePairsContaining(_contactExit, currentCollider);
            //RemovePairsContaining(_triggerExit, currentCollider);
        }

        //private void RemovePairsContaining(Dictionary<CollisionKey, CollisionValue> keys, Collider2D collider)
        //{
        //    int removed = keys.RemoveWhere(x => x.colliderA == collider || x.colliderB == collider);
        //    Debug.Log(removed);
        //}

        // Hack: helper to clear all when changing scenes.
        internal void ClearCollisions()
        {
            _contactEnter.Clear();
            _contactExit.Clear();
            _triggerEnter.Clear();
            _triggerExit.Clear();
        }

        private void OnCollision(Action<ScriptBehavior, Collision2D> action, Collider2D collA, Collider2D collB)
        {
            if (collA && collA.Actor && collB && collB.Actor)
            {
                _collisionData.Collider = collA;
                _collisionData.OtherCollider = collB;

                OnNotifyScripts(collA, collB, action, ref _collisionData);
            }

            _collisionData = default;
        }

        private void OnTrigger(Action<ScriptBehavior, Collider2D> action, Collider2D coll1, Collider2D coll2)
        {
            if (coll1 && (coll1.IsTrigger || coll2.IsTrigger) && coll1.Actor && coll2 && coll2.Actor)
            {
                OnNotifyScripts(coll1, coll2, action, ref coll2);
            }
        }

        private void OnNotifyScripts<T>(Collider2D current, Collider2D collided, Action<ScriptBehavior, T> funcEvent, ref T data)
        {
            if (current && current.Actor && current.Actor.IsActiveInHierarchy)
            {
                foreach (var component in current.Actor.Components)
                {
                    if (component && component.IsEnabled && component is ScriptBehavior script)
                    {
                        funcEvent(script, data);
                    }
                }
            }
        }
    }
}
