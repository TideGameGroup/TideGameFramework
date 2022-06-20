﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void CinematicDelegate(GameTime gameTime);

    public class ACinematicComponent : UComponent, IUpdateComponent, ISerialisableComponent
    {
        protected readonly AInputComponent input;
        public ACanvasComponent CinematicCanvas { get; private set; }

        protected Dictionary<string, ACanvasComponent> canvases            = new Dictionary<string, ACanvasComponent>();
        protected Dictionary<string, CinematicDelegate> functions = new Dictionary<string, CinematicDelegate>();

        public List<bool> bLocksInput                  = new List<bool>();
        public List<string> texts                      = new List<string>();
        public List<bindingType> bindingTypes          = new List<bindingType>();
        public List<string> bindings                   = new List<string>();
        public List<List<string>> highlights           = new List<List<string>>();
        public List<List<Vector3>> positions           = new List<List<Vector3>>();

        protected string currentFunction = "";
        protected int currentPage  = -1;
        protected int deferredPage = 0;

        public ACinematicComponent(
            UContentManager content,
            UInput input,
            int page = -1
            )
        {
            this.input = new AInputComponent(input); 

            CinematicCanvas = new ACanvasComponent(content, this.input, content.Load<FCanvas>("Cinematic"), EFocus.Cinematic | EFocus.GameUI);
            canvases.Add("self", CinematicCanvas);

            currentPage  = page;
            deferredPage = page;

            if (currentPage == -1)
            {
                CinematicCanvas.bIsActive = false;
                CinematicCanvas.bIsVisible = false;
            }
        }

        public void GoNext(GameTime gameTime)
        {
            deferredPage++;
        }
        public void GoBack(GameTime gameTime)
        {
            deferredPage--;
        }

        public bool IsPlaying()
        {
            return currentPage != -1 && currentPage < bLocksInput.Count;
        }

        private void BindCinematic(int page, double time)
        {
            if (bLocksInput[page])
            {
                AInputComponent.PushFocus(EFocus.Cinematic);
            }

            foreach (string key in canvases.Keys)
            {
                canvases[key].IsEnabled = false;
            }

            foreach (var highlight in highlights[page])
            {
                if (highlight == "") { continue; }

                string[] canvas_widget = highlight.Split(".", 2);
                canvases[canvas_widget[0]].HighlightWidget(canvas_widget[1], time);
                canvases[canvas_widget[0]].IsEnabled = true;
            }

            switch (bindingTypes[page])
            {
                case bindingType.CANVAS:
                    string[] binding = bindings[page].Split(".", 2);
                    if (binding.Length == 2 && canvases.ContainsKey(binding[0]))
                    {
                        canvases[binding[0]].BindAction(binding[1], GoNext); 
                        canvases[binding[0]].IsEnabled = true;
                    }
                    break;

                case bindingType.FUNCTION:
                    currentFunction = bindings[page];
                    break;

                default:
                    break;
            }
        }

        public int GetCurrentPage()
        {
            return currentPage;
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

        private void UnbindCinematicBindings(int page)
        {
            foreach (string key in canvases.Keys)
            {
                canvases[key].IsEnabled = true;
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
                AInputComponent.PopFocus(EFocus.Cinematic);
            }
        }

        public void UnregisterCanvas(string name)
        {
            canvases.Remove(name);
        }

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
                        CinematicCanvas.bIsVisible = false;
                        CinematicCanvas.bIsActive  = false;
                    }
                    else
                    {
                        CinematicCanvas.bIsVisible = true;
                        CinematicCanvas.bIsActive  = true;
                        CinematicCanvas.SetWidgetText("text", texts[currentPage]);
                    }
                }
                else
                {
                    UnregisterChildComponent(CinematicCanvas);
                    //currentPage = -1;
                }
            }

            if (currentFunction != "")
            {
                functions[currentFunction].Invoke(gameTime);
            }
        }

        /// <summary>

        private string currentCinematic = "";

        public void TryLoadCinematic(UContentManager content, string serialisedScriptPath)
        {
            if (currentPage > -1 && currentCinematic == serialisedScriptPath)
            {
                currentPage = 0;
                deferredPage = 0;
                RegisterChildComponent(CinematicCanvas);
                BindCinematic(currentPage, 0.0);
            }
            else
            {
                Load(content, serialisedScriptPath);
                RegisterChildComponent(CinematicCanvas);
                currentCinematic = serialisedScriptPath;
            }
        }

        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "" || serialisedScriptPath == null)
            {
                bIsActive   = false;
                bIsVisible  = false;
                UnregisterChildComponent(CinematicCanvas);
                return;
            }

            FCinematic cinematic = content.Load<FCinematic>(serialisedScriptPath);

            bLocksInput     = new List<bool>(cinematic.bLocksInput);
            texts           = new List<string>(cinematic.texts);
            bindingTypes    = new List<bindingType>(cinematic.bindingType);
            bindings        = new List<string>(cinematic.bindings);

            highlights.Clear();
            positions.Clear();

            foreach (var array in cinematic.highlights)
            {
                highlights.Add(new List<string>(array));
            }

            foreach (var array in cinematic.positions)
            {
                positions.Add(new List<Vector3>(array));
            }

            CinematicCanvas.bIsActive = true;
            CinematicCanvas.bIsVisible = true;
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
    }
}