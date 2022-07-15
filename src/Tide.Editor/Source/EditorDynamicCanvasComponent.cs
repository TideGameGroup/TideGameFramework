using System;
using System.Collections.Generic;
using System.Text;
using Tide.Core;
using Tide.Tools;

namespace Tide.Editor
{
    public delegate void DynamicCanvasDelegate();

    public struct EditorDynamicCanvasComponentConstructorArgs
    {

    }

    public class EditorDynamicCanvasComponent : UComponent
    {
        public int selection = 0;

        public EditorDynamicCanvasComponent()
        {

        }

        public FDynamicCanvas DynamicCanvas { get; private set; }
        public DynamicCanvasDelegate OnDynamicCanvasUpdated { get; set; }
        public DynamicCanvasDelegate OnDynamicCanvasSet { get; set; }
        public DynamicCanvasDelegate OnSelectionUpdated { get; set; }

        public void Refresh()
        {
            OnDynamicCanvasUpdated.Invoke();
        }

        public void Rebuild()
        {
            OnDynamicCanvasSet.Invoke();
        }

        public void Set(FDynamicCanvas dynamicCanvas)
        {
            DynamicCanvas = dynamicCanvas;
            OnDynamicCanvasSet.Invoke();
        }

        public void SetSelection(int i)
        {
            selection = i;
            OnSelectionUpdated.Invoke();
        }

        internal void New()
        {
            Set(new FDynamicCanvas("new_canvas"));
        }
    }
}
