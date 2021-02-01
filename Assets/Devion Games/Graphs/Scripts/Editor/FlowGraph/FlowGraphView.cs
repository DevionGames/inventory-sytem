using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

namespace DevionGames.Graphs
{
    [System.Serializable]
    public class FlowGraphView : GraphView<FlowNode>
    {
        [SerializeField]
        private FlowGraph m_Graph;
        [SerializeField]
        private UnityEngine.Object m_Target;
        private const int NODE_MIN_WIDTH = 55;
        private const int NODE_HEADER_HEIGHT = 36;
        private const int NODE_LINE_HEIGHT = 34;
        private const int NODE_PORT_OFFSET = 20;
        private const int NODE_PORT_SIZE = 11;
        private const int NODE_CONTENT_OFFSET = 10;
        private const int NODE_FIELD_HEIGHT = 18;

        private Vector2 m_CreateNodePosition;
        private Port m_ConnectingPort;


        private string m_Copy;

        public FlowGraphView(EditorWindow host, FlowGraph graph, UnityEngine.Object target) : base(host) {
            this.m_Graph = graph;
            this.m_Target = target;
        }

        protected override FlowNode[] inspectedNodes {
            get {
                return this.m_Graph.nodes.Cast<FlowNode>().ToArray();
            
            }
        }

