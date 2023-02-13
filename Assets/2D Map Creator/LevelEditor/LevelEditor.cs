//It's level Editor from DrawEditor.scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using TMPro;

public class LevelEditor : MonoBehaviour {

    enum State
    {
        pencil, //we can draw with pencil
        rubber, //we can clear that with rubber
        colorpicker //choose your own color
    }

    enum DragState
    {
        drag,
        end
    }

    private State state = State.pencil;
    private DragState drag = DragState.end;
    private Vector2 dragto;

    private Vector2 _size;
    private bool drawGrid = true;
    private int pencilSize = 0;
    private bool click;

    private Color color;
    private float _scale = 1;
    private Vector2 pos = Vector2.zero;
    private LevelGenerator levelGenerator;
    private bool pickColor;
    private string texturePath;

    private Text btnSave;

    private bool InputFieldColorValue;

    public Texture2D texture;
    public Texture2D[] textures;
    //public Tile t_gridImage;
    //public RuleTile rt_gridImage;
    public Button tilePrefab;
    public GameObject gridImage;

    void Start()
    {
        texturePath = "";
        size = Vector2.one * 10;
        color = Color.red;
        GetComponentInChildren<CUIColorPicker>().SetOnValueChangeCallback(OnSetColorPicker);
        GetPrefabImages();
        btnSave = transform.Find("Panel/ButtonSave/Text").GetComponent<Text>();
    }

