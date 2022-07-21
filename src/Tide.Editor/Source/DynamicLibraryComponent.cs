using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tide.Core;
using Tide.Tools;

namespace Tide.Editor
{
    public struct EditorDynamicLibraryComponentConstructorArgs
    {
        public UContentManager content;
    }

    public class DynamicLibraryComponent : UComponent
    {
        public DynamicLibraryComponent()
        {

        }

        private string OpenFileDialog()
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        private string OpenSaveDialog()
        {
            using SaveFileDialog openFileDialog = new SaveFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        /*
        private void OpenXMLFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                string projectDir = ProjectSourcePath.Path + "Content";
                UImportTools.ImportSerialisedData(projectDir, filePath, out _newcanvas);
            }
        }

        private void OpenTextureFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                string filename = Path.GetFileNameWithoutExtension(filePath);
                content.DynamicLibrary[filename] = Texture2D.FromFile(content.GraphicsDevice, filePath);
            }
        }

        private void OpenFontFile()
        {
            if (OpenFile(out SpriteFontContent file, out string filePath))
            {
                string filename = Path.GetFileNameWithoutExtension(filePath);

                var mipchain = file.Texture.Faces[0][0];

                Texture2D texture = new Texture2D(content.GraphicsDevice, mipchain.Width, mipchain.Height, false, SurfaceFormat.Alpha8);
                Rectangle bounds = new Rectangle(0, 0, mipchain.Width, mipchain.Height);
                byte[] data = mipchain.GetPixelData();
                texture.SetData(0, bounds, data, 0, mipchain.Width * mipchain.Height);

                SpriteFont spriteFont = new SpriteFont
                    (
                    texture,
                    file.Glyphs,
                    file.Cropping,
                    file.CharacterMap,
                    file.VerticalLineSpacing,
                    file.HorizontalSpacing,
                    file.Kerning,
                    file.DefaultCharacter
                    );
                content.DynamicLibrary[filename] = spriteFont;
            }
        }
        */

    }
}
