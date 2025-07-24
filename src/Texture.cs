using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Zpg
{
    public class TextureSetting : Dictionary<TextureParameterName, int>
    {
        public static readonly TextureSetting Default = new TextureSetting()
        {
            {TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat },
            {TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat },
            {TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear },
            {TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear },
        };
    }
    public class Texture
    {
        public int texID;
        public Texture(string filename, TextureSetting? settings = null)
        {
            settings ??= TextureSetting.Default;
            LoadFromFile(filename, settings);
        }

        private void LoadFromFile(string filename, TextureSetting? settings = null)
        {
            // Nastavíme překlopení obrázku
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Načteme obrázek z disku
            using (FileStream fs = File.OpenRead(filename))
            {
                ImageResult image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);

                // Načteme data obrázku
                LoadData(image.Width, image.Height, image.Data, settings);
            }
        }

        private TextureSetting LoadData(int width, int height, byte[] data, TextureSetting? settings = null)
        {
            settings ??= TextureSetting.Default;

            // Vytvoříme texturu v OpenGL
            texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texID);

            // Nahrajeme data textury do OpenGL
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Nastavení parametrů textury (Wrap, Filter)
            foreach (var setting in settings)
            {
                GL.TexParameter(TextureTarget.Texture2D, setting.Key, setting.Value);
            }

            // Odpojení bindu
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return settings;
        }

        public void Bind(int shaderLocation, int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.Uniform1(shaderLocation, textureUnit);
        }
    }
}