using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DevionGames.UIWidgets
{
	[CustomEditor (typeof(UIWidget), true)]
	public class UIWidgetInspector: CallbackHandlerInspector
	{

        protected CanvasGroup canvasGroup;

        protected override void OnEnable ()
		{
            base.OnEnable();
            this.canvasGroup = (target as UIWidget).GetComponent<CanvasGroup>();
        }

        protected virtual void OnSceneGUI()
        {
            if (canvasGroup == null)
            {
                return;
            }
            Handles.BeginGUI();
            Rect rect = Camera.current.pixelRect;
           
            if (GUI.Button(new Rect(rect.width - 110f, rect.height - 30f, 100f, 20f), canvasGroup.alpha > 0.1f ? "Hide" : "Show"))
            {

                if (canvasGroup.alpha > 0.1f)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
                else
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                EditorUtility.SetDirty(canvasGroup);
            }

            Handles.EndGUI();
        }
    }
}