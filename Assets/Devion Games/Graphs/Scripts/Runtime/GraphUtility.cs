using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace DevionGames.Graphs
{
	public static class GraphUtility
	{
		public static T AddNode<T>(Graph graph) where T : Node
		{
			return AddNode(graph, typeof(T)) as T;
		}

		public static Node AddNode(Graph graph,System.Type type)
		{
			Node node = System.Activator.CreateInstance(type) as Node;
			if (typeof(FlowNode).IsAssignableFrom(type))
			{
				CreatePorts(node as FlowNode);
			}
			node.name = NicifyVariableName(type.Name);
			node.graph = graph;
			graph.nodes.Add(node);
			Save(graph);
			return node;
		}

		private static string NicifyVariableName(string name) {
			string result = "";
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsUpper(name[i]) == true && i != 0)
				{
					result += " ";
				}
				result += name[i];
			}
			return result;
		}

		public static void RemoveNodes(Graph graph, FlowNode[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				FlowNode node = nodes[i];
				node.DisconnectAllPorts();
			}
			graph.nodes.RemoveAll(x => nodes.Any(y => y == x ));
			Save(graph);
		}

		public static void RemoveNodes(Graph graph, Node[] nodes)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				Node node = nodes[i];
			}
			graph.nodes.RemoveAll(x => nodes.Any(y => y == x));
			Save(graph);
		}

		private static void CreatePorts(FlowNode node)
		{
			FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				if (field.HasAttribute<InputAttribute>())
				{
					InputAttribute inputAttribute = field.GetCustomAttribute<InputAttribute>();
					Port port = new Port(node, field.Name,field.FieldType, PortCapacity.Single, PortDirection.Input);
					port.drawPort = inputAttribute.port;
					port.label = inputAttribute.label;
					node.AddPort(port);
				}
				else if (field.HasAttribute<OutputAttribute>()) {
					Port port = new Port(node, field.Name,field.FieldType, PortCapacity.Multiple, PortDirection.Output);
					node.AddPort(port);
				}
			}
		}
			
		public static void Save(Graph graph)
		{
			List<Node> nodes = graph.nodes;
			Dictionary<string, object> graphData = new Dictionary<string, object>();
			List<UnityEngine.Object> objectReferences = new List<UnityEngine.Object>();
			Dictionary<string, object>[] nodeData = new Dictionary<string, object>[nodes.Count];

			for (int i = 0; i < nodes.Count; i++)
			{
				nodeData[i] = SerializeNode(nodes[i], ref objectReferences);
			}
			graphData.Add("Nodes",nodeData);
			graph.serializationData = MiniJSON.Serialize(graphData);
			graph.serializedObjects = objectReferences;
			//Debug.Log(graph.serializationData);
		}

		private static Dictionary<string,object> SerializeNode(Node node, ref List<UnityEngine.Object> objectReferences) {
			Dictionary<string, object> data = new Dictionary<string, object>() {
				{ "Type", node.GetType () },
			};


			SerializeFields(node, ref data, ref objectReferences);
			return data;
		}

		private static void SerializeFields(object obj, ref Dictionary<string, object> dic, ref List<UnityEngine.Object> objectReferences)
		{
			if (obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			FieldInfo[] fields = type.GetAllSerializedFields();

			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo field = fields[j];
				object value = field.GetValue(obj);
				SerializeValue(field.Name, value, ref dic, ref objectReferences);

			}
		}


		private static void SerializeValue(string key, object value, ref Dictionary<string, object> dic, ref List<UnityEngine.Object> objectReferences)
		{
			if (value != null && !dic.ContainsKey(key))
			{
				Type type = value.GetType();
				if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				{
					UnityEngine.Object unityObject = value as UnityEngine.Object;
					if (!objectReferences.Contains(unityObject))
					{
						objectReferences.Add(unityObject);
					}
					dic.Add(key, objectReferences.IndexOf(unityObject));
				}
				else if (typeof(LayerMask).IsAssignableFrom(type))
				{
					dic.Add(key, ((LayerMask)value).value);
				}
				else if (typeof(Enum).IsAssignableFrom(type))
				{
					dic.Add(key, (Enum)value);
				}
				else if (type.IsPrimitive ||
						 type == typeof(string) ||
						 type == typeof(Vector2) ||
						 type == typeof(Vector3) ||
						 type == typeof(Vector4) ||
						 type == typeof(Color))
				{
					dic.Add(key, value);
				}
				else if (typeof(IList).IsAssignableFrom(type))
				{
					IList list = (IList)value;
					Dictionary<string, object> s = new Dictionary<string, object>();
					for (int i = 0; i < list.Count; i++)
					{
						SerializeValue(i.ToString(), list[i], ref s, ref objectReferences);
					}
					dic.Add(key, s);
				}
				else
				{
					Dictionary<string, object> data = new Dictionary<string, object>();
					SerializeFields(value, ref data, ref objectReferences);
					dic.Add(key, data);
				}
			}
		}

		public static void Load(Graph graph)
		{
			if (string.IsNullOrEmpty(graph.serializationData)){
				return;
			}
			Dictionary<string, object> data = MiniJSON.Deserialize(graph.serializationData) as Dictionary<string, object>;
			graph.nodes.Clear();
			object obj;
			if (data.TryGetValue("Nodes", out obj))
			{
				List<object> list = obj as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					Node node = DeserializeNode(list[i] as Dictionary<string, object>, graph.serializedObjects);
					node.graph = graph;
					graph.nodes.Add(node);
				}

				for (int i = 0; i < graph.nodes.Count; i++) {
					graph.nodes[i].OnAfterDeserialize();
				}
			}
		}


		private static Node DeserializeNode(Dictionary<string, object> data, List<UnityEngine.Object> objectReferences)
		{
			string typeString = (string)data["Type"];
			Type type = Utility.GetType(typeString);
			if (type == null && !string.IsNullOrEmpty(typeString))
			{
				type = Utility.GetType(typeString);
			}
			Node node = (Node)System.Activator.CreateInstance(type);
			DeserializeFields(node, data, objectReferences);
			return node;
		}

		private static void DeserializeFields(object source, Dictionary<string, object> data, List<UnityEngine.Object> objectReferences)
		{
			if (source == null){return;}
			Type type = source.GetType();
			FieldInfo[] fields = type.GetAllSerializedFields();

			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo field = fields[j];
				object value = DeserializeValue(field.Name, source, field, field.FieldType, data, objectReferences);
				if (value != null)
				{
					field.SetValue(source, value);
				}
			}
		}



		private static object DeserializeValue(string key, object source, FieldInfo field, Type type, Dictionary<string, object> data, List<UnityEngine.Object> objectReferences)
		{
			object value;
			if (data.TryGetValue(key, out value))
			{
				if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				{
					int index = System.Convert.ToInt32(value);
					if (index >= 0 && index < objectReferences.Count)
					{
						return objectReferences[index];
					}
				}
				else if (typeof(LayerMask) == type)
				{
					LayerMask mask = new LayerMask();
					mask.value = (int)value;
					return mask;
				}
				else if (typeof(Enum).IsAssignableFrom(type))
				{
					return Enum.Parse(type, (string)value);
				}
				else if (type.IsPrimitive ||
						 type == typeof(string) ||
						 type == typeof(Vector2) ||
						 type == typeof(Vector3) ||
						 type == typeof(Vector4) ||
						 type == typeof(Quaternion) ||
						 type == typeof(Color))
				{
					return value;
				}
				else if (typeof(IList).IsAssignableFrom(type))
				{
					Dictionary<string, object> dic = value as Dictionary<string, object>;

					Type targetType = typeof(List<>).MakeGenericType(Utility.GetElementType(type));
					IList result = (IList)Activator.CreateInstance(targetType);
					int count = dic.Count;
					for (int i = 0; i < count; i++) {
						Type elementType = Utility.GetElementType(type);
			
						result.Add(DeserializeValue(i.ToString(), source, field, elementType, dic, objectReferences));
					}

					if (type.IsArray)
					{
						Array array = Array.CreateInstance(Utility.GetElementType(type), count);
						result.CopyTo(array, 0);
						return array;
					}
					return result;
				}
				else
				{
					Dictionary<string,object> dic= value as Dictionary<string,object>;
					if (dic.ContainsKey("m_Type")) {
						type = Utility.GetType((string)dic["m_Type"]);
					}
					object instance = Activator.CreateInstance(type);
					
					DeserializeFields(instance, value as Dictionary<string, object>, objectReferences);
					return instance;
				}
			}
			return null;
		}

		public static object ConvertToArray(this IList collection)
		{
			// guess type
			Type type;
			if (collection.GetType().IsGenericType && collection.GetType().GetGenericArguments().Length == 0)
				type = collection.GetType().GetGenericArguments()[0];
			else if (collection.Count > 0)
				type = collection[0].GetType();
			else
				throw new NotSupportedException("Failed to identify collection type for: " + collection.GetType());

			var array = (object[])Array.CreateInstance(type, collection.Count);
			for (int i = 0; i < array.Length; ++i)
				array[i] = collection[i];
			return array;
		}

		public static object ConvertToArray(this IList collection, Type arrayType)
		{
			var array = (object[])Array.CreateInstance(arrayType, collection.Count);
			for (int i = 0; i < array.Length; ++i)
			{
				var obj = collection[i];

				// if it's not castable, try to convert it
				if (!arrayType.IsInstanceOfType(obj))
					obj = Convert.ChangeType(obj, arrayType);

				array[i] = obj;
			}

			return array;
		}
	}
}