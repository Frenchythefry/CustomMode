using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using MelonLoader;
using MelonLoader.TinyJSON;
using Unity.VisualScripting;
using UnityEngine;
using Color = System.Drawing.Color;

namespace CustomMode
{
    public static class LoadPNG
    {
        public static Texture2D LoadFromResources(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null) return null;

                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return texture;
            }
        }
        public static void Load()
        {
            Texture2D image = LoadTexture(LoadImage("/home/frenchy/Documents/Asesprite/BaseChessboard.png"));
            Piece[,] pieces = LoadPieces(image);
            Debug(pieces);
        }
        public static Byte[] LoadImage(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static UnityEngine.Texture2D LoadTexture(Byte[] image)
        {
            Texture2D tex = new UnityEngine.Texture2D(1, 1);
            tex.LoadImage(image);
            return tex;
        }
        
        public static Piece[,] LoadPieces(Texture2D texture)
        {
            Piece[,] ret = new Piece[8, 8];
            for (int x = 0; x < texture.width; x += 2)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    MelonLogger.Msg("Hex of " + x / 2 + ", " + y + " is " + texture.GetPixel(x + 1, y).ToHexString());
                    if (GetPieceFromColor(texture.GetPixel(x + 1, y)) == "")
                    {
                        ret[x / 2, y] = new Piece();
                        continue;
                    }
                    ret[x / 2, y] = new Piece(GetPieceFromColor(texture.GetPixel(x + 1, y)), GetIsWhiteFromColor(texture.GetPixel(x, y)));
                    /*UnityEngine.Color colorPiece = texture.GetPixel(x + 1, y);
                    UnityEngine.Color colorWhite = texture.GetPixel(x, y);

                    string pieceType = GetPieceFromColor(colorPiece);
                    bool isWhite = GetIsWhiteFromColor(colorWhite);

                    ret[x / 2, y] = string.IsNullOrEmpty(pieceType) ? new Piece() : new Piece(pieceType, isWhite);*/
                }
            }

            return ret;
        }

        public static string GetPieceFromColor(UnityEngine.Color color)
        {
            if (CompareColors(color, UnityEngine.Color.gray))    return "Pawn";
            if (CompareColors(color, UnityEngine.Color.green))   return "Rook";
            if (CompareColors(color, UnityEngine.Color.red))     return "Bishop";
            if (CompareColors(color, UnityEngine.Color.blue))    return "Knight";
            if (CompareColors(color, UnityEngine.Color.yellow))  return "King";
            if (CompareColors(color, UnityEngine.Color.magenta)) return "Queen";
            return "";
        }
        
        public static bool GetIsWhiteFromColor(UnityEngine.Color color)
        {
            if (color == UnityEngine.Color.black)
            {
                return false;
            }

            return true;
        }

        public static void Debug(Piece[,] pieces)
        {
            // Print board top-down (rank 8 to 1)
            for (int y = 7; y >= 0; y--)
            {
                string row = $"Rank {y + 1}: ";
                for (int x = 0; x < 8; x++)
                {
                    Piece piece = pieces[x, y];
                    string symbol = (piece.isNull || string.IsNullOrEmpty(piece.type)) ? " " : ( piece.isWhite ? "W" : "B");
                    row += $"[{symbol}]";
                }
                MelonLogger.Msg(row);
            }

            // Add file labels at the bottom
            MelonLogger.Msg("      a   b   c   d   e   f   g   h");
        }

        public static bool CompareColors(UnityEngine.Color a, UnityEngine.Color b, float tolerance = 0.01f)
        {
            return Mathf.Abs(a.r - b.r) < tolerance &&
                   Mathf.Abs(a.g - b.g) < tolerance &&
                   Mathf.Abs(a.b - b.b) < tolerance;
        }

    }
}