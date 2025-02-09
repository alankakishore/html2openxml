﻿using System;
using System.Collections.Generic;

namespace HtmlToOpenXml;

/// <summary>
/// Helper class to translate a named color to its ARGB representation.
/// </summary>
static class HtmlColorTranslator
{
    private static readonly Dictionary<string, HtmlColor> namedColors = InitKnownColors();

    public static HtmlColor FromHtml (string htmlColor)
    {
        namedColors.TryGetValue(htmlColor, out var color);
        return color;
    }

    private static Dictionary<string, HtmlColor> InitKnownColors()
    {
        var colors = new Dictionary<string, HtmlColor>(StringComparer.OrdinalIgnoreCase)
        {
            { "Black", HtmlColor.Black },
            { "White", HtmlColor.FromArgb(255,255,255) },
            { "AliceBlue", HtmlColor.FromArgb(240, 248, 255) },
            { "LightSalmon", HtmlColor.FromArgb(255, 160, 122) },
            { "AntiqueWhite", HtmlColor.FromArgb(250, 235, 215) },
            { "LightSeaGreen", HtmlColor.FromArgb(32, 178, 170) },
            { "Aqua", HtmlColor.FromArgb(0, 255, 255) },
            { "LightSkyBlue", HtmlColor.FromArgb(135, 206, 250) },
            { "Aquamarine", HtmlColor.FromArgb(127, 255, 212) },
            { "LightSlateGray", HtmlColor.FromArgb(119, 136, 153) },
            { "Azure", HtmlColor.FromArgb(240, 255, 255) },
            { "LightSteelBlue", HtmlColor.FromArgb(176, 196, 222) },
            { "Beige", HtmlColor.FromArgb(245, 245, 220) },
            { "LightYellow", HtmlColor.FromArgb(255, 255, 224) },
            { "Bisque", HtmlColor.FromArgb(255, 228, 196) },
            { "Lime", HtmlColor.FromArgb(0, 255, 0) },
            { "LimeGreen", HtmlColor.FromArgb(50, 205, 50) },
            { "BlanchedAlmond", HtmlColor.FromArgb(255, 255, 205) },
            { "Linen", HtmlColor.FromArgb(250, 240, 230) },
            { "Blue", HtmlColor.FromArgb(0, 0, 255) },
            { "Magenta", HtmlColor.FromArgb(255, 0, 255) },
            { "BlueViolet", HtmlColor.FromArgb(138, 43, 226) },
            { "Maroon", HtmlColor.FromArgb(128, 0, 0) },
            { "Brown", HtmlColor.FromArgb(165, 42, 42) },
            { "MediumAquamarine", HtmlColor.FromArgb(102, 205, 170) },
            { "BurlyWood", HtmlColor.FromArgb(222, 184, 135) },
            { "MediumBlue", HtmlColor.FromArgb(0, 0, 205) },
            { "CadetBlue", HtmlColor.FromArgb(95, 158, 160) },
            { "MediumOrchid", HtmlColor.FromArgb(186, 85, 211) },
            { "Chartreuse", HtmlColor.FromArgb(127, 255, 0) },
            { "MediumPurple", HtmlColor.FromArgb(147, 112, 219) },
            { "Chocolate", HtmlColor.FromArgb(210, 105, 30) },
            { "MediumSeaGreen", HtmlColor.FromArgb(60, 179, 113) },
            { "Coral", HtmlColor.FromArgb(255, 127, 80) },
            { "MediumSlateBlue", HtmlColor.FromArgb(123, 104, 238) },
            { "CornflowerBlue", HtmlColor.FromArgb(100, 149, 237) },
            { "MediumSpringGreen", HtmlColor.FromArgb(0, 250, 154) },
            { "Cornsilk", HtmlColor.FromArgb(255, 248, 220) },
            { "MediumTurquoise", HtmlColor.FromArgb(72, 209, 204) },
            { "Crimson", HtmlColor.FromArgb(220, 20, 60) },
            { "MediumVioletRed", HtmlColor.FromArgb(199, 21, 112) },
            { "Cyan", HtmlColor.FromArgb(0, 255, 255) },
            { "MidnightBlue", HtmlColor.FromArgb(25, 25, 112) },
            { "DarkBlue", HtmlColor.FromArgb(0, 0, 139) },
            { "MintCream", HtmlColor.FromArgb(245, 255, 250) },
            { "DarkCyan", HtmlColor.FromArgb(0, 139, 139) },
            { "MistyRose", HtmlColor.FromArgb(255, 228, 225) },
            { "DarkGoldenrod", HtmlColor.FromArgb(184, 134, 11) },
            { "Moccasin", HtmlColor.FromArgb(255, 228, 181) },
            { "DarkGray", HtmlColor.FromArgb(169, 169, 169) },
            { "NavajoWhite", HtmlColor.FromArgb(255, 222, 173) },
            { "DarkGreen", HtmlColor.FromArgb(0, 100, 0) },
            { "Navy", HtmlColor.FromArgb(0, 0, 128) },
            { "DarkKhaki", HtmlColor.FromArgb(189, 183, 107) },
            { "OldLace", HtmlColor.FromArgb(253, 245, 230) },
            { "DarkMagena", HtmlColor.FromArgb(139, 0, 139) },
            { "Olive", HtmlColor.FromArgb(128, 128, 0) },
            { "DarkOliveGreen", HtmlColor.FromArgb(85, 107, 47) },
            { "OliveDrab", HtmlColor.FromArgb(107, 142, 45) },
            { "DarkOrange", HtmlColor.FromArgb(255, 140, 0) },
            { "Orange", HtmlColor.FromArgb(255, 165, 0) },
            { "DarkOrchid", HtmlColor.FromArgb(153, 50, 204) },
            { "OrangeRed", HtmlColor.FromArgb(255, 69, 0) },
            { "DarkRed", HtmlColor.FromArgb(139, 0, 0) },
            { "Orchid", HtmlColor.FromArgb(218, 112, 214) },
            { "DarkSalmon", HtmlColor.FromArgb(233, 150, 122) },
            { "PaleGoldenrod", HtmlColor.FromArgb(238, 232, 170) },
            { "DarkSeaGreen", HtmlColor.FromArgb(143, 188, 143) },
            { "PaleGreen", HtmlColor.FromArgb(152, 251, 152) },
            { "DarkSlateBlue", HtmlColor.FromArgb(72, 61, 139) },
            { "PaleTurquoise", HtmlColor.FromArgb(175, 238, 238) },
            { "DarkSlateGray", HtmlColor.FromArgb(40, 79, 79) },
            { "PaleVioletRed", HtmlColor.FromArgb(219, 112, 147) },
            { "DarkTurquoise", HtmlColor.FromArgb(0, 206, 209) },
            { "PapayaWhip", HtmlColor.FromArgb(255, 239, 213) },
            { "DarkViolet", HtmlColor.FromArgb(148, 0, 211) },
            { "PeachPuff", HtmlColor.FromArgb(255, 218, 155) },
            { "DeepPink", HtmlColor.FromArgb(255, 20, 147) },
            { "Peru", HtmlColor.FromArgb(205, 133, 63) },
            { "DeepSkyBlue", HtmlColor.FromArgb(0, 191, 255) },
            { "Pink", HtmlColor.FromArgb(255, 192, 203) },
            { "DimGray", HtmlColor.FromArgb(105, 105, 105) },
            { "Plum", HtmlColor.FromArgb(221, 160, 221) },
            { "DodgerBlue", HtmlColor.FromArgb(30, 144, 255) },
            { "PowderBlue", HtmlColor.FromArgb(176, 224, 230) },
            { "Firebrick", HtmlColor.FromArgb(178, 34, 34) },
            { "Purple", HtmlColor.FromArgb(128, 0, 128) },
            { "FloralWhite", HtmlColor.FromArgb(255, 250, 240) },
            { "Red", HtmlColor.FromArgb(255, 0, 0) },
            { "ForestGreen", HtmlColor.FromArgb(34, 139, 34) },
            { "RosyBrown", HtmlColor.FromArgb(188, 143, 143) },
            { "Fuschia", HtmlColor.FromArgb(255, 0, 255) },
            { "RoyalBlue", HtmlColor.FromArgb(65, 105, 225) },
            { "Gainsboro", HtmlColor.FromArgb(220, 220, 220) },
            { "SaddleBrown", HtmlColor.FromArgb(139, 69, 19) },
            { "GhostWhite", HtmlColor.FromArgb(248, 248, 255) },
            { "Salmon", HtmlColor.FromArgb(250, 128, 114) },
            { "Gold", HtmlColor.FromArgb(255, 215, 0) },
            { "SandyBrown", HtmlColor.FromArgb(244, 164, 96) },
            { "Goldenrod", HtmlColor.FromArgb(218, 165, 32) },
            { "SeaGreen", HtmlColor.FromArgb(46, 139, 87) },
            { "Gray", HtmlColor.FromArgb(128, 128, 128) },
            { "Seashell", HtmlColor.FromArgb(255, 245, 238) },
            { "Green", HtmlColor.FromArgb(0, 128, 0) },
            { "Sienna", HtmlColor.FromArgb(160, 82, 45) },
            { "GreenYellow", HtmlColor.FromArgb(173, 255, 47) },
            { "Silver", HtmlColor.FromArgb(192, 192, 192) },
            { "Honeydew", HtmlColor.FromArgb(240, 255, 240) },
            { "SkyBlue", HtmlColor.FromArgb(135, 206, 235) },
            { "HotPink", HtmlColor.FromArgb(255, 105, 180) },
            { "SlateBlue", HtmlColor.FromArgb(106, 90, 205) },
            { "IndianRed", HtmlColor.FromArgb(205, 92, 92) },
            { "SlateGray", HtmlColor.FromArgb(112, 128, 144) },
            { "Indigo", HtmlColor.FromArgb(75, 0, 130) },
            { "Snow", HtmlColor.FromArgb(255, 250, 250) },
            { "Ivory", HtmlColor.FromArgb(255, 240, 240) },
            { "SpringGreen", HtmlColor.FromArgb(0, 255, 127) },
            { "Khaki", HtmlColor.FromArgb(240, 230, 140) },
            { "SteelBlue", HtmlColor.FromArgb(70, 130, 180) },
            { "Lavender", HtmlColor.FromArgb(230, 230, 250) },
            { "Tan", HtmlColor.FromArgb(210, 180, 140) },
            { "LavenderBlush", HtmlColor.FromArgb(255, 240, 245) },
            { "Teal", HtmlColor.FromArgb(0, 128, 128) },
            { "LawnGreen", HtmlColor.FromArgb(124, 252, 0) },
            { "Thistle", HtmlColor.FromArgb(216, 191, 216) },
            { "LemonChiffon", HtmlColor.FromArgb(255, 250, 205) },
            { "Tomato", HtmlColor.FromArgb(253, 99, 71) },
            { "LightBlue", HtmlColor.FromArgb(173, 216, 230) },
            { "Turquoise", HtmlColor.FromArgb(64, 224, 208) },
            { "LightCoral", HtmlColor.FromArgb(240, 128, 128) },
            { "Violet", HtmlColor.FromArgb(238, 130, 238) },
            { "LightCyan", HtmlColor.FromArgb(224, 255, 255) },
            { "Wheat", HtmlColor.FromArgb(245, 222, 179) },
            { "LightGoldenrodYellow", HtmlColor.FromArgb(250, 250, 210) },
            { "LightGreen", HtmlColor.FromArgb(144, 238, 144) },
            { "WhiteSmoke", HtmlColor.FromArgb(245, 245, 245) },
            { "LightGray", HtmlColor.FromArgb(211, 211, 211) },
            { "Yellow", HtmlColor.FromArgb(255, 255, 0) },
            { "LightPink", HtmlColor.FromArgb(255, 182, 193) },
            { "YellowGreen", HtmlColor.FromArgb(154, 205, 50) },
            { "Transparent", HtmlColor.FromArgb(0, 0, 0, 0) }
        };

        return colors;
    }
}