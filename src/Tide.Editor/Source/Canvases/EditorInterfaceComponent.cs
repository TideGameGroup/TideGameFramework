using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Tide.Core;
using Tide.Tools;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public struct FEditorInterfaceConstructorArgs
    {
        public UContentManager content;
        public TInput input;
        public TWindow window;
    }

    public class UEditorInterface : UComponent, IUpdateComponent
    {
        private readonly UContentManager content;

        private FCanvas? _newcanvas = null;

        private string openFilePath = "";

        public UEditorInterface(FEditorInterfaceConstructorArgs args)
        {
            NullCheck(args.input);
            TrySetDefault(args.content, out content);

            InputComponent = new AInputComponent(args.input);

            // top bar
            FCanvasComponentConstructorArgs canvasArgs =
                new FCanvasComponentConstructorArgs
                {
                    audio = null,
                    canvas = content.Load<FCanvas>("UI"),
                    content = content,
                    focus = EFocus.Cinematic | EFocus.GameUI,
                    input = args.input,
                    scale = 1f,
                    window = args.window.window
                };

            EditorCanvasComponent = new ACanvasComponent(canvasArgs);

            // top bar draw
            FCanvasDrawComponentConstructorArgs canvasRenderArgs =
                new FCanvasDrawComponentConstructorArgs
                {
                    component = EditorCanvasComponent,
                    content = content
                };

            EditorDrawComponent = new ACanvasDrawComponent(canvasRenderArgs);

            // dynamic canvas

            DynamicCanvasComponent = new DynamicCanvasComponent();

            // tree canvas
            EditorTreeCanvasComponentConstructorArgs treeArgs =
                new EditorTreeCanvasComponentConstructorArgs
                {
                    content = content,
                    input = args.input,
                    window = args.window.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            TreeCanvasComponent = new EditorTreeCanvasComponent(treeArgs);

            // property canvas
            EditorPropertiesCanvasComponentConstructorArgs propertiesArgs =
                new EditorPropertiesCanvasComponentConstructorArgs
                {
                    content = content,
                    input = args.input,
                    window = args.window.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            PropertiesCanvasComponent = new EditorPropertiesCanvasComponent(propertiesArgs);

            // preview canvas
            EditorPreviewCanvasComponentConstructorArgs previewArgs =
                new EditorPreviewCanvasComponentConstructorArgs
                {
                    content = content,
                    input = args.input,
                    window = args.window.window,
                    dynamicCanvasComponent = DynamicCanvasComponent
                };

            PreviewCanvasComponent = new EditorPreviewCanvasComponent(previewArgs);

            AddChildComponent(InputComponent);
            AddChildComponent(PreviewCanvasComponent);
            AddChildComponent(EditorCanvasComponent);
            AddChildComponent(EditorDrawComponent);
            AddChildComponent(DynamicCanvasComponent);
            AddChildComponent(TreeCanvasComponent);
            AddChildComponent(PropertiesCanvasComponent);

            SetupBindings(EditorCanvasComponent);
        }

        // panels
        public DynamicCanvasComponent DynamicCanvasComponent { get; private set; }

        public ACanvasComponent EditorCanvasComponent { get; private set; }
        public ACanvasDrawComponent EditorDrawComponent { get; private set; }
        public AInputComponent InputComponent { get; private set; }
        public EditorPreviewCanvasComponent PreviewCanvasComponent { get; private set; }
        public EditorPropertiesCanvasComponent PropertiesCanvasComponent { get; private set; }
        public EditorTreeCanvasComponent TreeCanvasComponent { get; private set; }

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

        private void OpenXMLFile()
        {
            string filePath = OpenFileDialog();
            if (filePath != "")
            {
                FVersioningInfo res = Versioning.CheckVersioning(filePath, out XDocument xml); 
                if (res.result == EVersioningResult.ESUCCESS)
                {
                    Versioning.DoConversionChain(res, ref xml);
                    xml.Save(filePath);

                    string projectDir = ProjectSourcePath.Path + "Content";
                    UImportTools.ImportSerialisedData(projectDir, filePath, out _newcanvas);
                }
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

        private bool OpenFile<T>(out T file, out string filePath)
        {
            filePath = OpenFileDialog();
            if (filePath != "")
            {
                string projectDir = ProjectSourcePath.Path + "Content";
                return UImportTools.ImportSerialisedData(projectDir, filePath, out file);
            }
            file = default;
            return false;
        }

        private void SaveFile()
        {
            if (DynamicCanvasComponent.DynamicCanvas == null) { return; }

            if (openFilePath == "")
            {
                SaveFileAs();
            }
            else
            {
                UExportTools.ExportSerialisedInstanceData(openFilePath, DynamicCanvasComponent.DynamicCanvas.AsCanvas());
            }
        }

        private void SaveFileAs()
        {
            if (DynamicCanvasComponent.DynamicCanvas == null) { return; }

            openFilePath = OpenSaveDialog();
            if (openFilePath != "")
            {
                SaveFile();
            }
        }

        public void SetupBindings(ACanvasComponent component)
        {
            component.BindAction("open.OnReleased", (gt) => { OpenXMLFile(); });
            component.BindAction("save.OnReleased", (gt) => { SaveFile(); });
            component.BindAction("saveas.OnReleased", (gt) => { SaveFileAs(); });
            component.BindAction("new.OnReleased", (gt) => { DynamicCanvasComponent.New(); });
            component.BindAction("undo.OnReleased", (gt) => { DynamicCanvasComponent.Undo(); });
            component.BindAction("redo.OnReleased", (gt) => { DynamicCanvasComponent.Redo(); });
            component.BindAction("newimage.OnReleased", (gt) => { OpenTextureFile(); });
            component.BindAction("newfont.OnReleased", (gt) => { OpenFontFile(); });
        }

        public void Update(GameTime gameTime)
        {
            if (_newcanvas != null)
            {
                DynamicCanvasComponent.Set(new FDynamicCanvas((FCanvas)_newcanvas));
                _newcanvas = null;
            }
        }
    }
}