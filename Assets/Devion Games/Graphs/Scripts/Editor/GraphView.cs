using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    public abstract class GraphView<T> : IGraphView, ISerializationCallbackReceiver
    {
        protected EditorWindow m_Host;
        protected Rect m_GraphViewArea;
        protected Vector2 m_GraphOffset;
        protected float m_GraphZoom = 1f;
        private bool m_CenterGraph;
        private bool m_DragStarted;
        private bool m_DragGraph;
        private bool m_DragNodes;
        private SelectionMode m_SelectionMode;
        private Vector2 m_SelectionStartPosition;
        protected List<T> m_Selection = new List<T>();
        private List<int> m_PrevSelection;

        protected abstract T[] inspectedNodes { get; }

        public GraphView(EditorWindow host)
        {
            this.m_Host = host;
        }

        public void OnGUI(Rect position)
        {
            this.m_GraphViewArea = position;
            this.DrawBackground();
            this.DrawGrid();
            ZoomableArea.Begin(this.m_GraphViewArea, this.m_Host.IsDocked() ? 0f : 3f, this.m_GraphZoom);
            this.DrawNodeConnections(inspectedNodes, this.m_GraphOffset);
            this.DrawNodes(inspectedNodes.Where(x => !this.m_Selection.Contains(x)).ToArray(), this.m_GraphOffset);
            this.DrawNodes(this.m_Selection.ToArray(), this.m_GraphOffset);
            this.ConnectNodes(inspectedNodes, this.m_GraphOffset);
            ZoomableArea.End();
            
            GUILayout.BeginArea(this.m_GraphViewArea);
            this.OnGUI();
            this.SelectInGraph();
            this.ContextMenuClick();
            this.DragNodes();
            this.ZoomToGraph();
            this.DragGraph();
            this.ExecuteCommands();
            GUILayout.EndArea();

            if (this.m_CenterGraph){
                CenterGraph();
            }
        }

        protected abstract void DrawNodeConnections(T[] nodes, Vector2 offset);
        protected abstract void DrawNode(Rect rect, T node, bool selected);
        protected abstract void ConnectNodes(T[] nodes, Vector2 offset);
        protected abstract Rect GetNodeRect(T node, Vector2 offset);
        protected abstract void MoveNode(T node, Vector2 delta);
        protected virtual void GraphContextMenu(Vector2 position) { }
        protected virtual void NodeContextMenu(T node, Vector2 position) { }
        protected virtual void ExecuteCommand(string name){}
        protected virtual void OnGUI(){}

        public virtual void OnBeforeSerialize()
        {
            SaveSelection();
        }

        public virtual void OnAfterDeserialize()
        {
            LoadSelection();
        }

        public void SaveSelection()
        {
            this.m_PrevSelection = inspectedNodes.Where(x => this.m_Selection.Contains(x)).Select(y => inspectedNodes.ToList().IndexOf(y)).ToList();
        }

        public void LoadSelection()
        {
            this.m_Selection = inspectedNodes.Where(x => this.m_PrevSelection.Contains(inspectedNodes.ToList().IndexOf(x))).ToList();
        }
      
        public void CenterGraphView()
        {
            this.m_CenterGraph = true;
        }

        private void CenterGraph()
        {
            Vector2 center = Vector2.zero;
            if (inspectedNodes.Length > 0)
            {
                for (int i = 0; i < inspectedNodes.Length; i++)
                {
                    T node = inspectedNodes[i];
                    center += GetNodeRect(node, Vector2.zero).center;
                }
                center /= inspectedNodes.Length;

            }
            this.m_GraphOffset = -center + (this.m_GraphViewArea.size * 0.5f) / this.m_GraphZoom;
            this.m_Host.Repaint();
            this.m_CenterGraph = false;
        }

        private void DrawNodes(T[] nodes, Vector2 offset)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                T node = nodes[i];
                Rect rect = GetNodeRect(node, offset);
                this.DrawNode(rect, node, this.m_Selection.Contains(node));
            }
        }

        private void ContextMenuClick()
        {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ContextClick)
            {
                Vector2 mousePosition = currentEvent.mousePosition / this.m_GraphZoom - this.m_GraphOffset;

                T node = GetNodeAtPosition(mousePosition);
                if (node != null)
                {
                    this.NodeContextMenu(node, mousePosition);
                }
                else
                {
                    this.GraphContextMenu(mousePosition);
                }
            }
        }

        protected T MouseOverNode()
        {
            return GetNodeAtPosition(Event.current.mousePosition / this.m_GraphZoom - this.m_GraphOffset) ;
        }

        protected T GetNodeAtPosition(Vector2 position)
        {
            for (int i = 0; i < this.m_Selection.Count; i++)
            {
                T node = this.m_Selection[i];
                Rect rect = GetNodeRect(node, Vector2.zero);
                if (rect.Contains(position))
                {
                    return node;
                }
            }

            for (int i = 0; i < inspectedNodes.Length; i++)
            {
                T node = inspectedNodes[i];
                Rect rect = GetNodeRect(node, Vector2.zero);
                if (rect.Contains(position))
                {
                    return node;
                }
            }
            return default(T);
        }

        private void ExecuteCommands()
        {
            Event currentEvent = Event.current;
            string[] commands = new string[] {
                "Copy",
                "Paste",
                "Cut",
                "Duplicate",
                "SoftDelete",
                "SelectAll",
                "DeselectAll",
                "CenterGraph"
            };
            if (currentEvent.type == EventType.ValidateCommand && commands.Contains(currentEvent.commandName))
            {
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.ExecuteCommand)
            {
                ExecuteCommand(currentEvent.commandName);
            }
        }

        private void ZoomToGraph()
        {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ScrollWheel)
            {
                GUI.FocusControl(string.Empty);
                Vector2 mousePosition = currentEvent.mousePosition / this.m_GraphZoom;
                float delta = -currentEvent.delta.y / 100f;
                this.m_GraphZoom = this.m_GraphZoom + delta;
                this.m_GraphZoom = Mathf.Clamp(this.m_GraphZoom, 0.4f, 1f);
                mousePosition = currentEvent.mousePosition / this.m_GraphZoom - mousePosition;
                this.m_GraphOffset = this.m_GraphOffset + mousePosition;
                currentEvent.Use();
            }
        }

        private void DragNodes()
        {
            Event currentEvent = Event.current;
            switch (currentEvent.rawType)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown)
                    {
                        this.m_DragNodes = true;
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (this.m_DragNodes)
                    {
                        this.m_DragNodes = false;
                        if (this.m_DragStarted)
                        {
                            this.m_DragStarted = false;
                            this.m_Host.SendEvent(EditorGUIUtility.CommandEvent("OnEndDrag"));
                        }
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (this.m_DragNodes)
                    {
                        if (!m_DragStarted)
                        {
                            this.m_Host.SendEvent(EditorGUIUtility.CommandEvent("OnStartDrag"));
                            this.m_DragStarted = true;
                        }
                        for (int i = 0; i < m_Selection.Count; i++)
                        {
                            T node = m_Selection[i];
                            MoveNode(node, currentEvent.delta / this.m_GraphZoom);
                        }
                        currentEvent.Use();
                    }
                    break;
                case EventType.Repaint:
                    if (this.m_DragNodes)
                    {
                        AutoScrollNodes(2.5f);
                    }
                    break;
            }
        }

        private void DragGraph()
        {
            Event currentEvent = Event.current;
            switch (currentEvent.rawType)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 2 && currentEvent.type == EventType.MouseDown)
                    {
                        this.m_DragGraph = true;
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (this.m_DragGraph)
                    {
                        this.m_DragGraph = false;
                        if (this.m_DragStarted)
                        {
                            this.m_DragStarted = false;
                            this.m_Host.SendEvent(EditorGUIUtility.CommandEvent("OnEndDrag"));
                        }

                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (this.m_DragGraph)
                    {

                        this.m_GraphOffset = this.m_GraphOffset + (currentEvent.delta / this.m_GraphZoom);
                        if (!m_DragStarted)
                        {
                            this.m_Host.SendEvent(EditorGUIUtility.CommandEvent("OnStartDrag"));
                            this.m_DragStarted = true;
                        }
                        currentEvent.Use();

                    }
                    break;
            }
        }


        protected virtual void AutoScrollGraph(float speed, float offset = 15f)
        {
            Vector2 delta = Vector2.zero;
            Vector2 mousePosition = Event.current.mousePosition;
            if (mousePosition.y < offset)
            {
                delta.y += speed;
            }
            else if (mousePosition.y > this.m_GraphViewArea.height - offset)
            {
                delta.y -= speed;
            }

            if (mousePosition.x < offset)
            {
                delta.x += speed;
            }
            else if (mousePosition.x > this.m_GraphViewArea.width - offset)
            {
                delta.x -= speed;
            }

            delta = Vector2.ClampMagnitude(delta, 1.5f);
            this.m_GraphOffset = this.m_GraphOffset + (delta / this.m_GraphZoom);
            this.m_SelectionStartPosition += (delta / this.m_GraphZoom);   
            this.m_Host.Repaint();  
        }

        private void AutoScrollNodes(float speed)
        {
            Vector2 delta = Vector2.zero;
            Vector2 mousePosition = Event.current.mousePosition;
            if (mousePosition.y < 15f)
            {
                delta.y += speed;
            }
            else if (mousePosition.y > this.m_GraphViewArea.height - 15f)
            {
                delta.y -= speed;
            }

            if (mousePosition.x < 15f)
            {
                delta.x += speed;
            }
            else if (mousePosition.x > this.m_GraphViewArea.width - 15f)
            {
                delta.x -= speed;
            }
            this.m_GraphOffset = this.m_GraphOffset + (delta / this.m_GraphZoom);
            for (int i = 0; i < this.m_Selection.Count; i++)
            {
                T node = this.m_Selection[i];
                MoveNode(node, -(delta / this.m_GraphZoom));
            }
            this.m_Host.Repaint();  
        }

        private bool Select(T node)
        {
            if (node != null)
            {
                if (EditorGUI.actionKey || Event.current.shift)
                {
                    if (!m_Selection.Contains(node))
                    {
                        m_Selection.Add(node);
                        OnSelectNode(node);
                    }
                    else
                    {
                        m_Selection.Remove(node);
                    }
                }
                else if (!m_Selection.Contains(node))
                {
                    m_Selection.Clear();
                    m_Selection.Add(node);
                    OnSelectNode(node);
                }
                return true;
            }
            if (!EditorGUI.actionKey && !Event.current.shift)
            {
                m_Selection.Clear();
             
            }
            return false;
        }

        private void Select(Rect rect)
        {
            for (int i = 0; i < inspectedNodes.Length; i++)
            {
                T node = inspectedNodes[i];
                Rect position = GetNodeRect(node, Vector2.zero);
                if (position.xMax < rect.x || position.x > rect.xMax || position.yMax < rect.y || position.y > rect.yMax)
                {
                    m_Selection.Remove(node);
                   
                    continue;
                }
                if (!m_Selection.Contains(node))
                {
                    m_Selection.Add(node);
                    OnSelectNode(node);
                }
            }
        }

        private void SelectInGraph()
        {
            Event currentEvent = Event.current;
            switch (currentEvent.rawType)
            {
                case EventType.MouseDown:
                    if (currentEvent.type == EventType.MouseDown)
                    {

                        T node = GetNodeAtPosition(currentEvent.mousePosition / this.m_GraphZoom - this.m_GraphOffset);
                        if (node != null && Event.current.button == 1 && !this.m_Selection.Contains(node))
                        {

                            this.m_Selection.Clear();
                            this.m_Selection.Add(node);
                            OnSelectNode(node);
                            Event.current.Use();
                            return;
                        }
                        if (currentEvent.button == 0)
                        {
                            this.m_SelectionStartPosition = currentEvent.mousePosition;
                            if (Select(node))
                            {
                                GUIUtility.keyboardControl = 0;
                                return;
                            }
                            this.m_SelectionMode = SelectionMode.Pick;
                            currentEvent.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (this.m_SelectionMode == SelectionMode.Pick || this.m_SelectionMode == SelectionMode.Rect)
                    {
                        this.m_SelectionMode = SelectionMode.None;
                        currentEvent.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (this.m_SelectionMode == SelectionMode.Pick || this.m_SelectionMode == SelectionMode.Rect)
                    {
                        Rect rect = new Rect(10f,this.m_GraphViewArea.y+10f,this.m_GraphViewArea.width-20f,this.m_GraphViewArea.height-20f);
                        if (rect.Contains(currentEvent.mousePosition))
                        {
                            this.m_SelectionMode = SelectionMode.Rect;
                            Select(this.m_SelectionStartPosition / this.m_GraphZoom - this.m_GraphOffset, currentEvent.mousePosition / this.m_GraphZoom - this.m_GraphOffset);
                            currentEvent.Use();
                        }
                        else {
                            this.m_SelectionMode = SelectionMode.None;
                        }
                    }
                    break;
                case EventType.Repaint:
                    if (m_SelectionMode == SelectionMode.Rect)
                    {
                        DrawSelectionRect(this.m_SelectionStartPosition, currentEvent.mousePosition);
                        AutoScrollGraph(2.5f);
                    }

                    break;
            }
        }

        protected virtual void OnSelectNode(T node) { }

        private void Select(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x = rect.x + rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y = rect.y + rect.height;
                rect.height = -rect.height;
            }
            Select(rect);
        }

        private void DrawSelectionRect(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Styles.selectionRect.Draw(rect, false, false, false, false);
            }
        }

        private void DrawSelectionRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x = rect.x + rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y = rect.y + rect.height;
                rect.height = -rect.height;
            }
            DrawSelectionRect(rect);
        }

        public enum SelectionMode
        {
            None,
            Pick,
            Rect,
        }

        protected virtual void DrawConnection(Vector2 start, Vector2 end, ConnectionStyle style, Color color)
        {
            Vector3[] points;
            Vector3[] tangents;

            switch (style)
            {
                case ConnectionStyle.Angular:
                    GetAngularConnectorValues(start, end, out points, out tangents);
                    DrawRoundedPolyLine(points, tangents, null, color);
                    break;
                case ConnectionStyle.Curvy:
                    GetCurvyConnectorValues(start, end, out points, out tangents);
                    Handles.DrawBezier(points[0], points[1], tangents[0], tangents[1], color, null, 3f);
                    break;
                case ConnectionStyle.Line:
                    Handles.color = color;
                    float offset = 15f;
                    Handles.DrawAAPolyLine(4f, start, start - Vector2.left * offset);
                    Handles.DrawAAPolyLine(4f, start - Vector2.left * offset, end + Vector2.left * offset);
                    Handles.DrawAAPolyLine(4f, end + Vector2.left * offset, end);
                    break;
            }
        }

        private void DrawRoundedPolyLine(Vector3[] points, Vector3[] tangets, Texture2D tex, Color color)
        {
            Handles.color = color;
            for (int i = 0; i < (int)points.Length; i = i + 2)
            {
                Handles.DrawAAPolyLine(tex, 3.5f, new Vector3[] { points[i], points[i + 1] });
            }
            for (int j = 0; j < (int)tangets.Length; j = j + 2)
            {
                Handles.DrawBezier(points[j + 1], points[j + 2], tangets[j], tangets[j + 1], color, tex, 3.5f);
            }
        }

        private void GetAngularConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
        {
            Vector2 vector2 = start - end;
            Vector2 vector21 = (vector2 / 2f) + end;
            Vector2 vector22 = new Vector2(Mathf.Sign(vector2.x), Mathf.Sign(vector2.y));
            Vector2 vector23 = new Vector2(Mathf.Min(Mathf.Abs(vector2.x / 2f), 5f), Mathf.Min(Mathf.Abs(vector2.y / 2f), 5f));
            points = new Vector3[] {
                start,
                new Vector3 (start.x, vector21.y + vector23.y * vector22.y),
                new Vector3 (start.x - vector23.x * vector22.x, vector21.y),
                new Vector3 (end.x + vector23.x * vector22.x, vector21.y),
                new Vector3 (end.x, vector21.y - vector23.y * vector22.y),
                end
            };
            Vector3[] vector3Array = new Vector3[4];
            Vector3 vector3 = points[1] - points[0];
            vector3Array[0] = ((vector3.normalized * vector23.x) * 0.6f) + points[1];
            Vector3 vector31 = points[2] - points[3];
            vector3Array[1] = ((vector31.normalized * vector23.y) * 0.6f) + points[2];
            Vector3 vector32 = points[3] - points[2];
            vector3Array[2] = ((vector32.normalized * vector23.y) * 0.6f) + points[3];
            Vector3 vector33 = points[4] - points[5];
            vector3Array[3] = ((vector33.normalized * vector23.x) * 0.6f) + points[4];
            tangents = vector3Array;
        }

        private void GetCurvyConnectorValues(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
        {
            points = new Vector3[] { start, end };
            tangents = new Vector3[2];
            float single = (start.x >= end.x ? 0.7f : 0.3f);
            single = 0.5f;
            float single1 = 1f - single;
            float single2 = 0f;
            if (start.y > end.y)
            {
                float single3 = -0.25f;
                single = single3;
                single1 = single3;
                float single4 = (start.x - end.x) / (start.y - end.y);
                if (Mathf.Abs(single4) > 0.5f)
                {
                    float single5 = (Mathf.Abs(single4) - 0.5f) / 8f;
                    single5 = Mathf.Sqrt(single5);
                    single2 = Mathf.Min(single5 * 80f, 80f);
                    if (start.x > end.x)
                    {
                        single2 = -single2;
                    }
                }
            }
            Vector2 vector2 = start - end;
            float single6 = Mathf.Clamp01((vector2.magnitude - 10f) / 50f);
            tangents[0] = start + (new Vector2(single2, (end.y - start.y) * single + 30f) * single6);
            tangents[1] = end + (new Vector2(-single2, (end.y - start.y) * -single1 - 30f) * single6);
        }

        /// <summary>
        /// Draw grid in graph view area with offset and zoom
        /// </summary>
        private void DrawGrid()
        {
            if (Event.current.type == EventType.Repaint)
            {
                GL.PushMatrix();
                GL.Begin(1);
                GL.Color(Styles.gridMinorColor);
                this.DrawGridLines(this.m_GraphViewArea, Styles.gridMinorSize * this.m_GraphZoom, new Vector2(this.m_GraphOffset.x % Styles.gridMinorSize * this.m_GraphZoom, this.m_GraphOffset.y % Styles.gridMinorSize * this.m_GraphZoom));
                GL.Color(Styles.gridMajorColor);
                this.DrawGridLines(this.m_GraphViewArea, Styles.gridMajorSize * this.m_GraphZoom, new Vector2(this.m_GraphOffset.x % Styles.gridMajorSize * this.m_GraphZoom, this.m_GraphOffset.y % Styles.gridMajorSize * this.m_GraphZoom));
                GL.End();
                GL.PopMatrix();
            }
        }

        /// <summary>
        /// Draw grid lines in area
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="offset"></param>
        private void DrawGridLines(Rect area, float gridSize, Vector2 offset)
        {
            float x = area.x + offset.x;
            if (offset.x < 0f)
            {
                x = x + gridSize;
            }
            for (float i = x; i < area.x + area.width; i = i + gridSize)
            {
                this.DrawLine(new Vector2(i, area.y), new Vector2(i, area.y + area.height));
            }
            float y = area.y + offset.y;
            if (offset.y < 0f)
            {
                y = y + gridSize;
            }
            for (float j = y; j < area.y + area.height; j = j + gridSize)
            {
                this.DrawLine(new Vector2(area.x, j), new Vector2(area.x + area.width, j));
            }
        }

        /// <summary>
        /// Draw a line between p1 and p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        /// <summary>
        /// Draws the graph view background
        /// </summary>
        private void DrawBackground()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Styles.background.Draw(this.m_GraphViewArea, false, false, false, false);
            }
        }

        /// <summary>
        /// GraphView styles
        /// </summary>
        private static class Styles
        {
            public const float gridMinorSize = 12f;
            public const float gridMajorSize = 120f;
            public static Color gridMinorColor;
            public static Color gridMajorColor;

            public static GUIStyle background;
            public static GUIStyle selectionRect;


            static Styles()
            {
                gridMinorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.18f) : new Color(0f, 0f, 0f, 0.1f);
                gridMajorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.28f) : new Color(0f, 0f, 0f, 0.15f);

                background = new GUIStyle("flow background");
                selectionRect = new GUIStyle("SelectionRect");
            }
        }
    }
}