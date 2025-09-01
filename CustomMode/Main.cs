using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CustomMode.LoadPNG;
[assembly: MelonInfo(typeof(CustomMode.Main), "Custom Modes", "1.0.0", "Frenchy")]
[assembly: MelonGame("Grouch", "Hyper Chess")]
namespace CustomMode
{

    public class Main : MelonMod
    {    
        bool isLobby = false;
        bool GotMM = false;
        Piece[,] pieces;
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Lobby")
            {
                isLobby = true;
            }
            else
            {
                isLobby = false;
            }

            GotMM = false;
        }

        public override void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("Mode", 0);
            PlayerPrefs.Save();
        }

        public override void OnUpdate()
        {
            if (Keyboard.current.uKey.wasPressedThisFrame)
            {
                CreateCustomLevelGUI();
            }
            if (isLobby && !GotMM)
            {
                try
                {
                    GameObject obj = GameObject.Find("Mode Button & Manager");
                    GameObject obj2 = GameObject.Find("Start Button");
                    if (obj != null && obj2 != null)
                    {
                        obj2.GetComponent<Button>().onClick.RemoveAllListeners();
                        Button startBtn = obj2.GetComponent<Button>();
                        startBtn.onClick = new Button.ButtonClickedEvent();
                        startBtn.onClick.AddListener(() => CreateCustomLevelGUI());
                        ModeManager mm = obj.GetComponent<ModeManager>();
                        mm.modeTitles = mm.modeTitles.Concat(new string[] { "Custom" }).ToArray();
                        mm.modeDescriptions = mm.modeDescriptions.Concat(new string[] { "Custom piece setup" }).ToArray();
                        GotMM = true;
                    }
                }
                catch(Exception e)
                {
                    MelonLogger.Error(e);
                }
            }
        }

        public static void CreateCustomLevelGUI()
        {
            GameObject canvas = GameObject.Find("Canvas");

            Texture2D GUIBackground = LoadFromResources("CustomMode.Assets.Resources.Textures.BR.png");
            GUIBackground.filterMode = FilterMode.Point;

            GameObject background = new GameObject("Background");
            background.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(450, 450);
            bgRect.anchoredPosition = Vector2.zero;

            RawImage bgImage = background.AddComponent<RawImage>();
            bgImage.texture = GUIBackground;

            TMP_FontAsset font = GameObject.Find("BTN_quit")
                ?.transform.GetChild(0)
                ?.GetComponent<TextMeshProUGUI>()
                ?.font;

            GameObject title = new GameObject("Title");
            title.transform.SetParent(background.transform, false);
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(400, 100);
            titleRect.anchoredPosition = new Vector2(0, 200);

            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.font = font;
            titleText.text = "Select Custom Mode";
            titleText.color = Color.red;
            titleText.alignment = TextAlignmentOptions.Center;

            GameObject specialCanvas = new GameObject("Special Canvas");
            specialCanvas.AddComponent<RectTransform>();
            Canvas c = specialCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            specialCanvas.AddComponent<CanvasScaler>();
            specialCanvas.AddComponent<GraphicRaycaster>();

            FileListBuilder.BuildFileList(
                Path.Combine(MelonEnvironment.GameRootDirectory, "Mods/Custom"),
                background,
                font
            );
            
            GameObject closeBtnObj = new GameObject("OpenPathButton");
            closeBtnObj.transform.SetParent(background.transform, false);
            RectTransform closeBtnRect = closeBtnObj.AddComponent<RectTransform>();
            closeBtnRect.sizeDelta = new Vector2(50, 50);
            closeBtnRect.anchoredPosition = new Vector2(-200, 200); // bottom of background

            Button closeBtn = closeBtnObj.AddComponent<Button>();
            Image closeBtnImage = closeBtnObj.AddComponent<Image>();
            closeBtnImage.color = UnityEngine.Color.gray; // dark background

            GameObject closeLabelObj = new GameObject("Label");
            closeLabelObj.transform.SetParent(closeBtnObj.transform, false);
            RectTransform closeLabelRect = closeLabelObj.AddComponent<RectTransform>();
            closeLabelRect.sizeDelta = closeBtnRect.sizeDelta;

            TextMeshProUGUI closeLabel = closeLabelObj.AddComponent<TextMeshProUGUI>();
            closeLabel.font = font;
            closeLabel.text = "X";
            closeLabel.color = Color.red;
            closeLabel.alignment = TextAlignmentOptions.Center;
            closeBtn.onClick.AddListener(() =>
            {
                GameObject.Destroy(background);
            });
            // === Add Open Path button ===
            GameObject openBtnObj = new GameObject("OpenPathButton");
            openBtnObj.transform.SetParent(background.transform, false);
            RectTransform btnRect = openBtnObj.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(300, 50);
            btnRect.anchoredPosition = new Vector2(0, -200); // bottom of background

            Button openBtn = openBtnObj.AddComponent<Button>();
            Image btnImage = openBtnObj.AddComponent<Image>();
            btnImage.color = UnityEngine.Color.gray; // dark background

            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(openBtnObj.transform, false);
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.sizeDelta = btnRect.sizeDelta;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.font = font;
            label.text = "Open Setup Path";
            label.color = Color.red;
            label.alignment = TextAlignmentOptions.Center;

            string folderPath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods/Custom");

            openBtn.onClick.AddListener(() =>
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                });
            });
        }
        public static void LoadCustomLevelLocal(string path)
        {
            GameObject pawnWhite = null;
            GameObject queenWhite = null;
            GameObject kingWhite = null;
            GameObject bishopWhite = null;
            GameObject rookWhite = null;
            GameObject knightWhite = null;
            GameObject pawnBlack = null;
            GameObject queenBlack = null;
            GameObject kingBlack = null;
            GameObject bishopBlack = null;
            GameObject rookBlack = null;
            GameObject knightBlack = null;
            GameObject obj = GameObject.Find("Local Pieces & Gamemode");
            foreach (Transform child in obj.transform)
            {
                if (child.name == "Classical Parent")
                {
                    child.gameObject.SetActive(true);
                }
                child.gameObject.SetActive(false);
            }
            GameObject cp = obj.transform.GetChild(0).gameObject;
            foreach (Transform child in cp.transform)
            {
                PieceControllerLocal pcl = child.GetComponent<PieceControllerLocal>();
                if (pcl.pieceName == "Pawn" && (pawnWhite == null || pawnBlack == null))
                {
                    if (pawnWhite == null && pcl.isWhite)
                    {
                        pawnWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (pawnBlack == null && !pcl.isWhite)
                    {
                        pawnBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                else if (pcl.pieceName == "Queen" && (queenWhite == null || queenBlack == null))
                {
                    if (queenWhite == null && pcl.isWhite)
                    {
                        queenWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (queenBlack == null && !pcl.isWhite)
                    {
                        queenBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                else if (pcl.pieceName == "King" && (kingWhite == null || kingBlack == null))
                {
                    if (kingWhite == null && pcl.isWhite)
                    {
                        kingWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (kingBlack == null && !pcl.isWhite)
                    {
                        kingBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                else if (pcl.pieceName == "Bishop" && (bishopWhite == null || bishopBlack == null))
                {
                    if (bishopWhite == null && pcl.isWhite)
                    {
                        bishopWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (bishopBlack == null && !pcl.isWhite)
                    {
                        bishopBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                else if (pcl.pieceName == "Rook" && (rookWhite == null || rookBlack == null))
                {
                    if (rookWhite == null && pcl.isWhite)
                    {
                        rookWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (rookBlack == null && !pcl.isWhite)
                    {
                        rookBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                else if (pcl.pieceName == "Knight" && (knightWhite == null || knightBlack == null))
                {
                    if (knightWhite == null && pcl.isWhite)
                    {
                        knightWhite = GameObject.Instantiate(child.gameObject);
                    }
                    else if (knightBlack == null && !pcl.isWhite)
                    {
                        knightBlack = GameObject.Instantiate(child.gameObject);
                    }
                }
                child.gameObject.SetActive(false);
            }
            
            pawnWhite.transform.parent = obj.transform;
            queenWhite.transform.parent = obj.transform;
            kingWhite.transform.parent = obj.transform;
            bishopWhite.transform.parent = obj.transform;
            rookWhite.transform.parent = obj.transform;
            knightWhite.transform.parent = obj.transform;
            pawnBlack.transform.parent = obj.transform;
            queenBlack.transform.parent = obj.transform;
            kingBlack.transform.parent = obj.transform;
            bishopBlack.transform.parent = obj.transform;
            rookBlack.transform.parent = obj.transform;
            knightBlack.transform.parent = obj.transform;
            pawnWhite.SetActive(false);
            queenWhite.SetActive(false);
            kingWhite.SetActive(false);
            bishopWhite.SetActive(false);
            rookWhite.SetActive(false);
            knightWhite.SetActive(false);
            pawnBlack.SetActive(false);
            queenBlack.SetActive(false);
            kingBlack.SetActive(false);
            bishopBlack.SetActive(false);
            rookBlack.SetActive(false);
            knightBlack.SetActive(false);
            Texture2D image = LoadTexture(LoadImage(path));
            Piece[,] pieces = LoadPieces(image);
            for (int x = 0; x < pieces.Length; x++)
            {
                for (int y = 0; y < pieces.GetLength(1); y++)
                {
                    if (pieces[x, y].isNull) continue; // skip empty squares

                    GameObject a = null;

                    switch (pieces[x, y].type)
                    {
                        case "Pawn":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(pawnWhite) 
                                : GameObject.Instantiate(pawnBlack);
                            break;

                        case "Rook":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(rookWhite) 
                                : GameObject.Instantiate(rookBlack);
                            break;

                        case "Knight":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(knightWhite) 
                                : GameObject.Instantiate(knightBlack);
                            break;

                        case "Bishop":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(bishopWhite) 
                                : GameObject.Instantiate(bishopBlack);
                            break;

                        case "Queen":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(queenWhite) 
                                : GameObject.Instantiate(queenBlack);
                            break;

                        case "King":
                            a = pieces[x, y].isWhite 
                                ? GameObject.Instantiate(kingWhite) 
                                : GameObject.Instantiate(kingBlack);
                            break;
                    }

                    if (a != null)
                    {
                        a.transform.parent = obj.transform; // keeps original hierarchy if needed
                        a.transform.position = new Vector2(x - 3.5f, y - 3.5f);
                        a.SetActive(true);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GamemodeController), "Awake")]
    class patch1
    {
        static bool Prefix(GamemodeController __instance)
        {
            switch (PlayerPrefs.GetInt("Mode"))
            {
                case 1:
                    __instance.EnableGamemode(1);
                    break;
                case 2:
                    __instance.EnableGamemode(2);
                    break;
                case 3:
                    __instance.FischerScramble();
                    break;
                case 4:
                    if (SceneManager.GetActiveScene().name == "Gameplay Local")
                    {
                        CustomMode.Main.LoadCustomLevelLocal(GameObject.Find("PathStorer").GetComponent<PathHolder>().path);
                    }
                    else __instance.EnableGamemode(0);
                    break;
                default:
                    __instance.EnableGamemode(0);
                    break;
            }

            return false;
        }
    }
}