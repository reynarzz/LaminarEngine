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

    internal class ContactsDispatcher
    {
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
            if (!B2Worlds.b2Shape_IsValid(shapeA) || !B2Worlds.b2Shape_IsValid(shapeB))
            {
#if DEBUG
                Debug.Warn("invalid shape");
#endif
                return;
            }

            var colA = B2Shapes.b2Shape_GetUserData(shapeA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeB) as Collider2D;

            if (!colA || !colB)
            {
                return;
            }

            colA.OnContactBegin(colB);
            colB.OnContactBegin(colA);
        }

        public static void EndContact(B2ShapeId shapeA, B2ShapeId shapeB)
        {
            if (!B2Worlds.b2Shape_IsValid(shapeA) || !B2Worlds.b2Shape_IsValid(shapeB))
            {
#if DEBUG
                Debug.Warn("invalid shape");
#endif
                return;
            }

            var colA = B2Shapes.b2Shape_GetUserData(shapeA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeB) as Collider2D;

            if (!colA || !colB)
            {
                return;
            }
            colA.OnContactEnd(colB);
            colB.OnContactEnd(colA);
        }
    }
}
