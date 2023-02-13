using UnityEngine;
using UnityEditor;

//SAVING FILE FROM EDITOR TO "TEXTURES/2D MAPS"
//You can change here every value you want

public class TexturePostProcessor : AssetPostprocessor
{
	/*void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = assetImporter as TextureImporter;
		importer.anisoLevel = 0;
		importer.filterMode = FilterMode.Point; //Point (No filter mode)

		if (importer.assetPath.IndexOf("Textures/2D Maps") != -1) { //save it to Textures/2D Maps
			// importer.spritePixelsPerUnit = 1;
		//	importer.maxTextureSize = 2048;
		//	importer.alphaSource = TextureImporterAlphaSource.FromInput;
			importer.alphaIsTransparency = true;
		//	importer.wrapMode = TextureWrapMode.Clamp;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			// importer.compressionQuality = 0;
		//	importer.npotScale = TextureImporterNPOTScale.None;
			importer.textureType = TextureImporterType.Sprite; //Maps must be in Sprite 2D
		//	importer.mipmapEnabled = false;
			importer.isReadable = true;
			// Debug.Log("!");
		}

		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));

		if (asset) {
			EditorUtility.SetDirty(asset);
		} else {
			texture.anisoLevel = 0;
			texture.filterMode = FilterMode.Point;  //Point (No filter mode)
		} 
	}*/


	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = assetImporter as TextureImporter;
		importer.anisoLevel = 0;
		importer.filterMode = FilterMode.Point; //Point (No filter mode)

		if (importer.assetPath.IndexOf("Textures/2D Maps") != -1) { //save it to Textures/2D Maps
			importer.textureType = TextureImporterType.Sprite;
			importer.isReadable = true;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
		}

		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));

		if (asset) {
			EditorUtility.SetDirty(asset);
		} else {
			texture.anisoLevel = 0;
			texture.filterMode = FilterMode.Point; //Point (No filter mode)
		}
		AssetDatabase.Refresh();
	}


}