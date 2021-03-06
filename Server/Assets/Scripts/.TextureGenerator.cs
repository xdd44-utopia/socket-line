using UnityEditor;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
	private static int thickness = 3;
	private static int interval = 40;

	[MenuItem("CreateTexture/3DTexture")]
	public static void CreateTexture3D()
	{
		string[] pixels = System.IO.File.ReadAllLines(@"/Users/xdd44/Documents/SCM/socket-line/textureConverter/bin/data/colors.txt");
		Debug.Log("generating");
		// Configure the texture
		int size = 256;
		int depth = 317;
		TextureFormat format = TextureFormat.RGBA32;
		TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

		// Create the texture and apply the configuration
		Texture3D texture = new Texture3D(size, size, depth, format, false);
		texture.wrapMode = wrapMode;

		// Create a 3-dimensional array to store color data
		Color[] colors = new Color[size * size * depth];

		// Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
		float inverseResolution = 1.0f / (size - 1.0f);
		for (int z = 0; z < depth; z++)
		{
			Debug.Log(z + "th image");
			int zOffset = z * size * size;
			for (int y = 0; y < size; y++)
			{
				int yOffset = y * size;
				for (int x = 0; x < size; x++)
				{
					colors[x + yOffset + zOffset] = converter(pixels[x + yOffset + zOffset]);
					if ((((x + interval / 2) % interval < thickness && (y + interval / 2) % interval < thickness) ||
						((x + interval / 2) % interval < thickness && (z + interval / 2) % interval < thickness) ||
						((y + interval / 2) % interval < thickness && (z + interval / 2) % interval < thickness)) &&
						x > thickness && y > thickness && z > thickness
						) {
						colors[x + yOffset + zOffset] = new Color(0.9f, 0.9f, 0.9f, 0.49f);
					}
				}
			}
		}

		// Copy the color values to the texture
		texture.SetPixels(colors);

		// Apply the changes to the texture and upload the updated texture to the GPU
		texture.Apply();        

		// Save the texture to your Unity Project
		AssetDatabase.CreateAsset(texture, "Assets/Material/HeadGrid.asset");
	}

	static Color converter(string str) {
		str = str.Substring(1);
		str = str.Remove(str.Length - 1);
		string[] temp = str.Split(',');
		float r = System.Convert.ToSingle(temp[0]);
		float g = System.Convert.ToSingle(temp[1]);
		float b = System.Convert.ToSingle(temp[2]);
		float a = System.Convert.ToSingle(temp[3]);
		Color clr = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		return clr;
	}

}