    //Load picture to edit it (png format - you can change it)
    void LoadDialog()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        if (path.Length != 0)
        {
            var fileContent = File.ReadAllBytes(path);
            texture.LoadImage(fileContent);
            texturePath = path;
            size = new Vector2(texture.width, texture.height);
        }
    }

    //Save picture you have created as png
    void SaveDialog()
    {
        var path = EditorUtility.SaveFilePanel(
            "Save texture as PNG",
            "",
            texture.name + ".png",
            "png"
        );

        if (path.Length != 0)
        {
            texturePath = path;
            Save();
        }
    }

    //Save
    void Save()
    {
        var pngData = texture.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(texturePath, pngData);
        AssetDatabase.Refresh();
    }

    void GetPrefabImages()
    {
        levelGenerator = GetComponent<LevelGenerator>();

        textures = new Texture2D[levelGenerator.colorMappings.Length];

        int cnt = 0;
        foreach (ColorSpawn colorMapping in levelGenerator.colorMappings)
        {
            if (colorMapping.ruleTile == null)
            {
                Button btn = Instantiate(tilePrefab, transform.Find("Panel/Palette/Viewport/Content"));
                btn.onClick.AddListener(() =>
                {
                    OnSetColorGrid(colorMapping.color);
                });
                btn.image.sprite = colorMapping.tile.sprite;
            }
            else if(colorMapping.ruleTile != null)
            {
                Button btn = Instantiate(tilePrefab, transform.Find("Panel/Palette/Viewport/Content"));
                btn.onClick.AddListener(() =>
                {
                    OnSetColorGrid(colorMapping.color);
                });
                btn.image.sprite = colorMapping.ruleTile.m_DefaultSprite;
            }
            else
            {
                Button btn = Instantiate(tilePrefab, transform.Find("Panel/Palette/Viewport/Content"));
                btn.onClick.AddListener(() =>
                {
                    OnSetColorGrid(colorMapping.color);
                });
                Sprite sprite = colorMapping.ornament.GetComponent<SpriteRenderer>().sprite;
                btn.image.sprite = sprite;

            }


        }
        CreateTexture();
    }

    public void editColorOnChange(string s)
    {
        editColorOnChangeEnd(s);
    }

    public void editColorOnChangeEnd(string s)
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString(s, out newCol))
        {
            if (color != newCol)
            {
                color = newCol;
            }
        }
    }

    //Take color of the prefab. If you have created prefabs, you can draw with them
    public void OnSetColorGrid(Color c)
    {
        color = c;
        transform.Find("Panel/ColorHEX").GetComponent<InputField>().text = "#" + ColorUtility.ToHtmlStringRGB(color) + "FF";
    }

    public void OnSetColorPicker(Color c)
    {
        color = c;
        transform.Find("Panel/ColorHEX").GetComponent<InputField>().text = "#" + ColorUtility.ToHtmlStringRGB(c) + "FF";
    }


    Vector2 size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
            transform.Find("Panel/SizeX").GetComponent<InputField>().text = _size.x.ToString();
            transform.Find("Panel/SizeY").GetComponent<InputField>().text = _size.y.ToString();
        }
    }

    //Slider
    float scale
    {
        get
        {
            return _scale;
        }
        set
        {
            _scale = value;
            _scale = Mathf.Clamp(_scale, .0625f, 10f);
            transform.Find("Panel/Slider").GetComponent<Slider>().value = _scale;
        }
    }

    public void ShowEditor()
    {
        RectTransform p = transform.Find("Panel").GetComponent<RectTransform>();
        Vector2 v = p.anchoredPosition;
        v.x = v.x == 0 ? -280 : 0;
        p.anchoredPosition = v;
    }

    public void buttonNew()
    {
        CreateTexture();
    }

    public void buttonLoad()
    {
        LoadDialog();
    }

    //If you click SHIFT then you can "Save photo AS" a new file
    public void buttonSave()
    {
        if (Input.GetKey(KeyCode.LeftShift) || texturePath == "")
            SaveDialog();

        else
        {

            Save();
        }
    }

    public void toolChangeValue(int i)
    {
        switch (i)
        {
            case 0:
                state = State.pencil;
                break;
            case 1:
                state = State.rubber;
                break;
        }
    }

    public void toolSizeChangeValue(int i)
    {
        pencilSize = i;
    }

    public void toggleGrid(bool b)
    {
        drawGrid = b;
    }

    public void editSizeXOnChange(string s)
    {
        Vector2 prev = size;
        try
        {
            Vector2 v = size;
            v.x = int.Parse(s);
            size = v;
            if (size.x < 0)
                size = prev;
        }
        catch
        {
            size = prev;
        }
        if (size != prev)
            ResizeTexture();
    }

    public void editSizeXOnEnd(string s)
    {
        var prev = size;
        Vector2 v = size;
        v.x = int.Parse(s);
        size = v;
        if (size.x < 0)
            size = prev;
        if (size != prev)
            ResizeTexture();
    }

    public void editSizeYOnChange(string s)
    {
        Vector2 prev = size;
        try
        {
            Vector2 v = size;
            v.y = int.Parse(s);
            size = v;
            if (size.y < 0)
                size = prev;
        }
        catch
        {
            size = prev;
        }
        if (size != prev)
            ResizeTexture();
    }

    public void editSizeYOnEnd(string s)
    {
        var prev = size;
        Vector2 v = size;
        v.y = int.Parse(s);
        size = v;
        if (size.y < 0)
            size = prev;
        if (size != prev)
            ResizeTexture();
    }

    public void sliderScale(float f)
    {
        scale = f;
    }

    void ResizeTexture()
    {
        Texture2D texturenew = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);

        Color32 resetColor = Color.clear;
        Color32[] resetColorArray = texturenew.GetPixels32();
        // print(size);
        for (int i = 0; i < resetColorArray.Length; i++)
        {
            int x = i % (int)size.x;
            int y = i / (int)size.x;
            // print(new Vector2(x, y));
            if (x < texture.width && y < texture.height)
                resetColorArray[i] = texture.GetPixel(x, y);
            else
                resetColorArray[i] = resetColor;
        }

        texturenew.SetPixels32(resetColorArray);
        texturenew.Apply();
        texture = texturenew;
    }

    void CreateTexture()
    {
        texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);

        Color32 resetColor = Color.clear;
        Color32[] resetColorArray = texture.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        texture.SetPixels32(resetColorArray);
        texture.Apply();

        size = new Vector2(texture.width, texture.height);
    }

    void ScrollImage(Vector2 v)
    {
        Texture2D texturenew = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);

        Color32 resetColor = Color.clear;
        Color32[] resetColorArray = texturenew.GetPixels32();
        for (int i = 0; i < resetColorArray.Length; i++)
        {
            int x = i % (int)size.x;
            int y = i / (int)size.x;
            if (
                x + v.x >= 0 && x + v.x < texture.width &&
                y + v.y >= 0 && y + v.y < texture.height
            )
                resetColorArray[i] = texture.GetPixel(x + (int)v.x, y + (int)v.y);
            else
            {
                resetColorArray[i] = resetColor;
            }
        }

        texturenew.SetPixels32(resetColorArray);
        texturenew.Apply();
        texture = texturenew;
    }

    void Update()
    {
        RectTransform panel = transform.GetChild(0).GetComponent<RectTransform>();
        Rect rect = panel.rect;
        rect.x += panel.position.x;
        rect.y += panel.position.y;
        bool inEditorPanel = Input.mousePosition.x >= rect.x && Input.mousePosition.x < rect.x + rect.width && Input.mousePosition.y >= rect.y && Input.mousePosition.y < rect.y + rect.height;

        btnSave.text = Input.GetKey(KeyCode.LeftShift) ? "Save as..." : "Save";

        pickColor = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ScrollImage(new Vector2(1, 0) * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ScrollImage(new Vector2(-1, 0) * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ScrollImage(new Vector2(0, -1) * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ScrollImage(new Vector2(0, 1) * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1));
        }

        if (!inEditorPanel)
        {
            scale += Input.GetAxis("Mouse ScrollWheel") * .5f;
        }

        if (!inEditorPanel && Input.GetMouseButtonDown(0))
        {
            click = true;
        }

        if (Input.GetMouseButton(1))
        {
            switch (drag)
            {
                case DragState.drag:
                    pos = dragto + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    break;
                case DragState.end:
                    drag = DragState.drag;
                    dragto = pos - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    break;
            }
        }
        else
            drag = DragState.end;

        if (Input.GetMouseButtonUp(0))
        {
            click = false;
        }
    }

    void OnDrawGizmos()
    {
        if (state == State.colorpicker)
            return;

        Vector2 deltaTexture = new Vector2(
            size.x % 2 != 0 ? 0f : (scale / 2f),
            size.y % 2 != 0 ? 0f : (scale / 2f)
        );

        if (drawGrid)
        {
            Gizmos.color = Color.white;
            for (int y = 0; y <= size.y; y++)
            {
                Gizmos.DrawLine(
                    pos + new Vector2(-size.x / 2 * scale, (-size.y / 2 + y) * scale),
                    pos + new Vector2(size.x / 2 * scale, (-size.y / 2 + y) * scale)
                );
                for (int x = 0; x <= size.x; x++)
                {
                    Gizmos.DrawLine(
                        pos + new Vector2((-size.x / 2 + x) * scale, -size.y / 2 * scale),
                        pos + new Vector2((-size.x / 2 + x) * scale, size.y / 2 * scale)
                    );
                }
            }
        }

        if (texture)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Gizmos.color = texture.GetPixel(x, y) * 2;
                    Gizmos.DrawCube(
                        pos + new Vector2(
                            (-size.x / 2 + x) * scale + scale / 2,
                            (-size.y / 2 + y) * scale + scale / 2
                        ),
                        Vector2.one * scale
                    );
                }
            }

        }

        Vector2 mp = Input.mousePosition;
        Vector2 screenPos = (Vector2)Camera.main.ScreenToWorldPoint(mp) - pos;
        Vector2 gridPos = new Vector2(
            Mathf.Round((screenPos.x - deltaTexture.x) / scale) * scale + deltaTexture.x,
            Mathf.Round((screenPos.y - deltaTexture.y) / scale) * scale + deltaTexture.y
        );
        Vector2 deltaPencil = deltaTexture + new Vector2(
            ((pencilSize + 1) % 2 != 0 ? 0 : scale / 2) - deltaTexture.x,
            ((pencilSize + 1) % 2 != 0 ? 0 : scale / 2) - deltaTexture.y
        );

        if (gridPos.x / scale > -size.x / 2 && gridPos.x / scale < size.x / 2 && gridPos.y / scale > -size.y / 2 && gridPos.y / scale < size.y / 2)
        {
            Vector2 textureCoord = (gridPos - deltaTexture) / scale + new Vector2(
                size.x / 2 - (size.x % 2 != 0 ? .5f : 0f),
                size.y / 2 - (size.y % 2 != 0 ? .5f : 0f)
            );

            Gizmos.color = color * 2;
            if (!pickColor)
                Gizmos.DrawCube(gridPos + deltaPencil + pos, new Vector2(pencilSize + 1, pencilSize + 1) * scale);
            else
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(gridPos + pos, scale * .4f);
                Gizmos.color = texture.GetPixel((int)Mathf.Round(textureCoord.x), (int)Mathf.Round(textureCoord.y)) * 2;
                Gizmos.DrawSphere(gridPos + pos, scale * .3f);
            }

            if (texture && click)
            {
                if (pickColor)
                {
                    color = texture.GetPixel((int)Mathf.Round(textureCoord.x), (int)Mathf.Round(textureCoord.y));
                    OnSetColorGrid(color);
                }
                else
                {
                    float dp = (pencilSize + 1) % 2 != 0 ? 0f : .5f;
                    // print(dp);
                    // print((pencilSize + 1) % 2);
                    // print(new Vector2(-pencilSize / 2, pencilSize / 2));
                    // print(new Vector2(-pencilSize / 2f + dp, pencilSize / 2f + dp));
                    for (int y = (int)(-pencilSize / 2f + dp); y <= (int)(pencilSize / 2f + dp); y++)
                        for (int x = (int)(-pencilSize / 2f + dp); x <= (int)(pencilSize / 2f + dp); x++)
                        {
                            // print(textureCoord.x);
                            // print(x);
                            // print(new Vector2(textureCoord.x + x, textureCoord.y + y));
                            // print(new Vector2((int)Mathf.Round(textureCoord.x + x), (int)Mathf.Round(textureCoord.y + y)));
                            if ((int)(textureCoord.x + x) >= 0 && (int)(textureCoord.x + x) < size.x && (int)(textureCoord.y + y) >= 0 && (int)(textureCoord.y + y) < size.y)
                            {
                                texture.SetPixel((int)Mathf.Round(textureCoord.x + x), (int)Mathf.Round(textureCoord.y + y), state == State.pencil ? color : Color.clear);
                            }
                        }
                }
            }
        }
    }
}
