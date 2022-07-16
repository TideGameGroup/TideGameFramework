﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void WidgetDelegate(GameTime gameTime);
    public delegate void FocusWidgetDelegate(int i);

    public struct FCanvasComponentConstructorArgs
    {
        public UAudio audio;
        public FCanvas canvas;
        public UContentManager content;
        public EFocus focus;
        public AInputComponent input;
        public float scale;
        public GameWindow window;
    }

    public class ACanvasComponent : UComponent, IUpdateComponent
    {
        private readonly UAudio audio;
        private readonly Dictionary<string, List<WidgetDelegate>> bindings = new Dictionary<string, List<WidgetDelegate>>();
        private readonly EFocus focus;
        private readonly AInputComponent input;
        private bool bIsHovered = false;
        private string suffix = ".Released";
        public readonly FCanvasCache cache;
        public readonly FCanvasGraph graph;
        public readonly Dictionary<int, double> highlightedWidgets = new Dictionary<int, double>();
        public readonly List<FSetting> values = new List<FSetting>();
        public string clickSound = "tick";
        public int focusedWidget = -1;
        public Dictionary<int, double> hoveredWidgets = new Dictionary<int, double>();
        public string hoverSound = "blip";

        public ACanvasComponent(FCanvasComponentConstructorArgs args)
        {
            NullCheck(args.canvas);
            NullCheck(args.content);
            NullCheck(args.scale);
            TrySetDefault(args.focus, out focus);
            TrySetOptional(args.input, out input);
            TrySetOptional(args.audio, out audio);

            IsEnabled = true;

            FCanvasCacheConstructorArgs cacheArgs = new FCanvasCacheConstructorArgs
            {
                canvas = args.canvas,
                content = args.content
            };

            cache = new FCanvasCache(cacheArgs);

            FCanvasGraphConstructorArgs graphArgs = new FCanvasGraphConstructorArgs
            {
                content = args.content,
                cache = cache,
                scale = args.scale
            };

            graph = new FCanvasGraph(graphArgs);

            for (int i = 0; i < cache.canvas.IDs.Length; i++)
            {
                values.Add(FSetting.Float(1.0f));
            }

            if (input != null)
            {
                FActionHandle handle1 = input.BindRawAction("primary.Pressed", (GameTime gt) => { suffix = ".Pressed"; });
                FActionHandle handle2 = input.BindRawAction("primary.OnPressed", (GameTime gt) => 
                { 
                    suffix = ".OnPressed";
                    if (focusedWidget != -1)
                    {
                        OnWidgetUnFocused?.Invoke(focusedWidget);
                        focusedWidget = -1;
                    }
                });
                FActionHandle handle3 = input.BindRawAction("primary.Released", (GameTime gt) => { suffix = ".Released"; });
                FActionHandle handle4 = input.BindRawAction("primary.OnReleased", (GameTime gt) => { suffix = ".OnReleased"; });
                
                OnUnregisterComponent += () =>
                {
                    input.UnbindAction(handle1);
                    input.UnbindAction(handle2);
                    input.UnbindAction(handle3);
                    input.UnbindAction(handle4);
                };
            }

            OnUnregisterComponent += () =>
            {
                UnFocusOnFocusLost();
            };

            if (args.window != null)
            {
                args.window.TextInput += handleTextInput;
            }
        }

        public void UnFocusOnFocusLost()
        {
            if (bIsHovered)
            {
                bIsHovered = false;
                AInputComponent.PopFocus(focus);
            }
        }

        public bool IsEnabled { get; set; }
        public bool IsHovered { get => bIsHovered; }
        public FocusWidgetDelegate OnWidgetFocused { get; set; }
        public FocusWidgetDelegate OnWidgetUnFocused { get; set; }

        private void DoHover(GameTime gameTime, Dictionary<int, double> frameHoveredWidgets, int i)
        {
            if (hoveredWidgets.ContainsKey(i))
            {
                frameHoveredWidgets.TryAdd(i, hoveredWidgets[i]);
            }
            else
            {
                frameHoveredWidgets.TryAdd(i, gameTime.TotalGameTime.TotalSeconds);
                audio?.PlaySingle(hoverSound);
            }
        }

        private bool DoScrollbarMovement(int i, Rectangle rect)
        {
            if (IsEnabled && input != null && input.CheckValidToTrigger(focus))
            {
                float height = rect.Height - cache.canvas.sources[i].Height;
                float start = rect.Top + (cache.canvas.sources[i].Height / 2);
                float current = input.MousePosition.Y;
                float value = ((current - start) / (height + 0.00001f));
                value = Math.Clamp(value, 0.0f, 1.0f);

                FSetting oldSetting = values[i];
                values[i] = FSetting.Float(value);
                return oldSetting.f != values[i].f;
            }
            return false;
        }

        private bool DoSliderMovement(int i, Rectangle rect)
        {
            if (IsEnabled && input != null && input.CheckValidToTrigger(focus))
            {
                float width = rect.Width - cache.canvas.sources[i].Width;
                float start = rect.Left + (cache.canvas.sources[i].Width / 2);
                float current = input.MousePosition.X;
                float value = ((current - start) / (width + 0.00001f));
                value = Math.Clamp(value, 0.0f, 1.0f);

                FSetting oldSetting = values[i];
                values[i] = FSetting.Float(value);
                return oldSetting.f != values[i].f;
            }
            return false;
        }

        private void handleTextInput(object sender, TextInputEventArgs e)
        {
            if (focusedWidget == -1) { return; }
            if (FTextField.HandleTextInput(cache.canvas, focusedWidget, e))
            {
                string key = cache.canvas.IDs[focusedWidget] + ".OnTextEntered";
                InvokeBindings(null, focusedWidget, key);
                OnWidgetUnFocused?.Invoke(focusedWidget);
                focusedWidget = -1;
            }
        }

        private void InvokeBindings(GameTime gameTime, int i)
        {
            string key = cache.canvas.IDs[i] + suffix;
            InvokeBindings(gameTime, i, key);
        }

        private void InvokeBindings(GameTime gameTime, int i, string key)
        {
            if (IsEnabled && input != null && input.CheckValidToTrigger(focus))
            {
                if (bindings.ContainsKey(key))
                {
                    foreach (var binding in bindings[key])
                    {
                        binding.Invoke(gameTime);
                    }
                    audio?.PlaySingle(clickSound);
                }
            }
        }

        private void ToggleTickBox(int i)
        {
            if (IsEnabled && input != null && input.CheckValidToTrigger(focus))
            {
                values[i] = FSetting.Bool(!values[i].b);
            }
        }

        public void BindAction(string action, WidgetDelegate callback)
        {
            if (!bindings.ContainsKey(action))
            {
                bindings[action] = new List<WidgetDelegate>();
            }

            bindings[action].Add(callback);
        }

        public string GetID()
        {
            return cache.canvas.ID;
        }

        public Rectangle GetRectangle(string widget)
        {
            int i = graph.widgetNameIndexMap[widget];
            return GetRectangle(i);
        }

        public Rectangle GetRectangle(int i)
        {
            return cache.canvas.rectangles[i];
        }

        public string GetWidgetText(string widget)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return default; }
            int i = graph.widgetNameIndexMap[widget];
            return GetWidgetText(i);
        }

        public string GetWidgetText(int i)
        {
            return cache.canvas.texts[i];
        }

        public FSetting GetWidgetValue(string widget)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return default; }
            int i = graph.widgetNameIndexMap[widget];
            return GetWidgetValue(i);
        }

        public FSetting GetWidgetValue(int i)
        {
            return values[i];
        }

        public void HandleOnHover(int i, Dictionary<int, double> frameHoveredWidgets, Rectangle rect, GameTime gameTime)
        {
            switch (cache.canvas.widgetTypes[i])
            {
                case EWidgetType.TEXT:
                    DoHover(gameTime, frameHoveredWidgets, i);
                    break;

                case EWidgetType.PANEL:
                    break;

                case EWidgetType.BUTTON:
                    DoHover(gameTime, frameHoveredWidgets, i);

                    InvokeBindings(gameTime, i);
                    break;

                case EWidgetType.LABEL:
                    break;

                case EWidgetType.SCROLLBAR:
                    DoHover(gameTime, frameHoveredWidgets, i);

                    if (suffix != ".Released")
                    {
                        if (DoScrollbarMovement(i, rect))
                        {
                            InvokeBindings(gameTime, i);
                        }
                    }
                    break;

                case EWidgetType.SLIDER:
                    DoHover(gameTime, frameHoveredWidgets, i);

                    if (suffix != ".Released")
                    {
                        if (DoSliderMovement(i, rect))
                        {
                            InvokeBindings(gameTime, i);
                        }
                    }
                    break;

                case EWidgetType.TICKBOX:
                    DoHover(gameTime, frameHoveredWidgets, i);

                    if (suffix == ".OnReleased")
                    {
                        ToggleTickBox(i);
                    }
                    InvokeBindings(gameTime, i);
                    break;

                case EWidgetType.TEXTFIELD:
                    DoHover(gameTime, frameHoveredWidgets, i);

                    if (suffix == ".OnReleased")
                    {
                        focusedWidget = i;
                        cache.canvas.texts[i] = "";
                        OnWidgetFocused?.Invoke(i);
                        // set cursor position here
                        // FTextField.SetCursorPosition(mouseposition, rect)
                    }
                    break;

                default:
                    break;
            }
        }

        public void HighlightWidget(string widget, double time)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            HighlightWidget(i, time);
        }

        public void HighlightWidget(int i, double time)
        {
            highlightedWidgets.Add(i, time);
        }

        public bool IsWidgetHovered(int i, ref Rectangle rect)
        {
            if (input == null) { return false; }

            Rectangle scissor = graph.GetRectangle(cache.canvas, cache.canvas.parents[i]);
            rect = graph.GetRectangleInParent(cache.canvas, i, scissor);

            if (i == focusedWidget) { return true; }
            if (!scissor.Contains(input.MousePosition)) { return false; }
            if (!rect.Contains(input.MousePosition)) { return false; }

            return true;
        }

        public void SetTooltipText(string toolTip, string widget, string text)
        {
            if (cache.tooltipCache.ContainsKey(toolTip))
            {
                FCanvas toolTipCanvas = cache.tooltipCache[toolTip];

                for (int i = 0; i < toolTipCanvas.IDs.Length; i++)
                {
                    if (cache.tooltipCache[toolTip].IDs[i] == widget)
                    {
                        toolTipCanvas.texts[i] = text;
                    }
                }
            }
        }

        public void SetWidgetText(string widget, string value)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            SetWidgetText(i, value);
        }

        public void SetWidgetText(int i, string value)
        {
            cache.canvas.texts[i] = value;
        }

        public void SetWidgetValue(string widget, FSetting value)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            SetWidgetValue(i, value);
        }

        public void SetWidgetValue(int i, FSetting value)
        {
            values[i] = value;
        }

        public void UnbindAction(string action, WidgetDelegate callback)
        {
            if (bindings.ContainsKey(action))
            {
                bindings[action].Remove(callback);
            }
        }

        public void UnHighlightWidget(string widget)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            UnHighlightWidget(i);
        }

        public void UnHighlightWidget(int i)
        {
            highlightedWidgets.Remove(i);
        }

        public void Update(GameTime gameTime)
        {
            if (input == null) { return; }

            Dictionary<int, double> frameHoveredWidgets = new Dictionary<int, double>();
            bool hovered = false;

            for (int i = 0; i < graph.Count; i++)
            {
                Rectangle rect = Rectangle.Empty;

                if (IsWidgetHovered(i, ref rect))
                {
                    hovered = true;
                    HandleOnHover(i, frameHoveredWidgets, rect, gameTime);
                }
            }

            hoveredWidgets = frameHoveredWidgets;

            if (hovered != bIsHovered)
            {
                if (hovered)
                {
                    AInputComponent.PushFocus(focus);
                    bIsHovered = true;
                }
                else if (focusedWidget == -1) // remain "hovered" if a widget is focused
                {
                    AInputComponent.PopFocus(focus);
                    bIsHovered = false;
                }
            }
        }
    }
}