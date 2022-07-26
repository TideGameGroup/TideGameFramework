using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct FCanvasDrawComponentConstructorArgs
    {
        public ACanvasComponent component;
        public UContentManager content;
    }

    public class ACanvasDrawComponent : UComponent, IDrawableCanvasComponent
    {
        private readonly FCanvasCache cache;
        private readonly ACanvasComponent component;
        private readonly FCanvasGraph graph;

        // instance data
        private readonly AInputComponent input;

        public ACanvasDrawComponent(FCanvasDrawComponentConstructorArgs args)
        {
            TrySetDefault(args.component, out component);
            TrySetDefault(args.component.cache, out cache);
            TrySetDefault(args.component.graph, out graph);
            TrySetOptional(args.component.InputComponent, out input);
        }

        private Color DoFlashing(FCanvas canvas, int i, GameTime gameTime)
        {
            float a = (float)Math.Sin((gameTime.TotalGameTime.TotalSeconds - component.highlightedWidgets[i]) * 3.14);
            a = MathF.Abs(a);
            return Color.Lerp(Color.Lerp(canvas.colors[i], Color.Black, 0.4f), Color.White, a);
        }

        private void DrawCanvas(FCanvas canvas, SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < canvas.parents.Length; i++)
            {
                Rectangle scissor = graph.GetRectangle(canvas, cache.canvas.parents[i]);
                Rectangle rect = graph.GetRectangleInParent(canvas, i, scissor);

                spriteBatch.GraphicsDevice.ScissorRectangle = scissor;

                // draw by type
                switch (canvas.widgetTypes[i])
                {
                    case EWidgetType.TEXT:
                        DrawTextWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.PANEL:
                    case EWidgetType.BUTTON:
                    case EWidgetType.LABEL:
                        DrawPanelWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.SLIDER:
                        DrawSliderWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.TICKBOX:
                        DrawTickBoxWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.SCROLLBAR:
                        DrawScrollBarWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.TEXTFIELD:
                        DrawTextFieldWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    default:
                        break;
                }
            }
        }

        private void DrawPanelWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                rect,
                canvas.sources[i],
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawScrollBarWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            Rectangle source = canvas.sources[i];
            Rectangle cSource = source;
            Rectangle rSource = source;
            Rectangle mSource = source;
            Rectangle lRect = rect;
            Rectangle clRect = rect;
            Rectangle crRect = rect;
            Rectangle rRect = rect;
            Rectangle mRect = rect;

            lRect.Height = source.Height;

            clRect.Height = (int)((rect.Height - source.Height) * component.values[i].f);
            crRect.Height = (int)((rect.Height - source.Height) * (1.0f - component.values[i].f));
            rRect.Height = source.Height;
            mRect.Height = source.Height;

            clRect.Offset(0, lRect.Height / 2);
            crRect.Offset(0, lRect.Height / 2 + clRect.Height);
            rRect.Offset(0, rect.Height - source.Height);
            mRect.Offset(0, (rect.Height - source.Height) * component.values[i].f);

            cSource.Offset(0, source.Height);
            rSource.Offset(0, source.Height * 2);
            mSource.Offset(0, source.Height * 3);

            // bar
            spriteBatch.Draw(
                 cache.textureCache[canvas.textures[i]],
                 clRect,
                 cSource,
                 canvas.highlightColors[i]
            );
            spriteBatch.Draw(
                  cache.textureCache[canvas.textures[i]],
                  crRect,
                  cSource,
                  canvas.colors[i]
             );

            // caps
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                lRect,
                source,
                canvas.colors[i]
            );
            spriteBatch.Draw(
                 cache.textureCache[canvas.textures[i]],
                 rRect,
                 rSource,
                 canvas.colors[i]
            );

            // grabber
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                mRect,
                mSource,
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawSliderWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            Rectangle source = canvas.sources[i];
            Rectangle cSource = source;
            Rectangle rSource = source;
            Rectangle mSource = source;
            Rectangle lRect = rect;
            Rectangle clRect = rect;
            Rectangle crRect = rect;
            Rectangle rRect = rect;
            Rectangle mRect = rect;

            lRect.Width = source.Width;

            clRect.Width = (int)((rect.Width - source.Width) * component.values[i].f);
            crRect.Width = (int)((rect.Width - source.Width) * (1.0f - component.values[i].f));
            rRect.Width = source.Width;
            mRect.Width = source.Width;

            clRect.Offset(lRect.Width / 2, 0);
            crRect.Offset(lRect.Width / 2 + clRect.Width, 0);
            rRect.Offset(rect.Width - source.Width, 0);
            mRect.Offset((rect.Width - source.Width) * component.values[i].f, 0);

            cSource.Offset(source.Width, 0);
            rSource.Offset(source.Width * 2, 0);
            mSource.Offset(source.Width * 3, 0);

            // bar
            spriteBatch.Draw(
                 cache.textureCache[canvas.textures[i]],
                 clRect,
                 cSource,
                 canvas.highlightColors[i]
            );
            spriteBatch.Draw(
                  cache.textureCache[canvas.textures[i]],
                  crRect,
                  cSource,
                  canvas.colors[i]
             );

            // caps
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                lRect,
                source,
                canvas.colors[i]
            );
            spriteBatch.Draw(
                 cache.textureCache[canvas.textures[i]],
                 rRect,
                 rSource,
                 canvas.colors[i]
            );

            // grabber
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                mRect,
                mSource,
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawTextWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.DrawString(
                cache.fontCache[canvas.fonts[i]],
                canvas.texts[i],
                rect.Location.ToVector2(),
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawTextFieldWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                rect,
                canvas.sources[i],
                GetWidgetDrawColor(canvas, i, gameTime)
            );
            spriteBatch.DrawString(
                cache.fontCache[canvas.fonts[i]],
                canvas.texts[i],
                rect.Location.ToVector2(),
                Color.Black
            );
        }

        private void DrawTickBoxWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.Draw(
                cache.textureCache[canvas.textures[i]],
                rect,
                canvas.sources[i],
                GetWidgetDrawColor(canvas, i, gameTime)
            );

            if (component.values[i].b)
            {
                Rectangle tSrc = canvas.sources[i];
                tSrc.Offset(tSrc.Width, 0);

                spriteBatch.Draw(
                    cache.textureCache[canvas.textures[i]],
                    rect,
                    tSrc,
                    GetWidgetDrawColor(canvas, i, gameTime)
                );
            }
        }

        private Color GetWidgetDrawColor(FCanvas canvas, int i, GameTime gameTime)
        {
            Color color = canvas.colors[i];

            if (!component.IsActive)
            {
                return Color.DarkGray;
            }

            // ignore tooltips
            if (canvas.ID == cache.canvas.ID)
            {
                if (component.hoveredWidgets.ContainsKey(i))
                {
                    if (component.IsHovered)
                    {
                        color = canvas.highlightColors[i];
                    }
                }
                else if (component.highlightedWidgets.ContainsKey(i))
                {
                    color = DoFlashing(canvas, i, gameTime);
                }
            }
            return color;
        }

        // draw interface
        public void Draw(FView view2D, SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle scissor = spriteBatch.GraphicsDevice.ScissorRectangle;

            // draw UI
            DrawCanvas(cache.canvas, spriteBatch, gameTime);

            // draw tooltips
            foreach (int i in component.hoveredWidgets.Keys)
            {
                if (gameTime.TotalGameTime.TotalSeconds - component.hoveredWidgets[i] < 0.5)
                {
                    continue;
                }

                if (input != null && cache.tooltipCache.ContainsKey(cache.canvas.tooltips[i]))
                {
                    FCanvas canvasData = cache.tooltipCache[cache.canvas.tooltips[i]];
                    canvasData.root.X = (int)input.MousePosition.X;
                    canvasData.root.Y = (int)input.MousePosition.Y;

                    for (int t = 0; t < canvasData.IDs.Length; t++)
                    {
                        if (canvasData.IDs[t] == "tooltip_text")
                        {
                            canvasData.texts[t] = cache.canvas.tooltiptexts[i];
                            cache.tooltipCache[cache.canvas.tooltips[i]] = canvasData;
                        }
                    }

                    DrawCanvas(canvasData, spriteBatch, gameTime);
                }
            }

            spriteBatch.GraphicsDevice.ScissorRectangle = scissor;
        }

        public Color GetWidgetColor(string widget)
        {
            int i = graph.widgetNameIndexMap[widget];
            return GetWidgetColor(i);
        }

        public Color GetWidgetColor(int i)
        {
            return cache.canvas.colors[i];
        }

        public Rectangle GetWidgetSource(string widget)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return default; }

            int i = graph.widgetNameIndexMap[widget];
            return GetWidgetSource(i);
        }

        public Rectangle GetWidgetSource(int i)
        {
            return cache.canvas.sources[i];
        }

        public void SetHighlightColor(string widget, Color value)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            SetHighlightColor(i, value);
        }

        public void SetHighlightColor(int i, Color value)
        {
            cache.canvas.highlightColors[i] = value;
        }

        public void SetWidgetColor(string widget, Color value)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            SetWidgetColor(i, value);
        }

        public void SetWidgetColor(int i, Color value)
        {
            cache.canvas.colors[i] = value;
        }

        public void SetWidgetSource(string widget, Rectangle source)
        {
            if (!graph.widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = graph.widgetNameIndexMap[widget];
            SetWidgetSource(i, source);
        }

        public void SetWidgetSource(int i, Rectangle source)
        {
            cache.canvas.sources[i] = source;
        }
    }
}