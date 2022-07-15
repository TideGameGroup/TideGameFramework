using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tide.Core;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class FPropertiesParser
    {
        private ACanvasComponent component;
        private EditorDynamicCanvasComponent dynamicComponent;

        public FPropertiesParser(ACanvasComponent component, EditorDynamicCanvasComponent dynamicComponent)
        {
            this.component = component;
            this.dynamicComponent = dynamicComponent;
        }

        public static List<string> str_errors = new List<string>
        {
            "invalid value",
            "value cannot be null or whitespace",
            "duplicate IDs are not allowed",
            "invalid ID or index for parent",
            "color conversion error",
            "???",
            "int conversion error",
            "not a widget type",
        };

        public bool IsInvalid(string str, out string error)
        {
            error = "";

            if (str_errors.Contains(str))
            {
                error = str_errors[0];
                return true;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                error = str_errors[1];
                return true;
            }

            return false;
        }

        public bool IsInRange(int index, bool allowminus1 = false)
        {
            return index >= (allowminus1 ? -1 : 0) && index < dynamicComponent.DynamicCanvas.Count;
        }

        public bool IsValidID(string ID, out int index)
        {
            for (int i = 0; i < dynamicComponent.DynamicCanvas.Count; i++)
            {
                if (dynamicComponent.DynamicCanvas.IDs[i] == ID)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public bool GetFieldValue(string field, out string field_value)
        {
            field_value = component.cache.canvas.texts[component.graph.widgetNameIndexMap[field]];

            if (IsInvalid(field_value, out string err))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[field]] = err;
                return false;
            }
            return true;
        }

        public bool IsValidColorString(string str)
        {
            return byte.TryParse(str, out _);
        }

        public void HandleColorField(string fieldPrefix, ref List<Color> colorlist)
        {
            if (!GetFieldValue(fieldPrefix + "_R_field", out string R) || !IsValidColorString(R))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_R_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_G_field", out string G) || !IsValidColorString(G))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_G_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_B_field", out string B) || !IsValidColorString(B))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_B_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_A_field", out string A) || !IsValidColorString(A))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_A_field"]] = str_errors[5];
                return;
            }

            if (FStaticTypeStringConversions.StringArrayToColor(new string[] { R, G, B, A }, out Color color))
            {
                colorlist[dynamicComponent.selection] = color;
                return;
            }
            else
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_R_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_G_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_B_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_A_field"]] = str_errors[5];
            }
        }

        public bool IsValidIntValue(string str)
        {
            return int.TryParse(str, out _);
        }

        public void HandleRectangleField(string fieldPrefix, ref List<Rectangle> rectlist)
        {
            if (!GetFieldValue(fieldPrefix + "_X_field", out string X) || !IsValidIntValue(X))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_X_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_Y_field", out string Y) || !IsValidIntValue(Y))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_Y_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_W_field", out string W) || !IsValidIntValue(W))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_W_field"]] = str_errors[5];
                return;
            }

            if (!GetFieldValue(fieldPrefix + "_H_field", out string H) || !IsValidIntValue(H))
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_H_field"]] = str_errors[5];
                return;
            }

            if (FStaticTypeStringConversions.StringArrayToRectangle(new string[] { X, Y, W, H }, out Rectangle rect))
            {
                rectlist[dynamicComponent.selection] = rect;
                return;
            }
            else
            {
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_X_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_Y_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_W_field"]] = str_errors[5];
                component.cache.canvas.texts[component.graph.widgetNameIndexMap[fieldPrefix + "_H_field"]] = str_errors[5];
            }
        }
    }
}
