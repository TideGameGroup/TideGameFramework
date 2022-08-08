using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public interface IViewComponent
    {
        public Matrix ViewMatrix { get; set; }
        public Matrix Projection { get; set; }
        public Matrix ViewProjectionMatrix { get; set; }
    }
}
