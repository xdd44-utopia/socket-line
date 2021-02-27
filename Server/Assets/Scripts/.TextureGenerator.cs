using UnityEditor;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
	private static Color voidColor = new Color(0.0862f, 0.5529f, 0.2039f, 0f);
	private static Color earthColor = new Color(0.0862f, 0.5529f, 0.2039f, 1f);
	private static Color crustColor = new Color(0.5647f, 0.4901f, 0.4196f, 1f);
	private static Color mantleColor = new Color(0.9725f, 0.2509f, 0.0156f, 1f);
	private static Color outercoreColor = new Color(0.9764f, 0.5215f, 0f, 1f);
	private static Color innercoreColor = new Color(0.9607f, 0.7529f, 0.2862f, 1f);

	[MenuItem("CreateTexture/3DTexture")]
	public static void CreateTexture3D()
	{
		Debug.Log("generating");
		// Configure the texture
		int size = 256;
		TextureFormat format = TextureFormat.RGBA32;
		TextureWrapMode wrapMode =  TextureWrapMode.Clamp;

		// Create the texture and apply the configuration
		Texture3D texture = new Texture3D(size, size, size, format, false);
		texture.wrapMode = wrapMode;

		// Create a 3-dimensional array to store color data
		Color[] colors = new Color[size * size * size];

		// Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
		float inverseResolution = 1.0f / (size - 1.0f);
		for (int z = 0; z < size; z++)
		{
			int zOffset = z * size * size;
			for (int y = 0; y < size; y++)
			{
				int yOffset = y * size;
				for (int x = 0; x < size; x++)
				{
					//float radius = (x - size / 2) * (x - size / 2) + (y - size / 2) * (y - size / 2) + (z - size / 2) * (z - size / 2);
					float radius = Mathf.Max((x - size / 2) * (x - size / 2), (y - size / 2) * (y - size / 2), (z - size / 2) * (z - size / 2));
					radius = Mathf.Sqrt(radius);
					if (radius < 25f) {
						colors[x + yOffset + zOffset] = innercoreColor;
					}
					else if (radius < 50f) {
						colors[x + yOffset + zOffset] = outercoreColor;
					}
					else if (radius < 75f) {
						colors[x + yOffset + zOffset] = mantleColor;
					}
					else if (radius < 100f) {
						colors[x + yOffset + zOffset] = crustColor;
					}
					else if (radius < 110f) {
						colors[x + yOffset + zOffset] = earthColor;
					}
					else {
						colors[x + yOffset + zOffset] = voidColor;
					}
				}
			}
		}

		// Copy the color values to the texture
		texture.SetPixels(colors);

		// Apply the changes to the texture and upload the updated texture to the GPU
		texture.Apply();        

		// Save the texture to your Unity Project
		AssetDatabase.CreateAsset(texture, "Assets/Material/EarthCube.asset");
	}

}