﻿/* Copyright (C) Olivier Nizet https://github.com/onizet/html2openxml - All Rights Reserved
 * 
 * This source is subject to the Microsoft Permissive License.
 * Please see the License.txt file for more information.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 */
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml;

/// <summary>
/// Represents the collection of attributes present in the current html tag.
/// </summary>
sealed class HtmlAttributeCollection
{
    // This regex split the attributes. This line is valid and all the attributes are well discovered:
    // <table border="1" contenteditable style="text-align: center; color: #ff00e6" cellpadding=0 cellspacing='0' align="center">
    // RegexOptions.Singleline stands for dealing with attributes that contain newline (typically for base64 image, see issue #8)
    private static readonly Regex stripAttributesRegex = new(@"
#tag and its value surrounded by "" or '
((?<tag>\w+)=(?<sep>""|')\s*(?<val>\#?.*?)(\k<sep>|>))
|
# tag whereas the value is not delimited: cellspacing=0
(?<tag>\w+)=(?<val>\w+)
|
# single tag (with no value): contenteditable
\b(?<tag>\w+)\b", RegexOptions.IgnorePatternWhitespace| RegexOptions.Singleline);

    private static readonly Regex stripStyleAttributesRegex = new(@"(?<name>.+?):\s*(?<val>[^;]+);*\s*");

    private readonly Dictionary<string, string> attributes = [];



    private HtmlAttributeCollection()
    {
    }

    public static HtmlAttributeCollection ParseStyle(string? htmlTag)
    {
        var collection = new HtmlAttributeCollection();
        if (string.IsNullOrEmpty(htmlTag)) return collection;

        // Encoded ':' and ';' characters are valid for browser but not handled by the regex (bug #13812 reported by robin391)
        // ex= <span style="text-decoration&#58;underline&#59;color:red">
        MatchCollection matches = stripStyleAttributesRegex.Matches(
#if NET462
            HttpUtility.HtmlDecode(htmlTag)
#else 
            System.Web.HttpUtility.HtmlDecode(htmlTag)
#endif
        );
        foreach (Match m in matches)
            collection.attributes[m.Groups["name"].Value] = m.Groups["val"].Value;

        return collection;
    }

    /// <summary>
    /// Gets the named attribute.
    /// </summary>
    public string? this[string name]
    {
        get => attributes.TryGetValue(name, out var value)? value : null;
    }

    /// <summary>
    /// Gets an attribute representing a color (named color, hexadecimal or hexadecimal 
    /// without the preceding # character).
    /// </summary>
    public HtmlColor GetAsColor(string name)
    {
        return HtmlColor.Parse(this[name]);
    }

    /// <summary>
    /// Gets an attribute representing an unit: 120px, 10pt, 5em, 20%, ...
    /// </summary>
    /// <returns>If the attribute is misformed, the <see cref="Unit.IsValid"/> property is set to false.</returns>
    public Unit GetAsUnit(string name)
    {
        return Unit.Parse(this[name]);
    }

    /// <summary>
    /// Gets an attribute representing the 4 unit sides.
    /// If a side has been specified individually, it will override the grouped definition.
    /// </summary>
    /// <returns>If the attribute is misformed, the <see cref="Margin.IsValid"/> property is set to false.</returns>
    public Margin GetAsMargin(string name)
    {
        Margin margin = Margin.Parse(this[name]);
        Unit u;

        u = GetAsUnit(name + "-top");
        if (u.IsValid) margin.Top = u;
        u = GetAsUnit(name + "-right");
        if (u.IsValid) margin.Right = u;
        u = GetAsUnit(name + "-bottom");
        if (u.IsValid) margin.Bottom = u;
        u = GetAsUnit(name + "-left");
        if (u.IsValid) margin.Left = u;

        return margin;
    }

    /// <summary>
    /// Gets an attribute representing the 4 border sides.
    /// If a border style/color/width has been specified individually, it will override the grouped definition.
    /// </summary>
    /// <returns>If the attribute is misformed, the <see cref="HtmlBorder.IsEmpty"/> property is set to false.</returns>
    public HtmlBorder GetAsBorder()
    {
        HtmlBorder border = new(GetAsSideBorder("border"));
        SideBorder sb;

        sb = GetAsSideBorder("border-top");
        if (sb.IsValid) border.Top = sb;
        sb = GetAsSideBorder("border-right");
        if (sb.IsValid) border.Right = sb;
        sb = GetAsSideBorder("border-bottom");
        if (sb.IsValid) border.Bottom = sb;
        sb = GetAsSideBorder("border-left");
        if (sb.IsValid) border.Left = sb;

        return border;
    }

    /// <summary>
    /// Gets an attribute representing a single border side.
    /// If a border style/color/width has been specified individually, it will override the grouped definition.
    /// </summary>
    /// <returns>If the attribute is misformed, the <see cref="HtmlBorder.IsEmpty"/> property is set to false.</returns>
    public SideBorder GetAsSideBorder(string name)
    {
        var attrValue = this[name];
        SideBorder border = SideBorder.Parse(attrValue);

        // handle attributes specified individually.
        Unit width = SideBorder.ParseWidth(this[name + "-width"]);
        if (!width.IsValid) width = border.Width;

        var color = GetAsColor(name + "-color");
        if (color.IsEmpty) color = border.Color;

        var style = Converter.ToBorderStyle(this[name + "-style"]);
        if (style == BorderValues.Nil) style = border.Style;

        return new SideBorder(style, color, width);
    }

    /// <summary>
    /// Gets the font attribute and combine with the style, size and family.
    /// </summary>
    public HtmlFont GetAsFont(string name)
    {
        HtmlFont font = HtmlFont.Parse(this[name]);
        FontStyle? fontStyle = font.Style;
        FontVariant? variant = font.Variant;
        FontWeight? weight = font.Weight;
        Unit fontSize = font.Size;
        string? family = font.Family;

        var attrValue = this[name + "-style"];
        if (attrValue != null)
        {
            fontStyle = Converter.ToFontStyle(attrValue) ?? font.Style;
        }
        attrValue = this[name + "-variant"];
        if (attrValue != null)
        {
            variant = Converter.ToFontVariant(attrValue) ?? font.Variant;
        }
        attrValue = this[name + "-weight"];
        if (attrValue != null)
        {
            weight = Converter.ToFontWeight(attrValue) ?? font.Weight;
        }
        attrValue = this[name + "-family"];
        if (attrValue != null)
        {
            family = Converter.ToFontFamily(attrValue) ?? font.Family;
        }

        Unit unit = this.GetAsUnit(name + "-size");
        if (unit.IsValid) fontSize = unit;

        return new HtmlFont(fontStyle, variant, weight, fontSize, family);
    }
}
