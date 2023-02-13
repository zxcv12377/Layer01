using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour {

	public Texture2D map;

	public ColorSpawn[] colorMappings;

	public Text text;

	public Tilemap platformtilemap;
	public Tilemap spikeTilemap;

	public string mappingfile;

	void Start ()
	{
		CreateLevel ();
	}

	//Spawn tiles in width and height
	void CreateLevel()
	{
		for (int x = 0; x < map.width; x++)
		{
			for (int y = 0; y < map.height; y++)
			{
				CreateTile (x, y);
			}
		}
	}

	//Create new tiles function
	void CreateTile (int x, int y)
	{

		//Take the pixel and store it in pixel color
		Color pixelColor = map.GetPixel (x, y);

		if (pixelColor.a == 0) {
			// If The pixel is transparent. Generator will ignore it
			return;
		}

		//Show pixel colors in Text input but... it's not impemented into Unity YET
		//text = pixelColor;

		foreach (ColorSpawn colorMapping in colorMappings) {

			if (ColorUtility.ToHtmlStringRGB(colorMapping.color) == ColorUtility.ToHtmlStringRGB(pixelColor))
			{
				if (colorMapping.ruleTile == null)
				{
					Vector3Int position = new Vector3Int(x, y, 0);
					spikeTilemap.SetTile(position, colorMapping.tile);
				}
				else if (colorMapping.ruleTile != null)
                {
					Vector3Int position = new Vector3Int(x, y, 0);
					platformtilemap.SetTile(position, colorMapping.ruleTile);
				}
                else
                {
					Vector2 position = new Vector2(x, y);
					Instantiate(colorMapping.ornament, position, Quaternion.identity, transform);
				}
			}
		}
	}

}
