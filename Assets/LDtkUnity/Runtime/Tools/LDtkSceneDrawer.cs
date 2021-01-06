﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LDtkUnity.Data;
using UnityEngine;

namespace LDtkUnity.Tools
{
    [AddComponentMenu("")]
    public class LDtkSceneDrawer : MonoBehaviour
    {
        private Component _source;
        private FieldInfo _field;
        private LDtkDefinitionFieldDisplayMode _mode;
        private Color _gizmoColor;
        private float _gridSize;

        public void SetReference(Component source, FieldInfo field, LDtkDataEntity entityData, LDtkDefinitionFieldDisplayMode mode, int gridSize)
        {
            if (!Application.isEditor)
            {
                return;
            }

            _source = source;
            _field = field;
            _mode = mode;
            _gridSize = gridSize;

            Color gizmoColor = entityData.Definition().Color();
            gizmoColor.a = 0.33f;
            const float incrementDifference = -0.1f;
            gizmoColor.r += incrementDifference;
            gizmoColor.g += incrementDifference;
            gizmoColor.b += incrementDifference;
            _gizmoColor = gizmoColor;
        }
        
        private Vector2[] GetPoints()
        {
            if (_field == null)
            {
                return null;
            }
            
            if (_field.FieldType.IsArray)
            {
                return (Vector2[]) _field.GetValue(_source);
            }

            Vector2 point = (Vector2)_field.GetValue(_source);
            return new[] { point };
        }

        private float GetRadius()
        {
            if (_field == null)
            {
                return default;
            }
            
            if (_field.FieldType == typeof(float))
            {
                return (float) _field.GetValue(_source);
            }
            if (_field.FieldType == typeof(int))
            {
                return (int) _field.GetValue(_source);
            }
            return default;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;

            switch (_mode)
            {
                case LDtkDefinitionFieldDisplayMode.Hidden:
                case LDtkDefinitionFieldDisplayMode.ValueOnly:
                case LDtkDefinitionFieldDisplayMode.NameAndValue:
                case LDtkDefinitionFieldDisplayMode.EntityTile:
                    //nothing
                    break;
                
                case LDtkDefinitionFieldDisplayMode.PointPath:
                case LDtkDefinitionFieldDisplayMode.PointStar:
                    DrawPoints();
                    break;
                
                case LDtkDefinitionFieldDisplayMode.RadiusPx:
                case LDtkDefinitionFieldDisplayMode.RadiusGrid:
                    DrawRadius();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawRadius()
        {
            switch (_mode)
            {
                case LDtkDefinitionFieldDisplayMode.RadiusPx:
                    DrawRadiusInternal(_gridSize);
                    break;

                case LDtkDefinitionFieldDisplayMode.RadiusGrid:
                    DrawRadiusInternal(1);
                    break;
            }
        }

        private void DrawRadiusInternal(float pixelsPerUnit)
        {
            float radius = GetRadius() / pixelsPerUnit; 
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        
        private void DrawPoints()
        {
            Vector2[] points = GetPoints();
            if (points.NullOrEmpty())
            {
                return;
            }

            List<Vector2> convertedRoute = Array.ConvertAll(points, input => new Vector2(input.x, input.y)).ToList();

            //round the starting position to the bottom left of the current tile
            Vector2 pos = transform.position;
            pos += (Vector2.one * 0.001f);
            
            int left = Mathf.FloorToInt(pos.x);
            int right = Mathf.CeilToInt(pos.x);
            pos.x = Mathf.Lerp(left, right, 0.5f);
            
            int down = Mathf.FloorToInt(pos.y);
            int up = Mathf.CeilToInt(pos.y);
            pos.y = Mathf.Lerp(down, up, 0.5f);
            
            convertedRoute.Insert(0, pos);

            switch (_mode)
            {
                case LDtkDefinitionFieldDisplayMode.PointPath:
                    DrawPath(convertedRoute);
                    break;
                case LDtkDefinitionFieldDisplayMode.PointStar:
                    DrawStar(convertedRoute);
                    break;
            }
        }
        
        private void DrawPath(List<Vector2> convertedRoute)
        {
            for (int i = 0; i < convertedRoute.Count - 1; i++)
            {
                Vector2 pointPos = convertedRoute[i];
                Vector2 nextPointPos = convertedRoute[i + 1];
                Gizmos.DrawLine(pointPos, nextPointPos);
            }
        }
        private void DrawStar(List<Vector2> convertedRoute)
        {
            Vector2 pointPos = convertedRoute[0];

            for (int i = 1; i < convertedRoute.Count; i++)
            {
                Vector2 nextPointPos = convertedRoute[i];
                Gizmos.DrawLine(pointPos, nextPointPos);
            }
        }
    }
}