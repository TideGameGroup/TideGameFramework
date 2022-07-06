using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void WidgetDelegate(GameTime gameTime);

    public class ACanvasComponent : UComponent, IDrawableCanvasScript, IUpdateComponent
    {
        private readonly UAudio audio;
        private Dictionary<string, List<WidgetDelegate>> bindings = new Dictionary<string, List<WidgetDelegate>>();
        private bool bIsHovered = false;

        // instance data
        protected FCanvas canvas;

        protected UContentManager content;
        protected EFocus focus;
        protected Dictionary<string, SpriteFont> fontCache = new Dictionary<string, SpriteFont>();
        protected Dictionary<int, double> highlightedWidgets = new Dictionary<int, double>();
        protected Dictionary<int, double> hoveredWidgets = new Dictionary<int, double>();
        protected AInputComponent input;
        protected string suffix = ".Released";
        protected Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        protected Dictionary<string, FCanvas> tooltipCache = new Dictionary<string, FCanvas>();
        protected Dictionary<string, int> widgetNameIndexMap = new Dictionary<string, int>();
        protected List<FSetting> widgetValues = new List<FSetting>();

        public string hoverSound = "blip";
        public string clickSound = "tick";

        public ACanvasComponent(UContentManager content, AInputComponent input, FCanvas canvas, EFocus focus = EFocus.GameUI, UAudio audio = null)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
            this.input = input;
            this.focus = focus;
            this.audio = audio;
            this.canvas = canvas.DeepCopy();

            IsEnabled = true;
            Scale = 1.0f;

            for (int i = 0; i < this.canvas.IDs.Length; i++)
            {
                widgetNameIndexMap.Add(this.canvas.IDs[i], i);
                widgetValues.Add(FSetting.Float(1.0f));
            }

            CacheWidgetContent(content, this.canvas);

            if (input != null)
            {
                input.BindRawAction("primary.Pressed", (GameTime gt) => { suffix = ".Pressed"; });
                input.BindRawAction("primary.OnPressed", (GameTime gt) => { suffix = ".OnClick"; });
                input.BindRawAction("primary.Released", (GameTime gt) => { suffix = ".Released"; });
                input.BindRawAction("primary.OnReleased", (GameTime gt) => { suffix = ".OnEndClick"; });
            }

            OnUnregisterComponent += () =>
            {
                if (bIsHovered)
                {
                    bIsHovered = false;
                    AInputComponent.PopFocus(focus);
                }
            };
        }

        public bool IsEnabled { get; set; }

        public bool IsHovered { get => bIsHovered; }
        public float Scale { get; set; }

        private void CacheWidgetContent(UContentManager content, FCanvas widget)
        {
            foreach (string texture in widget.textures)
            {
                if (texture == "") { continue; }
                textureCache.TryAdd(texture, content.Load<Texture2D>(texture));
            }

            CreateDefaultTexture(content);

            foreach (string tooltip in widget.tooltips)
            {
                if (tooltip == "") { continue; }

                FCanvas canvas = content.Load<FCanvas>(tooltip).DeepCopy();
                tooltipCache.TryAdd(tooltip, canvas);
                CacheWidgetContent(content, canvas);
            }

            foreach (string font in widget.fonts)
            {
                if (font == "") { continue; }
                fontCache.TryAdd(font, content.Load<SpriteFont>(font));
            }
        }

        private Rectangle CalculateWidgetPosition(FCanvas canvas, Rectangle root, int i)
        {
            Rectangle rect = canvas.rectangles[i];

            rect.X = (int)(rect.X * Scale);
            rect.Y = (int)(rect.Y * Scale);
            rect.Width = (int)(rect.Width * Scale);
            rect.Height = (int)(rect.Height * Scale);

            rect.Offset(root.Location);

            float width = rect.Width;

            if (canvas.widgetTypes[i] == EWidgetType.text)
            {
                width = fontCache[canvas.fonts[i]].MeasureString(canvas.texts[i]).X;
            }

            switch (canvas.alignments[i])
            {
                case EWidgetAlignment.Centre:
                    rect.Offset(new Vector2(-width / 2, 0.0f));
                    break;

                case EWidgetAlignment.Right:
                    rect.Offset(new Vector2(-width, 0.0f));
                    break;

                default:
                    break;
            }

            return rect;
        }

        private void CreateDefaultTexture(UContentManager content)
        {
            Texture2D texture = new Texture2D(content.GraphicsDevice, 8, 8);

            Color[] data = new Color[texture.Width * texture.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.White;
            }
            texture.SetData(data);
            textureCache.TryAdd("", texture);
        }

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
            if (IsEnabled && input.CheckValidToTrigger(focus))
            {
                float height = rect.Height - canvas.sources[i].Height;
                float start = rect.Top + (canvas.sources[i].Height / 2);
                float current = input.MousePosition.Y;
                float value = ((current - start) / (height + 0.00001f));
                value = Math.Clamp(value, 0.0f, 1.0f);

                FSetting oldSetting = widgetValues[i];
                widgetValues[i] = FSetting.Float(value);
                return oldSetting.f != widgetValues[i].f;
            }
            return false;
        }

        private bool DoSliderMovement(int i, Rectangle rect)
        {
            if (IsEnabled && input.CheckValidToTrigger(focus))
            {
                float width = rect.Width - canvas.sources[i].Width;
                float start = rect.Left + (canvas.sources[i].Width / 2);
                float current = input.MousePosition.X;
                float value = ((current - start) / (width + 0.00001f));
                value = Math.Clamp(value, 0.0f, 1.0f);

                FSetting oldSetting = widgetValues[i];
                widgetValues[i] = FSetting.Float(value);
                return oldSetting.f != widgetValues[i].f;
            }
            return false;
        }

        private void DrawCanvas(FCanvas canvas, SpriteBatch spriteBatch, int currentParent, Rectangle root, GameTime gameTime)
        {
            for (int i = 0; i < canvas.parents.Length; i++)
            {
                spriteBatch.GraphicsDevice.ScissorRectangle = GetScissor(canvas.parents[i], currentParent, root, content.GraphicsDevice.Viewport.Bounds);
                Rectangle rect = CalculateWidgetPosition(canvas, root, i);

                // draw by type
                switch (canvas.widgetTypes[i])
                {
                    case EWidgetType.text:
                        DrawTextWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.panel:
                    case EWidgetType.button:
                    case EWidgetType.label:
                        DrawPanelWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.slider:
                        DrawSliderWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.tickbox:
                        DrawTickBoxWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    case EWidgetType.scrollbar:
                        DrawScrollBarWidget(canvas, spriteBatch, i, rect, gameTime);
                        break;

                    default:
                        break;
                }
            }
        }

        private void DrawPanelWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
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

            clRect.Height = (int)((rect.Height - source.Height) * widgetValues[i].f);
            crRect.Height = (int)((rect.Height - source.Height) * (1.0f - widgetValues[i].f));
            rRect.Height = source.Height;
            mRect.Height = source.Height;

            clRect.Offset(0, lRect.Height / 2);
            crRect.Offset(0, lRect.Height / 2 + clRect.Height);
            rRect.Offset(0, rect.Height - source.Height);
            mRect.Offset(0, (rect.Height - source.Height) * widgetValues[i].f);

            cSource.Offset(0, source.Height);
            rSource.Offset(0, source.Height * 2);
            mSource.Offset(0, source.Height * 3);

            // bar
            spriteBatch.Draw(
                 textureCache[canvas.textures[i]],
                 clRect,
                 cSource,
                 canvas.highlightColors[i]
            );
            spriteBatch.Draw(
                  textureCache[canvas.textures[i]],
                  crRect,
                  cSource,
                  canvas.colors[i]
             );

            // caps
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
                lRect,
                source,
                canvas.colors[i]
            );
            spriteBatch.Draw(
                 textureCache[canvas.textures[i]],
                 rRect,
                 rSource,
                 canvas.colors[i]
            );

            // grabber
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
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

            clRect.Width = (int)((rect.Width - source.Width) * widgetValues[i].f);
            crRect.Width = (int)((rect.Width - source.Width) * (1.0f - widgetValues[i].f));
            rRect.Width = source.Width;
            mRect.Width = source.Width;

            clRect.Offset(lRect.Width / 2, 0);
            crRect.Offset(lRect.Width / 2 + clRect.Width, 0);
            rRect.Offset(rect.Width - source.Width, 0);
            mRect.Offset((rect.Width - source.Width) * widgetValues[i].f, 0);

            cSource.Offset(source.Width, 0);
            rSource.Offset(source.Width * 2, 0);
            mSource.Offset(source.Width * 3, 0);

            // bar
            spriteBatch.Draw(
                 textureCache[canvas.textures[i]],
                 clRect,
                 cSource,
                 canvas.highlightColors[i]
            );
            spriteBatch.Draw(
                  textureCache[canvas.textures[i]],
                  crRect,
                  cSource,
                  canvas.colors[i]
             );

            // caps
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
                lRect,
                source,
                canvas.colors[i]
            );
            spriteBatch.Draw(
                 textureCache[canvas.textures[i]],
                 rRect,
                 rSource,
                 canvas.colors[i]
            );

            // grabber
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
                mRect,
                mSource,
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawTextWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.DrawString(
                fontCache[canvas.fonts[i]],
                canvas.texts[i],
                rect.Location.ToVector2(),
                GetWidgetDrawColor(canvas, i, gameTime)
            );
        }

        private void DrawTickBoxWidget(FCanvas canvas, SpriteBatch spriteBatch, int i, Rectangle rect, GameTime gameTime)
        {
            spriteBatch.Draw(
                textureCache[canvas.textures[i]],
                rect,
                canvas.sources[i],
                GetWidgetDrawColor(canvas, i, gameTime)
            );

            if (widgetValues[i].b)
            {
                Rectangle tSrc = canvas.sources[i];
                tSrc.Offset(tSrc.Width, 0);

                spriteBatch.Draw(
                    textureCache[canvas.textures[i]],
                    rect,
                    tSrc,
                    GetWidgetDrawColor(canvas, i, gameTime)
                );
            }
        }

        private Rectangle GetMinRectangle(Rectangle A, Rectangle B)
        {
            return new Rectangle
            {
                X = A.X,
                Y = A.Y,
                Width = B.Width == 0 ? A.Width : Math.Min(A.Width, B.Width),
                Height = B.Height == 0 ? A.Height : Math.Min(A.Height, B.Height)
            };
        }

        private Rectangle GetScissor(int parentIndex, int currentParent, Rectangle root, Rectangle defaultScissor)
        {
            if (parentIndex != currentParent)
            {
                if (parentIndex == -1)
                {
                    return defaultScissor;
                }
                else
                {
                    Rectangle parentRect = canvas.rectangles[parentIndex];
                    parentRect.Offset(root.Location);
                    return parentRect;
                }
            }

            return defaultScissor;
        }

        private Color GetWidgetDrawColor(FCanvas canvas, int i, GameTime gameTime)
        {
            Color color = canvas.colors[i];

            if (!IsEnabled)
            {
                return Color.DarkGray;
            }

            // ignore tooltips
            if (canvas.ID == this.canvas.ID)
            {
                if (hoveredWidgets.ContainsKey(i))
                {
                    if (bIsHovered)
                    {
                        color = canvas.highlightColors[i];
                    }
                }
                else if (highlightedWidgets.ContainsKey(i))
                {
                    float a = (float)Math.Sin((gameTime.TotalGameTime.TotalSeconds - highlightedWidgets[i]) * 3.14);
                    a = MathF.Abs(a);
                    color = Color.Lerp(Color.Lerp(canvas.colors[i], Color.Black, 0.4f), Color.White, a);
                }
            }
            return color;
        }

        private void InvokeBindings(GameTime gameTime, int i)
        {
            if (IsEnabled && input.CheckValidToTrigger(focus))
            {
                string key = canvas.IDs[i] + suffix;
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
            if (IsEnabled && input.CheckValidToTrigger(focus))
            {
                widgetValues[i] = FSetting.Bool(!widgetValues[i].b);
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

        // draw interface
        public void DrawUI(UView view2D, SpriteBatch spriteBatch, GameTime gameTime)
        {
            int currentParent = 0;

            Rectangle view = content.GraphicsDevice.Viewport.Bounds;
            Rectangle root = GetMinRectangle(view, canvas.root);
            Rectangle anchoredRoot = GetAnchoredRectangle(canvas.root, root, canvas.anchor);

            DrawCanvas(canvas, spriteBatch, currentParent, anchoredRoot, gameTime);

            foreach (int i in hoveredWidgets.Keys)
            {
                if (gameTime.TotalGameTime.TotalSeconds - hoveredWidgets[i] < 0.5)
                {
                    continue;
                }

                if (input != null && tooltipCache.ContainsKey(canvas.tooltips[i]))
                {
                    FCanvas canvasData = tooltipCache[canvas.tooltips[i]];

                    Rectangle toolroot = canvasData.root;
                    toolroot.X = (int)input.MousePosition.X;
                    toolroot.Y = (int)input.MousePosition.Y;

                    DrawCanvas(canvasData, spriteBatch, currentParent, toolroot, gameTime);
                }
            }

            spriteBatch.GraphicsDevice.ScissorRectangle = view;
        }

        public Rectangle GetAnchoredRectangle(Rectangle rect, Rectangle root, EWidgetAnchor anchor)
        {
            switch (anchor)
            {
                case EWidgetAnchor.N:
                    rect.X += (root.Width / 2);
                    break;

                case EWidgetAnchor.NE:
                    rect.X += root.Width;
                    break;

                case EWidgetAnchor.E:
                    rect.X += root.Width;
                    rect.Y += (root.Height / 2);
                    break;

                case EWidgetAnchor.SE:
                    rect.X += root.Width;
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.S:
                    rect.X += (root.Width / 2);
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.SW:
                    rect.Y += root.Height;
                    break;

                case EWidgetAnchor.W:
                    rect.Y += root.Height / 2;
                    break;

                case EWidgetAnchor.NW:
                    break;

                case EWidgetAnchor.C:
                    rect.X += (root.Width / 2);
                    rect.Y += (root.Height / 2);
                    break;

                default:
                    break;
            }
            return rect;
        }

        public string GetID()
        {
            return canvas.ID;
        }

        public Rectangle GetRectangle(string widget)
        {
            int i = widgetNameIndexMap[widget];
            return GetRectangle(i);
        }

        public Rectangle GetRectangle(int i)
        {
            return canvas.rectangles[i];
        }

        public Rectangle GetRoot()
        {
            return canvas.root;
        }

        public Color GetWidgetColor(string widget)
        {
            int i = widgetNameIndexMap[widget];
            return GetWidgetColor(i);
        }

        public Color GetWidgetColor(int i)
        {
            return canvas.colors[i];
        }

        public Rectangle GetWidgetSource(string widget)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return default; }

            int i = widgetNameIndexMap[widget];
            return GetWidgetSource(i);
        }

        public Rectangle GetWidgetSource(int i)
        {
            return canvas.sources[i];
        }

        public string GetWidgetText(string widget)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return default; }
            int i = widgetNameIndexMap[widget];
            return GetWidgetText(i);
        }

        public string GetWidgetText(int i)
        {
            return canvas.texts[i];
        }

        public FSetting GetWidgetValue(string widget)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return default; }
            int i = widgetNameIndexMap[widget];
            return GetWidgetValue(i);
        }

        public FSetting GetWidgetValue(int i)
        {
            return widgetValues[i];
        }

        public void HighlightWidget(string widget, double time)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            HighlightWidget(i, time);
        }

        public void HighlightWidget(int i, double time)
        {
            highlightedWidgets.Add(i, time);
        }

        public void SetHighlightColor(string widget, Color value)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            SetHighlightColor(i, value);
        }

        public void SetHighlightColor(int i, Color value)
        {
            canvas.highlightColors[i] = value;
        }

        public void SetRoot(Rectangle root)
        {
            canvas.root = root;
        }

        public void SetTooltipText(string toolTip, string widget, string text)
        {
            if (tooltipCache.ContainsKey(toolTip))
            {
                FCanvas toolTipCanvas = tooltipCache[toolTip];

                for (int i = 0; i < toolTipCanvas.IDs.Length; i++)
                {
                    if (tooltipCache[toolTip].IDs[i] == widget)
                    {
                        toolTipCanvas.texts[i] = text;
                    }
                }
            }
        }

        public void SetWidgetColor(string widget, Color value)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            SetWidgetColor(i, value);
        }

        public void SetWidgetColor(int i, Color value)
        {
            canvas.colors[i] = value;
        }

        public void SetWidgetSource(string widget, Rectangle source)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            SetWidgetSource(i, source);
        }

        public void SetWidgetSource(int i, Rectangle source)
        {
            canvas.sources[i] = source;
        }

        public void SetWidgetText(string widget, string value)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            SetWidgetText(i, value);
        }

        public void SetWidgetText(int i, string value)
        {
            canvas.texts[i] = value;
        }

        public void SetWidgetValue(string widget, FSetting value)
        {
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
            SetWidgetValue(i, value);
        }

        public void SetWidgetValue(int i, FSetting value)
        {
            widgetValues[i] = value;
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
            if (!widgetNameIndexMap.ContainsKey(widget)) { return; }
            int i = widgetNameIndexMap[widget];
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

            int currentParent = 0;
            bool hovered = false;

            Rectangle view = content.GraphicsDevice.Viewport.Bounds;
            Rectangle root = GetMinRectangle(view, canvas.root);
            Rectangle anchoredRoot = GetAnchoredRectangle(canvas.root, root, canvas.anchor);

            for (int i = 0; i < canvas.parents.Length; i++)
            {
                Rectangle scissor = GetScissor(canvas.parents[i], currentParent, anchoredRoot, content.GraphicsDevice.Viewport.Bounds);
                Rectangle rect = CalculateWidgetPosition(canvas, root, i);

                rect.Offset(anchoredRoot.Location);

                if (!scissor.Contains(input.MousePosition)) { continue; }
                if (!rect.Contains(input.MousePosition)) { continue; }

                hovered = true;

                switch (canvas.widgetTypes[i])
                {
                    case EWidgetType.text:
                    case EWidgetType.panel:
                        break;

                    case EWidgetType.button:
                        DoHover(gameTime, frameHoveredWidgets, i);

                        InvokeBindings(gameTime, i);
                        break;

                    case EWidgetType.label:
                        break;

                    case EWidgetType.scrollbar:
                        DoHover(gameTime, frameHoveredWidgets, i);

                        if (suffix != ".Released")
                        {
                            if (DoScrollbarMovement(i, rect))
                            {
                                InvokeBindings(gameTime, i);
                            }
                        }
                        break;

                    case EWidgetType.slider:
                        DoHover(gameTime, frameHoveredWidgets, i);

                        if (suffix != ".Released")
                        {
                            if (DoSliderMovement(i, rect))
                            {
                                InvokeBindings(gameTime, i);
                            }
                        }
                        break;

                    case EWidgetType.tickbox:
                        DoHover(gameTime, frameHoveredWidgets, i);

                        if (suffix == ".OnEndClick")
                        {
                            ToggleTickBox(i);
                        }
                        InvokeBindings(gameTime, i);
                        break;

                    default:
                        break;
                }
            }

            hoveredWidgets = frameHoveredWidgets;

            if (hovered != bIsHovered)
            {
                if (hovered)
                {
                    AInputComponent.PushFocus(focus);
                }
                else
                {
                    AInputComponent.PopFocus(focus);
                }
                bIsHovered = hovered;
            }
        }
    }
}