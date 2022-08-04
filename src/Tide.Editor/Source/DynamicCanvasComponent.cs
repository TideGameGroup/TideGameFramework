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

    public class DynamicCanvasComponent : UComponent
    {
        public int selection = 0;

        private int dynamicCanvasActiveIndex = 0;
        private List<FDynamicCanvas> dynamicCanvasEditStack = new List<FDynamicCanvas>();

        public DynamicCanvasComponent()
        {
            dynamicCanvasEditStack.Add(null);
        }

        public FDynamicCanvas DynamicCanvas => dynamicCanvasEditStack[dynamicCanvasActiveIndex];
        public DynamicCanvasDelegate OnDynamicCanvasUpdated { get; set; }
        public DynamicCanvasDelegate OnDynamicCanvasSet { get; set; }
        public DynamicCanvasDelegate OnSelectionUpdated { get; set; }

        public void Refresh()
        {
            if (DynamicCanvas != null)
            {
                OnDynamicCanvasUpdated.Invoke();
                AddUndoStep(DynamicCanvas);
            }
        }

        public void Rebuild()
        {
            if (DynamicCanvas != null)
            {
                OnDynamicCanvasSet.Invoke();
                AddUndoStep(DynamicCanvas);
            }
        }

        public void Set(FDynamicCanvas dynamicCanvas)
        {
            AddUndoStep(dynamicCanvas);
            OnDynamicCanvasSet.Invoke();
        }

        private void AddUndoStep(FDynamicCanvas dynamicCanvas)
        {
            // remove excess
            while (dynamicCanvasEditStack.Count - 1 > dynamicCanvasActiveIndex)
            {
                dynamicCanvasEditStack.RemoveAt(dynamicCanvasEditStack.Count - 1);
            }

            //if (dynamicCanvasEditStack.Count > 64)
            //{
                //dynamicCanvasEditStack.RemoveAt(0);
            //}

            // add new
            dynamicCanvasEditStack.Add(new FDynamicCanvas(dynamicCanvas.AsCanvas()));
            dynamicCanvasActiveIndex += 1;
        }

        public void Undo()
        {
            dynamicCanvasActiveIndex = Math.Max(0, dynamicCanvasActiveIndex - 1);
            if (DynamicCanvas != null)
            {
                OnDynamicCanvasSet.Invoke();
            }
        }

        public void Redo()
        {
            dynamicCanvasActiveIndex = Math.Min(dynamicCanvasActiveIndex + 1, dynamicCanvasEditStack.Count - 1);
            if (DynamicCanvas != null)
            {
                OnDynamicCanvasSet.Invoke();
            }
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
