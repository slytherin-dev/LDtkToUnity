﻿using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor
{
    public class LDtkReferenceDrawerIntGridLayer : LDtkReferenceDrawer<LayerDefinition>
    {
        protected override void DrawInternal(Rect controlRect, LayerDefinition data)
        {
            DrawLabel(controlRect, data.Identifier);

        }

        
    }
}