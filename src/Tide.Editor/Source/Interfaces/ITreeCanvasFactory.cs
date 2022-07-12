using System;
using System.Collections.Generic;
using System.Text;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    interface ITreeCanvasFactory
    {
        public FDynamicCanvas GetDynamicCanvas();

        public FCanvas GetCanvas();
    }
}
