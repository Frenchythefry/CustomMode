using System.IO;
using CustomMode;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FileListBuilder
{
/*    public static void BuildScrollableFileList(string folderPath, GameObject canvas, TMP_FontAsset font)
    {
        // === Scroll View Container ===
        GameObject scrollView = new GameObject("FileScrollView");
        scrollView.transform.SetParent(canvas.transform, false);

        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(500, 400);
        scrollRect.anchoredPosition = Vector2.zero;

        // Add Image background
        Image bg = scrollView.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.5f);

        // Add ScrollRect component
        ScrollRect sr = scrollView.AddComponent<ScrollRect>();
        
        sr.horizontal = false;

        // === Viewport ===
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform vpRect = viewport.AddComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        vpRect.offsetMin = Vector2.zero;
        vpRect.offsetMax = Vector2.zero;

        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        Image maskImg = viewport.AddComponent<Image>();
        maskImg.color = Color.clear;

        sr.viewport = vpRect;

        // === Content Holder ===
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0); // Let ContentSizeFitter update the height

        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5;
        layout.childAlignment = TextAnchor.UpperCenter;

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // === Populate with Buttons ===
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                MelonLogger.Msg(name);
                GameObject buttonObj = new GameObject(name);
                buttonObj.transform.SetParent(content.transform, false);

                RectTransform btnRect = buttonObj.AddComponent<RectTransform>();
                btnRect.anchorMin = new Vector2(0, 1);
                btnRect.anchorMax = new Vector2(1, 1);
                btnRect.pivot = new Vector2(0.5f, 1);
                btnRect.anchoredPosition = Vector2.zero;
                btnRect.sizeDelta = new Vector2(0, 40); // only height matters, width will stretch 
                RawImage btnImg = buttonObj.AddComponent<RawImage>();
                btnImg.texture = LoadPNG.LoadFromResources("CustomMode.Assets.Resources.Textures.DRBR.png");

                Button btn = buttonObj.AddComponent<Button>();
                btn.onClick.AddListener(() => OnFileSelected(file));
                MelonLoader.MelonLogger.Msg($"Button created at: {btnRect.anchoredPosition}");

                // Add text
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                RectTransform txtRect = textObj.AddComponent<RectTransform>();
                txtRect.sizeDelta = new Vector2(460, 40);

                TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
                tmp.font = font;
                tmp.text = name;
                tmp.fontSize = 20;
                tmp.color = Color.red;
                tmp.alignment = TextAlignmentOptions.Center;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        }
    }*/
    public static void BuildFileList(string folderPath, GameObject parentContainer, TMP_FontAsset font)
    {
        if (!Directory.Exists(folderPath)) { Debug.LogError($"Folder does not exist: {folderPath}"); return; }

        GameObject scrollViewGO = new GameObject("FileScrollView");
        scrollViewGO.transform.SetParent(parentContainer.transform, false);
        RectTransform scrollRectTransform = scrollViewGO.AddComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
        scrollRectTransform.sizeDelta = new Vector2(400, 320);
        scrollRectTransform.anchoredPosition = Vector2.zero;

        ScrollRect scrollRect = scrollViewGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollViewGO.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(1, 1, 1, 0.05f);
        viewportImage.raycastTarget = false;
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        scrollRect.viewport = viewportRect;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        scrollRect.content = contentRect;

        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.spacing = 6;
        layoutGroup.padding = new RectOffset(6, 6, 6, 6);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        foreach (string file in Directory.GetFiles(folderPath, "*.png"))
        {
            GameObject buttonGO = new GameObject(Path.GetFileName(file));
            buttonGO.transform.SetParent(content.transform, false);

            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = new Color(0.85f, 0.85f, 0.85f);
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            RectTransform btnRect = buttonGO.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 0.5f);
            btnRect.anchorMax = new Vector2(1, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(0, 30);

            LayoutElement le = buttonGO.AddComponent<LayoutElement>();
            le.preferredHeight = 30;
            le.minHeight = 30;

            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TMP_Text tmpText = textGO.AddComponent<TextMeshProUGUI>();
            tmpText.font = font;
            tmpText.text = Path.GetFileNameWithoutExtension(file);
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.black;
            tmpText.fontSize = 16;

            string capturedPath = file;
            button.onClick.AddListener(() => OnFileSelected(capturedPath));
        }
    }


    private static void OnFileSelected(string path)
    {
        GameObject g = new GameObject("PathStorer");
        g.name = "PathStorer";
        var holder = g.AddComponent<PathHolder>();
        holder.path = path;
        Object.DontDestroyOnLoad(g);
        SceneManager.LoadScene("Gameplay Local");
    }

}
public class PathHolder : MonoBehaviour
{
    public string path;
}