        protected override void DrawNodeConnections(FlowNode[] nodes, Vector2 offset)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes[i].OutputPorts.Count; j++)
                {
                    Port port = nodes[i].OutputPorts[j];
                    for (int k = 0; k < port.Connections.Count; k++) {
                        DrawConnection(GetPortRect(port).center, GetPortRect(port.Connections[k].port).center, ConnectionStyle.Line, new Color(0.506f, 0.62f, 0.702f, 1f));
                    }
                }
            }
        }


        protected override void ConnectNodes(FlowNode[] nodes, Vector2 offset)
        {
            Event currentEvent = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        for (int j = 0; j < nodes[i].Ports.Count; j++)
                        {
                            Rect portRect = GetPortRect(nodes[i].Ports[j]);
                            if (portRect.Contains(Event.current.mousePosition)) {
                                m_ConnectingPort = nodes[i].GetPort(j);
                                GUIUtility.hotControl = controlID;
                                currentEvent.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        bool connected = false;
                        for (int i = 0; i < nodes.Length; i++)
                        {
                            for (int j = 0; j < nodes[i].Ports.Count; j++)
                            {
                                Rect portRect = GetPortRect(nodes[i].Ports[j]);
                                if (portRect.Contains(currentEvent.mousePosition)) {
                                    if (m_ConnectingPort.node == nodes[i]) {
                                        Debug.LogWarning("You can't connect to same node.");
                                        break;
                                    }

                                    if (m_ConnectingPort.direction == nodes[i].GetPort(j).direction) {
                                        Debug.LogWarning("You can't connect two "+m_ConnectingPort.direction.ToString()+" ports.");
                                        break;
                                    }
                                    this.m_ConnectingPort.Connect(nodes[i].GetPort(j));
                                    connected = true;
                                    break;
                                }
                            }
                        }

                        if (!connected && this.m_ConnectingPort != null) {

                            this.m_ConnectingPort.DisconnectAll();

                        }
                        GraphUtility.Save(this.m_Graph);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(this.m_Target);
                        m_ConnectingPort = null;
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }  
                    break;
            }

            if (m_ConnectingPort != null && this.m_ConnectingPort.node != null)
            {
                DrawConnection(GetPortRect(this.m_ConnectingPort).center, currentEvent.mousePosition,ConnectionStyle.Line, new Color(0.506f, 0.62f, 0.702f, 1f));
                m_Host.Repaint();
            }
        }

        protected override void DrawNode(Rect rect, FlowNode node, bool selected)
        {
            GUI.Box(rect, GUIContent.none, Styles.nodeNormal);
            DrawNodeHeader(rect, node, selected);
            DrawPorts(node);
            DrawPortFields(rect, node);
           
            if (node is EventNode){
                GUI.Box(rect, GUIContent.none, Styles.nodeActive);
            }else if (selected){
                GUI.Box(rect, GUIContent.none, Styles.nodeSelected);
            }
        }

        private void DrawNodeHeader(Rect rect, FlowNode node, bool selected)
        {
            NodeStyleAttribute nodeStyle = node.GetType().GetCustomAttribute<NodeStyleAttribute>();
            if (nodeStyle == null) { nodeStyle = new NodeStyleAttribute(true); }
            if (!nodeStyle.displayHeader){return;}

            Styles.nodeHeaderText.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
            Texture2D icon = Resources.Load<Texture2D>(nodeStyle.iconPath);
            if (icon != null)
            {
                GUI.Label(new Rect(rect.x + NODE_CONTENT_OFFSET, rect.y + NODE_HEADER_HEIGHT * 0.5f - icon.height * 0.5f, icon.width, icon.height), icon);
            }

            GUI.Label(new Rect(rect.x + NODE_CONTENT_OFFSET + (icon != null ? icon.width + 3 : 0), rect.y, rect.width, NODE_HEADER_HEIGHT), node.name, Styles.nodeHeaderText);
            if (Event.current.type == EventType.Repaint)
            {
                Styles.seperator.Draw(new Rect(rect.x, rect.y + NODE_HEADER_HEIGHT, rect.width, 1), false, false, false, false);
            }
        }


        private void DrawPortFields(Rect rect, FlowNode node) {
            float labelWidth = GetLabelWidth(node);
            float fieldWidth = GetFieldWidth(node);
            for (int i = 0; i < node.Ports.Count; i++)
            {
                Port port = node.GetPort(i);
                if (port.direction == PortDirection.Input)
                {
                    Rect labelRect = new Rect(rect.x + NODE_CONTENT_OFFSET, rect.y + NODE_LINE_HEIGHT * i + GetHeaderRect(node, this.m_GraphOffset).height + (NODE_LINE_HEIGHT - NODE_FIELD_HEIGHT) * 0.5f, labelWidth, NODE_FIELD_HEIGHT);
                  
                    if (port.Connections.Count == 0)
                    {
                        FieldInfo field = port.node.GetType().GetField(port.fieldName);
                        object value = field.GetValue(port.node);
                        if (port.label)
                        {
                            GUI.Label(labelRect, ObjectNames.NicifyVariableName(port.fieldName), Styles.nodeText);
                        }

                        Rect fieldRect = new Rect(rect.x + NODE_CONTENT_OFFSET + (port.label?labelWidth+NODE_CONTENT_OFFSET:0f), rect.y + NODE_LINE_HEIGHT * i + GetHeaderRect(node, this.m_GraphOffset).height + (NODE_LINE_HEIGHT - NODE_FIELD_HEIGHT) * 0.5f, fieldWidth, NODE_FIELD_HEIGHT);
                        EditorGUI.BeginChangeCheck();
                        if (port.fieldType == typeof(float))
                        {
                            value = EditorGUI.FloatField(fieldRect, (float)value);
                        } else if (port.fieldType == typeof(string)) {
                            value = EditorGUI.TextField(fieldRect, (string)value);
                        } else if (port.fieldType == typeof(AnimationCurve)) {
                            value = EditorGUI.CurveField(fieldRect,(AnimationCurve) value);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            field.SetValue(port.node, value);
                            GraphUtility.Save(this.m_Graph);
                            PrefabUtility.RecordPrefabInstancePropertyModifications(this.m_Target);
                        }
                    }
                    else
                    {
                        GUI.Label(labelRect, ObjectNames.NicifyVariableName(port.fieldName), Styles.nodeText);
                    }
                }

                NodeStyleAttribute nodeStyle = node.GetType().GetCustomAttribute<NodeStyleAttribute>();
                if (nodeStyle == null) { nodeStyle = new NodeStyleAttribute(true); }
                Texture2D icon = Resources.Load<Texture2D>(nodeStyle.iconPath);
                if (!nodeStyle.displayHeader && icon != null)
                {
                    GUI.Label(new Rect(rect.x + rect.width - NODE_CONTENT_OFFSET - icon.width, rect.y + rect.height * 0.5f - icon.height * 0.5f, icon.width, icon.height), icon);
                }


                if (Event.current.type == EventType.Repaint )// && port.drawPort)
                {
                    if (i > 0 && i < node.Ports.Count - 1)
                    {
                        Styles.seperator.Draw(new Rect(rect.x, rect.y+ GetHeaderRect(node, this.m_GraphOffset).height + i * NODE_LINE_HEIGHT, !nodeStyle.displayHeader && icon != null ? GetContentWidth(node)+NODE_CONTENT_OFFSET*1.6f:rect.width , 1f), false, false, false, false);
                    }
                }
            }
        }


        private void DrawPorts(FlowNode node)
        {
            for (int i = 0; i < node.Ports.Count; i++)
            {
                Port port = node.GetPort(i);
                Rect portRect = GetPortRect(port);
                GUIStyle portStyle = (port.Connections.Count() > 0 || this.m_ConnectingPort == port) ? Styles.portConnected : Styles.port;

                /*if(port.direction== PortDirection.Output && EditorApplication.isPlaying)
                    GUI.Label(new Rect(portRect.x,portRect.y-18f,50f,20f),node.OnRequestValue(port).ToString());*/

                if (Event.current.type == EventType.Repaint && port.drawPort)
                {
                    portStyle.Draw(portRect, false, false, false, false);
                }
            }
        }

      
        private Rect GetPortRect(Port port)
        {
            Rect rect = GetNodeRect(port.node,this.m_GraphOffset);
            int index = port.node.Ports.IndexOf(port);
            if (port.direction == PortDirection.Input)
            {
                Vector2 inputPosition = new Vector2(rect.x - NODE_PORT_OFFSET, rect.y + NODE_LINE_HEIGHT * 0.5f - NODE_PORT_SIZE * 0.5f) + new Vector2(0f, GetHeaderRect(port.node, this.m_GraphOffset).height + NODE_LINE_HEIGHT * index);
                return new Rect(inputPosition.x, inputPosition.y, NODE_PORT_SIZE, NODE_PORT_SIZE);
            }
            else
            {
                NodeStyleAttribute nodeStyle = port.node.GetType().GetCustomAttribute<NodeStyleAttribute>();
                int height = (nodeStyle != null && !nodeStyle.displayHeader) ? NODE_LINE_HEIGHT : NODE_HEADER_HEIGHT;

                Vector2 outputPosition = new Vector2(rect.x + rect.width + NODE_PORT_OFFSET - NODE_PORT_SIZE, rect.y - NODE_PORT_SIZE * 0.5f) + new Vector2(0f, height * 0.5f);
                return new Rect(outputPosition.x, outputPosition.y, NODE_PORT_SIZE, NODE_PORT_SIZE);
            }

        }


        private Rect GetHeaderRect(FlowNode node, Vector2 offset) {
            NodeStyleAttribute nodeStyle = node.GetType().GetCustomAttribute<NodeStyleAttribute>();
            if (nodeStyle == null) { nodeStyle = new NodeStyleAttribute(true); }
            Texture2D icon = Resources.Load<Texture2D>(nodeStyle.iconPath);

            Vector2 size = Vector2.zero;

            if (nodeStyle.displayHeader)
            {
                size = EditorStyles.label.CalcSize(new GUIContent(ObjectNames.NicifyVariableName(node.name)));
                size.x += NODE_CONTENT_OFFSET * 2;
                if (icon != null) { size.x += icon.width + NODE_CONTENT_OFFSET; }
                size.y = NODE_HEADER_HEIGHT;
            }
            size.x = Mathf.Clamp(size.x, NODE_MIN_WIDTH, float.PositiveInfinity);

            return new Rect(node.position.x + offset.x, node.position.y + offset.y, size.x, size.y);
        }

        private float GetFieldWidth(FlowNode node) {
            float fieldWidth = 0f;
            foreach (Port port in node.InputPorts) {
                if (port.Connections.Count == 0) {
                    FieldInfo field = node.GetType().GetField(port.fieldName);

                    object value = field.GetValue(node);
                    float x = EditorGUIUtility.fieldWidth+20f;
                    if (UnityTools.IsNumeric(value) || value is string)
                    {
                        Vector2 temp = EditorStyles.textField.CalcSize(new GUIContent(value.ToString()));
                        x = Mathf.Clamp(temp.x, 15f, float.MaxValue); 
                    }
                   
                    if (x > fieldWidth)
                        fieldWidth = x;
                }
            }
            return fieldWidth;
        }

        private float GetLabelWidth(FlowNode node) {
            float width = 0;
            foreach (Port port in node.InputPorts) {
                if (port.Connections.Count > 0 || port.label)
                {
                    Vector2 size = Styles.nodeText.CalcSize(new GUIContent(ObjectNames.NicifyVariableName(port.fieldName)));
                    if (size.x > width)
                        width = size.x;
                }
            }
            return width;
        }

        private float GetContentWidth(FlowNode node) {
            float maxWidth = 0f;
            float labelWidth = GetLabelWidth(node);
            float fieldWidth = GetFieldWidth(node);
            foreach (Port port in node.InputPorts)
            {
                if (port.Connections.Count == 0 && maxWidth < fieldWidth)
                {
                    maxWidth = fieldWidth+(port.label? labelWidth+NODE_CONTENT_OFFSET:0f);
                }
            }

            foreach (Port port in node.InputPorts)
            {
                if (port.Connections.Count > 0 && maxWidth < labelWidth)
                {
                    maxWidth = labelWidth;
                }
            }
            return maxWidth;
        }

        protected override Rect GetNodeRect(FlowNode node, Vector2 offset)
        {
            NodeStyleAttribute nodeStyle = node.GetType().GetCustomAttribute<NodeStyleAttribute>();
            if (nodeStyle == null) { nodeStyle = new NodeStyleAttribute(true); }
            Texture2D icon = Resources.Load<Texture2D>(nodeStyle.iconPath);

            Vector2 size = Vector2.zero;
            Vector2 headerSize = Vector2.zero;

            if (nodeStyle.displayHeader)
            {
                headerSize = EditorStyles.label.CalcSize(new GUIContent(ObjectNames.NicifyVariableName(node.name)));
                headerSize.x += NODE_CONTENT_OFFSET * 2;
                if (headerSize.x > NODE_MIN_WIDTH)
                    headerSize.x += NODE_CONTENT_OFFSET;

                if (icon != null) { headerSize.x += icon.width+NODE_CONTENT_OFFSET; }
                headerSize.y = NODE_HEADER_HEIGHT;
            }

            size.x = Mathf.Clamp(headerSize.x, NODE_MIN_WIDTH, float.PositiveInfinity);
            float maxWidth = GetContentWidth(node);

            float width = maxWidth + NODE_CONTENT_OFFSET * 2 + (!nodeStyle.displayHeader && icon != null?icon.width+NODE_CONTENT_OFFSET:0f);
            if (width > size.x)
                size.x = width;
           
            size.y = Mathf.Clamp(headerSize.y + node.InputPorts.Count * NODE_LINE_HEIGHT, NODE_LINE_HEIGHT, float.MaxValue);
         
            return new Rect(node.position.x + offset.x, node.position.y + offset.y, size.x, size.y);
        }

        protected override void MoveNode(FlowNode node, Vector2 delta)
        {
            node.position += delta;
        }

        protected override void ExecuteCommand(string name)
        {
            switch (name)
            {
                case "Copy":
                    this.CopyNodes();
                    break;
                case "Paste":
                    this.PasteNodes(Vector2.zero);
                    break;
                case "Cut":
                    SaveSelection();
                    this.CutNodes();
                    break;
                case "Duplicate":
                    this.DuplicateNodes();
                    break;
                case "SoftDelete":
                    SaveSelection();
                    this.DeleteNodes();
                    break;
                case "CenterGraph":
                    this.CenterGraphView();
                    break;
            }
        }


        protected override void GraphContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Node"), false, delegate ()
            {
                Vector2 pos = (position + this.m_GraphOffset) * this.m_GraphZoom + this.m_GraphViewArea.position;
                this.m_CreateNodePosition = position;
                AddObjectWindow.ShowWindow<FlowNode>(new Rect(pos.x - this.m_GraphViewArea.x, pos.y, 230f, 0f), AddNode, CreateNodeScript);
               // AddNodeWindow.ShowWindow(new Rect(pos.x, pos.y, 230f, 21f), position, this.m_Graph);
            });

            if (!string.IsNullOrEmpty(this.m_Copy))
            {
                menu.AddItem(new GUIContent("Paste Nodes"), false, delegate {
                    this.PasteNodes(position);
                });

            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste Nodes"));
            }
            menu.ShowAsContext();
        }

        protected override void NodeContextMenu(FlowNode node, Vector2 position)
        {
            if (typeof(EventNode).IsAssignableFrom(node.GetType()))
            {
            //    return;
            }
            GenericMenu menu = new GenericMenu();
         //   this.m_Selection.RemoveAll(x => x.GetType() == typeof(EventNode));
            string s = (this.m_Selection.Count > 1 ? "s" : "");
            menu.AddItem(new GUIContent("Copy Node" + s), false, new GenericMenu.MenuFunction(this.CopyNodes));
            if (!string.IsNullOrEmpty(this.m_Copy))
            {
                menu.AddItem(new GUIContent("Paste Node" + s), false, delegate () {
                    this.PasteNodes(position);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste Node" + s));
            }
            menu.AddItem(new GUIContent("Cut Node" + s), false, delegate {
                SaveSelection();
                this.CutNodes();
            });
            menu.AddItem(new GUIContent("Delete Node" + s), false, delegate {
                SaveSelection();
                this.DeleteNodes();
            });
            menu.ShowAsContext();
        }

        private void AddNode(Type type)
        {
            Node node = GraphUtility.AddNode(this.m_Graph, type);
            node.position = this.m_CreateNodePosition;
        }

        private void CreateNodeScript(string scriptName) { Debug.LogWarning("This is not implemented yet!"); }

        private void CopyNodes()
        {
            Graph copy = new Graph();
            copy.serializationData = this.m_Graph.serializationData;
            GraphUtility.Load(copy);
           
            Node[] toDelete = copy.nodes.Where(x => !this.m_Selection.Exists(y => x.id == y.id)).ToArray();
            GraphUtility.RemoveNodes(copy, toDelete.Cast<FlowNode>().ToArray());
            GraphUtility.Save(copy);
            for (int i = 0; i < copy.nodes.Count; i++)
            {
                copy.serializationData = copy.serializationData.Replace(copy.nodes[i].id, System.Guid.NewGuid().ToString());
            }
            this.m_Copy = copy.serializationData;
        }

        private void DeleteNodes()
        {
            GraphUtility.RemoveNodes(this.m_Graph, this.m_Selection.ToArray());
            this.m_Selection.Clear();
            GraphUtility.Save(this.m_Graph);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this.m_Target);
        }

        private void CutNodes()
        {
            CopyNodes();
            DeleteNodes();
        }

        private void DuplicateNodes()
        {
            CopyNodes();
            PasteNodes(Vector2.zero);
        }

        private void PasteNodes(Vector2 position)
        {
            if (!string.IsNullOrEmpty(this.m_Copy))
            {
                this.m_Selection.Clear();
                Graph graph = new Graph();
                graph.serializationData = this.m_Copy;
                GraphUtility.Load(graph);

                for (int i = 0; i < graph.nodes.Count; i++)
                {
                    Node node = graph.nodes[i];
                    if (position == Vector2.zero)
                    {
                        position = node.position + new Vector2(20, 15);
                    }
                    node.position += position - node.position;
                    this.m_Graph.nodes.Add(node);
                    this.m_Selection.Add((FlowNode)node);
                }
                GraphUtility.Save(this.m_Graph);
                PrefabUtility.RecordPrefabInstancePropertyModifications(this.m_Target);
            }
        }

        /// <summary>
        /// GraphView styles
        /// </summary>
        private static class Styles
        {
            private static GUISkin skin;
            public static GUIStyle nodeNormal;
            public static GUIStyle nodeActive;
            public static GUIStyle nodeSelected;
            public static GUIStyle port;
            public static GUIStyle portConnected;
            public static GUIStyle seperator;
            public static GUIStyle nodeHeaderText;
            public static GUIStyle nodeText;

            static Styles()
            {
                skin = Resources.Load<GUISkin>("FormulaSkin");
                nodeNormal = skin.GetStyle("NodeNormal");
                nodeActive = skin.GetStyle("NodeActive");
                nodeSelected = skin.GetStyle("NodeSelected");
                port = skin.GetStyle("Port");
                portConnected = skin.GetStyle("PortConnected");
                seperator = skin.GetStyle("Seperator");
                nodeText = skin.GetStyle("NodeText");
                nodeHeaderText = skin.GetStyle("NodeHeaderText");
            }
        }
    }
}