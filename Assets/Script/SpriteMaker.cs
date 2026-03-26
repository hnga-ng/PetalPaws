using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteMaker : MonoBehaviour
{
    public Camera targetCamera;
    public int resWidth = 512;
    public int resHeight = 512;
    public string fileName = "NewSprite";
    public string folderName = "UI";

    [ContextMenu("Take Screenshot")]
    public void TakeScreenshot()
    {
        // 1. Setup Folder Path
        string folderPath = Application.dataPath + "/" + folderName;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 2. Setup Render Texture
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24, RenderTextureFormat.ARGB32);
        targetCamera.targetTexture = rt;

        // 3. Render and Capture
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
        targetCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

        // Clean up memory
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        // 4. Save to Disk
        byte[] bytes = screenShot.EncodeToPNG();
        string localPath = "Assets/" + folderName + "/" + fileName + ".png";
        string fullPath = Application.dataPath + "/" + folderName + "/" + fileName + ".png";
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"<color=green>Saved:</color> {localPath}");

        // 5. AUTO-FORMAT AS SPRITE (Editor Only)
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(localPath);
        TextureImporter importer = AssetImporter.GetAtPath(localPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite; // Set to Sprite (2D and UI)
            importer.spriteImportMode = SpriteImportMode.Single; // This prevents the 'Multiple' bug
            importer.alphaIsTransparency = true;              // Ensure transparency is clean
            importer.mipmapEnabled = false;                   // UI usually doesn't need mipmaps
            importer.SaveAndReimport();
        }
#endif
    }
}