using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class Texture3DLoader : EditorWindow
{
    public TextAsset asset;
    public Material mat;

    [MenuItem("Windows/Texture3DLoader")]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(Texture3DLoader));
    }

    private void OnGUI()
    {
        asset = EditorGUILayout.ObjectField(asset, typeof(TextAsset)) as TextAsset;
        mat = EditorGUILayout.ObjectField(mat, typeof(Material)) as Material;

        if (GUILayout.Button("load"))
        {
            LoadTexture(asset, mat, out Vector3 min, out Vector3 max);
        }
    }

    public static void LoadTexture(TextAsset asset, Material mat, out Vector3 min, out Vector3 max)
    {
        if(GameObject.Find("render"))
        {
            DestroyImmediate(GameObject.Find("render"));
        }

        GameObject parent = new GameObject("render");

        float[,,] array = Load(asset, out min, out max);
        if (array == null)
        {
            return;
        }
        int length = array.GetLength(0);
        int length2 = array.GetLength(1);
        int length3 = array.GetLength(2);
        Texture3D texture3D = new Texture3D(length, length2, length3, TextureFormat.Alpha8, false);
        texture3D.filterMode = FilterMode.Bilinear;
        texture3D.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < length3; i++)
        {
            for (int j = 0; j < length2; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    float num2 = array[k, j, i] / 5f;
                    texture3D.SetPixel(k, j, i, new Color32(255, 255, 255, (byte)Mathf.Clamp(num2 * 128f + 128f, 0f, 255f)));
                }
            }
        }
        texture3D.Apply(false, false);

        for(int x = 0; x < texture3D.width; x++)
        {
            for(int y = 0; y < texture3D.height; y++)
            {
                for(int z = 0; z < texture3D.depth; z++)
                {
                    Color col = texture3D.GetPixel(x, y, z);

                    if (x % 5 == 0 && y % 5 == 0 && z % 5 == 0)
                    {
                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(x, y, z);
                        cube.transform.parent = parent.transform;
                        cube.GetComponent<MeshRenderer>().sharedMaterial = new Material(mat);
                        cube.GetComponent<MeshRenderer>().sharedMaterial.color = col;
                        cube.transform.localScale = Vector3.one * 5f;
                    }
                }
            }
        }
    }

    public static float[,,] Load(TextAsset asset, out Vector3 min, out Vector3 max)
    {
        float[,,] result;
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(asset.bytes)))
        {
            int num = binaryReader.ReadInt32();
            if (num != 1)
            {
                Debug.LogErrorFormat("Distance field '{0}' was in the wrong format (was {1}, expected {2})", new object[]
                {
                    asset.name,
                    num,
                    1
                });
                min = Vector3.zero;
                max = Vector3.zero;
                result = null;
            }
            else
            {
                int num2 = binaryReader.ReadInt32();
                int num3 = binaryReader.ReadInt32();
                int num4 = binaryReader.ReadInt32();
                min.x = binaryReader.ReadSingle();
                min.y = binaryReader.ReadSingle();
                min.z = binaryReader.ReadSingle();
                max.x = binaryReader.ReadSingle();
                max.y = binaryReader.ReadSingle();
                max.z = binaryReader.ReadSingle();
                Debug.Log($"x: {num2} y: {num3} z: {num4} min: {min} max: {max}");
                float[,,] array = new float[num2, num3, num4];
                for (int i = 0; i < num4; i++)
                {
                    for (int j = 0; j < num3; j++)
                    {
                        for (int k = 0; k < num2; k++)
                        {
                            array[k, j, i] = binaryReader.ReadSingle();
                        }
                    }
                }
                result = array;
            }
        }
        return result;
    }
}
