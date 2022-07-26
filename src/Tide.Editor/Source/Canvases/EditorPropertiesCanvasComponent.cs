using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct EditorPropertiesCanvasComponentConstructorArgs
    {
        public UContentManager content;
        public DynamicCanvasComponent dynamicCanvasComponent;
        public TInput input;
        public GameWindow window;
    }

    public class EditorPropertiesCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly TInput input;
        private readonly GameWindow window;
        private ITreeCanvasFactory factory = null;
        public DynamicCanvasComponent dynamicCanvasComponent;

        public EditorPropertiesCanvasComponent(EditorPropertiesCanvasComponentConstructorArgs args)
        {
            TrySetDefault(args.content, out content);
            TrySetDefault(args.input, out input);
            TrySetDefault(args.window, out window);
            TrySetDefault(args.dynamicCanvasComponent, out dynamicCanvasComponent);

            //dynamicCanvasComponent.OnDynamicCanvasUpdated += () => { CanvasComponent.cache.canvas = dynamicCanvasComponent.DynamicCanvas.AsCanvas(); };
            dynamicCanvasComponent.OnDynamicCanvasSet += () => { RebuildCanvas(); };
            dynamicCanvasComponent.OnSelectionUpdated += () => { RebuildCanvas(); };
        }

        public ACanvasComponent CanvasComponent { get; private set; }
        public ACanvasDrawComponent DrawComponent { get; private set; }

        private void RebuildCanvasComponents(FCanvas canvas)
        {
            RemoveChildComponent(CanvasComponent);
            RemoveChildComponent(DrawComponent);

            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = canvas,
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = input,
                    scale = 1f,
                    window = window
                };

            CanvasComponent = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = CanvasComponent,
                    content = content
                };

            DrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(CanvasComponent);
            AddChildComponent(DrawComponent);

            SetupBindings(CanvasComponent);
        }

        private void SetupBindings(ACanvasComponent canvas)
        {
            FPropertiesParser parser = new FPropertiesParser(CanvasComponent, dynamicCanvasComponent);

            CanvasComponent.OnWidgetUnFocused += (i) => {
                dynamicCanvasComponent.Rebuild();
            };

            canvas.BindAction("ID_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("ID_field", out string field_value))
                {
                    if (parser.IsValidID(field_value, out int index))
                    {
                        //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["ID_field"]] = FPropertiesParser.str_errors[2];
                        return;
                    }
                    dynamicCanvasComponent.DynamicCanvas.IDs[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("parent_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("parent_field", out string field_value))
                {
                    if (int.TryParse(field_value, out int parentindex) && parser.IsInRange(parentindex, true))
                    {
                        dynamicCanvasComponent.DynamicCanvas.parents[dynamicCanvasComponent.selection] = parentindex;
                        return;
                    }
                    else
                    {
                        if (parser.IsValidID(field_value, out int index))
                        {
                            dynamicCanvasComponent.DynamicCanvas.parents[dynamicCanvasComponent.selection] = index;
                            return;
                        }
                    }
                    //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["parent_field"]] = FPropertiesParser.str_errors[3];
                }
            });

            canvas.BindAction("widgettype_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("widgettype_field", out string field_value))
                {
                    field_value = field_value.ToUpper();
                    if (Enum.TryParse(field_value, out EWidgetType wtype))
                    {
                        dynamicCanvasComponent.DynamicCanvas.widgetTypes[dynamicCanvasComponent.selection] = wtype;
                    }
                    else
                    {
                        //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["widgettype_field"]] = FPropertiesParser.str_errors[7];
                    }
                }
            });

            canvas.BindAction("position_X_field.OnTextEntered", (gt) => { parser.HandleRectangleField("position", ref dynamicCanvasComponent.DynamicCanvas.rectangles); });
            canvas.BindAction("position_Y_field.OnTextEntered", (gt) => { parser.HandleRectangleField("position", ref dynamicCanvasComponent.DynamicCanvas.rectangles); });
            canvas.BindAction("position_W_field.OnTextEntered", (gt) => { parser.HandleRectangleField("position", ref dynamicCanvasComponent.DynamicCanvas.rectangles); });
            canvas.BindAction("position_H_field.OnTextEntered", (gt) => { parser.HandleRectangleField("position", ref dynamicCanvasComponent.DynamicCanvas.rectangles); });

            canvas.BindAction("texture_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("texture_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.textures[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("source_X_field.OnTextEntered", (gt) => { parser.HandleRectangleField("source", ref dynamicCanvasComponent.DynamicCanvas.sources); });
            canvas.BindAction("source_Y_field.OnTextEntered", (gt) => { parser.HandleRectangleField("source", ref dynamicCanvasComponent.DynamicCanvas.sources); });
            canvas.BindAction("source_W_field.OnTextEntered", (gt) => { parser.HandleRectangleField("source", ref dynamicCanvasComponent.DynamicCanvas.sources); });
            canvas.BindAction("source_H_field.OnTextEntered", (gt) => { parser.HandleRectangleField("source", ref dynamicCanvasComponent.DynamicCanvas.sources); });

            canvas.BindAction("alignment_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("alignment_field", out string field_value))
                {
                    field_value = field_value.ToUpper();
                    if (Enum.TryParse(field_value, out EWidgetAlignment alignment))
                    {
                        dynamicCanvasComponent.DynamicCanvas.alignments[dynamicCanvasComponent.selection] = alignment;
                    }
                    else
                    {
                        //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["alignment_field"]] = FPropertiesParser.str_errors[0];
                    }
                }
            });

            canvas.BindAction("anchor_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("anchor_field", out string field_value))
                {
                    field_value = field_value.ToUpper();
                    if (Enum.TryParse(field_value, out EWidgetAnchor anchor))
                    {
                        dynamicCanvasComponent.DynamicCanvas.anchors[dynamicCanvasComponent.selection] = anchor;
                    }
                    else
                    {
                        //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["anchor_field"]] = FPropertiesParser.str_errors[0];
                    }
                }
            });

            canvas.BindAction("text_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("text_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("font_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("font_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.fonts[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("color_R_field.OnTextEntered", (gt) => { parser.HandleColorField("color", ref dynamicCanvasComponent.DynamicCanvas.colors); });
            canvas.BindAction("color_G_field.OnTextEntered", (gt) => { parser.HandleColorField("color", ref dynamicCanvasComponent.DynamicCanvas.colors); });
            canvas.BindAction("color_B_field.OnTextEntered", (gt) => { parser.HandleColorField("color", ref dynamicCanvasComponent.DynamicCanvas.colors); });
            canvas.BindAction("color_A_field.OnTextEntered", (gt) => { parser.HandleColorField("color", ref dynamicCanvasComponent.DynamicCanvas.colors); });

            canvas.BindAction("highlightcolor_R_field.OnTextEntered", (gt) => { parser.HandleColorField("highlightcolor", ref dynamicCanvasComponent.DynamicCanvas.highlightColors); });
            canvas.BindAction("highlightcolor_G_field.OnTextEntered", (gt) => { parser.HandleColorField("highlightcolor", ref dynamicCanvasComponent.DynamicCanvas.highlightColors); });
            canvas.BindAction("highlightcolor_B_field.OnTextEntered", (gt) => { parser.HandleColorField("highlightcolor", ref dynamicCanvasComponent.DynamicCanvas.highlightColors); });
            canvas.BindAction("highlightcolor_A_field.OnTextEntered", (gt) => { parser.HandleColorField("highlightcolor", ref dynamicCanvasComponent.DynamicCanvas.highlightColors); });

            canvas.BindAction("clicksound_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("clicksound_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.clickSounds[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("hoversound_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("hoversound_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.hoverSounds[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("tooltip_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("tooltip_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.tooltips[dynamicCanvasComponent.selection] = field_value;
                }
            });

            canvas.BindAction("tooltiptext_field.OnTextEntered", (gt) => {
                if (parser.GetFieldValue("tooltiptext_field", out string field_value))
                {
                    dynamicCanvasComponent.DynamicCanvas.tooltiptexts[dynamicCanvasComponent.selection] = field_value;
                }
            });

            // todo tooltip text
        }

        public void RebuildCanvas()
        {
            factory = new DynamicSingleStructFactory(dynamicCanvasComponent.DynamicCanvas, dynamicCanvasComponent.selection);
        }

        public void Update(GameTime gameTime)
        {
            if (factory != null)
            {
                RebuildCanvasComponents(factory.GetCanvas());
                factory = null;
            }
        }
    }
}