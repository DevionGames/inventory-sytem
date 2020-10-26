using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class ZoomableArea
    {
        private static Matrix4x4 m_PrevGuiMatrix;
        private static float m_TabHeight;

        public static Rect Begin(Rect area, float tabHeight, float zoom)
        {
            GUI.EndGroup();
            Rect rect = ScaleSizeBy(area, 1f / zoom, new Vector2(area.xMin, area.yMin));
            ZoomableArea.m_TabHeight = tabHeight;
            rect.y = rect.y + 18f + tabHeight;
            GUI.BeginGroup(rect);
            ZoomableArea.m_PrevGuiMatrix = GUI.matrix;
            Matrix4x4 matrix4x4 = Matrix4x4.TRS(new Vector2(rect.xMin, rect.yMin), Quaternion.identity, Vector3.one);
            Vector3 vector3 = Vector3.one;
            float single = zoom;
            float single1 = single;
            vector3.y = single;
            vector3.x = single1;
            Matrix4x4 matrix4x41 = Matrix4x4.Scale(vector3);
            GUI.matrix = ((matrix4x4 * matrix4x41) * matrix4x4.inverse) * GUI.matrix;
            return rect;
        }

        public static void End()
        {
            GUI.matrix = ZoomableArea.m_PrevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0f, 18f + ZoomableArea.m_TabHeight, (float)Screen.width, (float)Screen.height));
        }

        private static Rect ScaleSizeBy(Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect rect1 = rect;
            rect1.x = rect1.x - pivotPoint.x;
            rect1.y = rect1.y - pivotPoint.y;
            rect1.xMin = rect1.xMin * scale;
            rect1.xMax = rect1.xMax * scale;
            rect1.yMin = rect1.yMin * scale;
            rect1.yMax = rect1.yMax * scale;
            rect1.x = rect1.x + pivotPoint.x;
            rect1.y = rect1.y + pivotPoint.y;
            return rect1;
        }
    }
}