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
        public EditorDynamicCanvasComponent dynamicCanvasComponent;
        public AInputComponent input;
        public GameWindow window;
    }

    public class EditorPropertiesCanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;
        private readonly AInputComponent input;
        private readonly GameWindow window;
        private ITreeCanvasFactory factory = null;
        public EditorDynamicCanvasComponent dynamicCanvasComponent;

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
            UnregisterChildComponent(CanvasComponent);
            UnregisterChildComponent(DrawComponent);

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
                    content = content,
                    input = input
                };

            DrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            RegisterChildComponent(CanvasComponent);
            RegisterChildComponent(DrawComponent);

            SetupBindings(CanvasComponent);
        }

        private void SetupBindings(ACanvasComponent canvas)
        {
            List<string> str_errors = new List<string>
            {
                "invalid value",
                "value cannot be null or whitespace",
                "duplicate IDs are not allowed",
            };

            bool IsInvalid(string str, out string error)
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

            bool GetFieldValue(string field, out string field_value)
            {
                field_value = CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap[field]];

                if (IsInvalid(field_value, out string err))
                {
                    CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap[field]] = err;
                    return false;
                }
                return true;
            }

            canvas.BindAction("ID_field.OnTextEntered", (gt) => {
                if (GetFieldValue("ID_field", out string field_value))
                {
                    for (int i = 0; i < dynamicCanvasComponent.DynamicCanvas.Count; i++)
                    {
                        if (dynamicCanvasComponent.DynamicCanvas.IDs[i] == field_value)
                        {
                            CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["ID_field"]] = str_errors[2];
                            return;
                        }
                    }

                    dynamicCanvasComponent.DynamicCanvas.IDs[dynamicCanvasComponent.selection] = field_value;
                    dynamicCanvasComponent.Rebuild();
                }
            });

            canvas.BindAction("parent_field.OnTextEntered", (gt) => {
                string field_value = CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["parent_field"]];

                if (int.TryParse(field_value, out int parentindex))
                {
                    if (parentindex >= -1 && parentindex < dynamicCanvasComponent.DynamicCanvas.Count)
                    {
                        dynamicCanvasComponent.DynamicCanvas.parents[dynamicCanvasComponent.selection] = parentindex;
                        dynamicCanvasComponent.Rebuild();
                        return;
                    }
                }
                else
                {
                    for (int i = 0; i < dynamicCanvasComponent.DynamicCanvas.Count; i++)
                    {
                        if (dynamicCanvasComponent.DynamicCanvas.IDs[i] == field_value)
                        {
                            dynamicCanvasComponent.DynamicCanvas.parents[dynamicCanvasComponent.selection] = i;
                            dynamicCanvasComponent.Rebuild();
                            return;
                        }
                    }
                }

                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["parent_field"]] = "invalid value";
            });

            canvas.BindAction("position_field.OnTextEntered", (gt) => {
                //dynamicCanvasComponent.DynamicCanvas.IDs[dynamicCanvasComponent.selection] =
                //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["position_field"]];
                dynamicCanvasComponent.Refresh();
            });

            canvas.BindAction("texture_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.textures[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["texture_field"]];

                // check font is loaded otherwise set to default

                dynamicCanvasComponent.Rebuild();
            });

            canvas.BindAction("source_field.OnTextEntered", (gt) => {
                //dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] =
                //CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["source_field"]];
                dynamicCanvasComponent.Refresh();
            });

            canvas.BindAction("alignment_field.OnTextEntered", (gt) => {
                string field_value = CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["alignment_field"]];

                if (Enum.TryParse(field_value, out EWidgetAlignment alignment))
                {
                    dynamicCanvasComponent.DynamicCanvas.alignments[dynamicCanvasComponent.selection] = alignment;
                    dynamicCanvasComponent.Refresh();
                }
                else
                {
                    CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["alignment_field"]] = "invalid value";
                }
            });

            canvas.BindAction("anchor_field.OnTextEntered", (gt) => {
                string field_value = CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["anchor_field"]];

                if (Enum.TryParse(field_value, out EWidgetAnchor anchor))
                {
                    dynamicCanvasComponent.DynamicCanvas.anchors[dynamicCanvasComponent.selection] = anchor;
                    dynamicCanvasComponent.Refresh();
                }
                else
                {
                    CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["anchor_field"]] = "invalid value";
                }
            });

            canvas.BindAction("text_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["text_field"]];
                dynamicCanvasComponent.Refresh();
            });

            canvas.BindAction("font_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.fonts[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["font_field"]];

                // check font is loaded - if not use arial

                dynamicCanvasComponent.Rebuild();
            });

            canvas.BindAction("color_field.OnTextEntered", (gt) => {
                //dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] =
                //CanvasComponent.cache.canvas.colors[CanvasComponent.graph.widgetNameIndexMap["color_field"]];
                dynamicCanvasComponent.Refresh();
            });

            canvas.BindAction("highlightcolor_field.OnTextEntered", (gt) => {
                //dynamicCanvasComponent.DynamicCanvas.texts[dynamicCanvasComponent.selection] =
                //CanvasComponent.cache.canvas.colors[CanvasComponent.graph.widgetNameIndexMap["color_field"]];
                dynamicCanvasComponent.Refresh();
            });

            canvas.BindAction("clicksound_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.clickSounds[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["clicksound_field"]];
                dynamicCanvasComponent.Rebuild();
            });

            canvas.BindAction("hoversound_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.hoverSounds[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["hoversound_field"]];
                dynamicCanvasComponent.Rebuild();
            });

            canvas.BindAction("tooltip_field.OnTextEntered", (gt) => {
                dynamicCanvasComponent.DynamicCanvas.hoverSounds[dynamicCanvasComponent.selection] =
                CanvasComponent.cache.canvas.texts[CanvasComponent.graph.widgetNameIndexMap["hoversound_field"]];
                dynamicCanvasComponent.Rebuild();
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