using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DevionGames.Graphs
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InputAttribute : Attribute
    {
        public readonly bool label = true;
        public readonly bool port = true;

        public InputAttribute() { }

        public InputAttribute(bool label, bool port) {
            this.label = label;
            this.port = port;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class OutputAttribute : Attribute
    {

        public OutputAttribute() { }
        
    }

    public enum PortDirection { Input = 0 , Output = 1 }
    public enum PortCapacity {Single = 0, Multiple = 1}

    [System.Serializable]
    public class Edge {

        public string nodeId;
        public string fieldName;

        [NonSerialized] 
        public Port port;

    }


    [System.Serializable]
    public class Port
    {
        [System.NonSerialized]
        public FlowNode node;

        private Type m_FieldType;
        public Type fieldType {
            get { 
                if (this.m_FieldType == null) {
                    this.m_FieldType = Utility.GetType(this.m_FieldTypeName);
                }
                return this.m_FieldType;
            }
        }

        public string fieldName;
        public bool drawPort = true;
        public bool label = true;
        public PortCapacity capacity = PortCapacity.Single;
        public PortDirection direction = PortDirection.Input;


        [SerializeField]
        private List<Edge> m_Connections;
        public List<Edge> Connections
        {
            get
            {
                return this.m_Connections;
            }
        }

        //Changed to public because it was using not FullName, and need to change that. Will be switched back to private
        [SerializeField]
        public string m_FieldTypeName;

        public Port() {
            m_Connections = new List<Edge>();
        }

        public Port(FlowNode node,string fieldName, Type fieldType,PortCapacity capacity, PortDirection direction )
        {
            m_Connections = new List<Edge>();
            this.node = node;
            this.fieldName = fieldName;
            this.capacity = capacity;
            this.direction = direction;
            this.m_FieldTypeName = fieldType.FullName;
        }

        public virtual T GetValue<T>(T defaultValue = default)
        {
            if (direction == PortDirection.Input)
            {

                if (Connections.Count > 0)
                {
                    return Connections[0].port.GetValue<T>();
                }
                return defaultValue;
            }

            object value = node.OnRequestValue(this);

            if (value == null && typeof(T).IsValueType)
            {
                throw new InvalidCastException(
                    $"Cannot cast null to value type `{typeof(T).FullName}`"
                );
            }

            if (value == null || typeof(T).IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception e)
            {
                throw new InvalidCastException(
                    $"Cannot cast `{value.GetType()}` to `{typeof(T)}`. Error: {e}."
                );
            }
        }

        public virtual IEnumerable<T> GetValues<T>()
        {
            if (direction == PortDirection.Input)
            {
                if (Connections.Count > 0)
                {
                    for (var i = 0; i < Connections.Count; i++)
                    {
                        yield return Connections[i].port.GetValue<T>();
                    }
                }
            }

            var values = node.OnRequestValue(this) as IEnumerable<T>;
            foreach (var value in values)
            {
                yield return value;
            }
        }

        public void Connect(Port port)
        {
            m_Connections.Add(new Edge()
            {
                nodeId = port.node.id,
                port = port,
                fieldName = port.fieldName
            }); 
            port.m_Connections.Add(new Edge() {
                port = this,
                nodeId = node.id,
                fieldName = fieldName
            });
        }

        public void Disconnect(Port port)
        {
            this.m_Connections.RemoveAll(x => x.nodeId == port.node.id && x.fieldName == port.fieldName);
            port.Connections.RemoveAll(x =>x.nodeId == node.id && x.fieldName == fieldName);
        }

        public void DisconnectAll()
        {
            for (var i = 0; i < this.m_Connections.Count; i++)
            {
                var port = this.m_Connections[i].port;
                port.m_Connections.RemoveAll(x => x.nodeId == node.id && x.fieldName == fieldName);
            }
            this.m_Connections.Clear();
        }
    }
}