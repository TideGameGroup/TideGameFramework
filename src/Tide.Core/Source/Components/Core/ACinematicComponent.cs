﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void CinematicDelegate(GameTime gameTime);

    public class ACinematicComponent : UComponent, IUpdateComponent, ISerialisableComponent
    {
        private string currentCinematic = "";
        protected Dictionary<string, ACanvasComponent> canvases = new Dictionary<string, ACanvasComponent>();
        protected string currentFunction = "";
        protected int currentPage = -1;
        protected int deferredPage = 0;
        protected Dictionary<string, CinematicDelegate> functions = new Dictionary<string, CinematicDelegate>();
        public List<string> bindings = new List<string>();
        public List<bindingType> bindingTypes = new List<bindingType>();
        public List<bool> bLocksInput = new List<bool>();
        public List<List<string>> highlights = new List<List<string>>();
        public List<List<Vector2>> positions = new List<List<Vector2>>();
        public List<string> texts = new List<string>();

        public ACinematicComponent(
            UContentManager content,
            TInput input,
            int page = -1,
            string canvas = "Cinematic"
            )
        {
            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = content.Load<FCanvas>(canvas),
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = input,
                    scale = 1f
                };

            CinematicCanvas = new ACanvasComponent(canvasArgs);

            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = CinematicCanvas,
                    content = content
                };

            CinematicCanvasRenderer = new ACanvasDrawComponent(canvasRenderArgs);

            AddChildComponent(CinematicCanvas);
            AddChildComponent(CinematicCanvasRenderer);

            canvases.Add("self", CinematicCanvas);

            currentPage = page;
            deferredPage = page;

            if (currentPage == -1)
            {
                CinematicCanvas.IsActive = false;
                CinematicCanvas.IsVisible = false;
                CinematicCanvasRenderer.IsActive = false;
                CinematicCanvasRenderer.IsVisible = false;
            }
        }

        /// <summary>
        public string CachedHash { get; set; }

        public ACanvasComponent CinematicCanvas { get; private set; }
        public ACanvasDrawComponent CinematicCanvasRenderer { get; private set; }

        private void BindCinematic(int page, double time)
        {
            if (bLocksInput[page])
            {
                CinematicCanvas.InputComponent.PushFocus(EFocus.Cinematic);
            }

            foreach (string key in canvases.Keys)
            {
                canvases[key].IsActive = false;
            }

            foreach (var highlight in highlights[page])
            {
                if (highlight == "") { continue; }

                string[] canvas_widget = highlight.Split(".", 2);
                canvases[canvas_widget[0]].HighlightWidget(canvas_widget[1], time);
                canvases[canvas_widget[0]].IsActive = true;
            }

            switch (bindingTypes[page])
            {
                case bindingType.CANVAS:
                    string[] binding = bindings[page].Split(".", 2);
                    if (binding.Length == 2 && canvases.ContainsKey(binding[0]))
                    {
                        canvases[binding[0]].BindAction(binding[1], GoNext);
                        canvases[binding[0]].IsActive = true;
                    }
                    break;

                case bindingType.FUNCTION:
                    currentFunction = bindings[page];
                    break;

                default:
                    break;
            }
        }

        private void UnbindCinematicBindings(int page)
        {
            foreach (string key in canvases.Keys)
            {
                canvases[key].IsActive = true;
            }

            foreach (var highlight in highlights[page])
            {
                if (highlight == "") { continue; }

                string[] canvas_widget = highlight.Split(".", 2);
                canvases[canvas_widget[0]].UnHighlightWidget(canvas_widget[1]);
            }

            switch (bindingTypes[page])
            {
                case bindingType.CANVAS:
                    string[] binding = bindings[page].Split(".", 2);
                    if (binding.Length == 2 && canvases.ContainsKey(binding[0]))
                    {
                        canvases[binding[0]].UnbindAction(binding[1], GoNext);
                    }
                    break;

                case bindingType.FUNCTION:
                    currentFunction = "";
                    break;

                default:
                    break;
            }

            if (bLocksInput[page])
            {
                CinematicCanvas.InputComponent.PopFocus(EFocus.Cinematic);
            }
        }

        public int GetCurrentPage()
        {
            return currentPage;
        }

        public void GoBack(GameTime gameTime)
        {
            deferredPage--;
        }

        public void GoNext(GameTime gameTime)
        {
            deferredPage++;
        }

        public bool IsPlaying()
        {
            return currentPage != -1 && currentPage < bLocksInput.Count;
        }

        public void RegisterCanvas(string name, ACanvasComponent canvas)
        {
            canvases.TryAdd(name, canvas);
        }

        public void RegisterCanvases(Dictionary<string, ACanvasComponent> dict)
        {
            foreach (string key in dict.Keys)
            {
                RegisterCanvas(key, dict[key]);
            }
        }

        public void RegisterFunction(string name, CinematicDelegate func)
        {
            functions.TryAdd(name, func);
        }

        public void RunFunctionByName(string name, GameTime gt)
        {
            if (name != "")
            {
                functions[name].Invoke(gt);
            }
        }

        public void UnregisterCanvas(string name)
        {
            canvases.Remove(name);
        }

        #region ISerialisableComponent

        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "" || serialisedScriptPath == null)
            {
                IsActive = false;
                IsVisible = false;
                return;
            }

            FCinematic cinematic = content.Load<FCinematic>(serialisedScriptPath);

            bLocksInput = new List<bool>(cinematic.bLocksInput);
            texts = new List<string>(cinematic.texts);
            bindingTypes = new List<bindingType>(cinematic.bindingType);
            bindings = new List<string>(cinematic.bindings);

            highlights.Clear();
            positions.Clear();

            foreach (var array in cinematic.highlights)
            {
                highlights.Add(new List<string>(array));
            }

            foreach (var array in cinematic.positions)
            {
                positions.Add(new List<Vector2>(array));
            }

            CinematicCanvas.IsActive = true;
            CinematicCanvas.IsVisible = true;
            CinematicCanvasRenderer.IsActive = true;
            CinematicCanvasRenderer.IsVisible = true;
            currentPage = 0;
            deferredPage = 0;
            BindCinematic(currentPage, 0.0);
            CinematicCanvas.SetWidgetText("text", texts[currentPage]);
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);

            serialisedSet.Add(ID,
                new FCinematic
                {
                    texts = texts.ToArray(),
                    bindingType = bindingTypes.ToArray(),
                    bindings = bindings.ToArray()
                }
            );
            return ID;
        }

        public void TryLoadCinematic(UContentManager content, string serialisedScriptPath)
        {
            if (currentPage > -1 && currentCinematic == serialisedScriptPath)
            {
                currentPage = 0;
                deferredPage = 0;
                BindCinematic(currentPage, 0.0);
            }
            else
            {
                Load(content, serialisedScriptPath);
                currentCinematic = serialisedScriptPath;
            }
        }

        #endregion ISerialisableComponent

        #region IUpdateComponent

        public void Update(GameTime gameTime)
        {
            if (currentPage >= 0 && currentPage != deferredPage)
            {
                UnbindCinematicBindings(currentPage);

                currentPage = deferredPage;

                if (currentPage < texts.Count)
                {
                    BindCinematic(currentPage, gameTime.TotalGameTime.TotalSeconds);

                    if (texts[currentPage] == null || texts[currentPage] == "")
                    {
                        CinematicCanvas.IsVisible = false;
                        CinematicCanvas.IsActive = false;
                        CinematicCanvasRenderer.IsVisible = false;
                        CinematicCanvasRenderer.IsActive = false;
                    }
                    else
                    {
                        CinematicCanvas.IsVisible = true;
                        CinematicCanvas.IsActive = true;
                        CinematicCanvasRenderer.IsVisible = true;
                        CinematicCanvasRenderer.IsActive = true;
                        CinematicCanvas.SetWidgetText("text", texts[currentPage]);
                    }
                }
            }

            if (currentFunction != "")
            {
                RunFunctionByName(currentFunction, gameTime);
            }
        }

        #endregion IUpdateComponent
    }
}