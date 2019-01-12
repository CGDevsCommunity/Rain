using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleTextureGenerator
{
    private int _TextureSize;
    private Texture2D _Texture;
    //r - offsetX, g - r - offsetY, b - wave max length, a - frequency
    public Texture2D GeneratePoleTexture(Pole[] poles,  float meshSize = 1)
    {
        _Texture = _Texture == null ? new Texture2D(_TextureSize, _TextureSize) : _Texture;
 
        for (int x = 0; x < _TextureSize; x++)
        {
            for (int y = 0; y < _TextureSize; y++)
            {
                _Texture.SetPixel(x, y, Color.clear);
                for (int k = 0; k < poles.Length; k++)
                {
                    Vector2 texPos = new Vector2((float) x / (_TextureSize - 1),
                        (float) y / (_TextureSize - 1));

                    var pole = poles[k];
                    float distance = Vector2.Distance(texPos, pole.Position);
                    if (distance <= pole.Radius)
                    {
                        _Texture.SetPixel(x, y,
                            new Color(pole.Position.x, pole.Position.y, pole.Radius - distance, pole.Phase));
                    }
                }
              
            }
        }
        _Texture.Apply();
        return _Texture;
    }

    public PoleTextureGenerator(int textureSize = 128)
    {
        _TextureSize = textureSize;
    }
}
