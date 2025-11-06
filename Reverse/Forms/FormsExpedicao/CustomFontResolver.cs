using PdfSharp.Fonts;
using System;
using System.IO;

namespace Reverse.Forms.FormsExpedicao
{
    public class CustomFontResolver : IFontResolver
    {
        private const string LiberationFamily = "LiberationSans";
        private const string TimesFamily = "Times New Roman";

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // 🔹 LiberationSans
            if (familyName.Equals(LiberationFamily, StringComparison.OrdinalIgnoreCase))
            {
                if (isBold) return new FontResolverInfo($"{LiberationFamily}-Bold");
                return new FontResolverInfo($"{LiberationFamily}-Regular");
            }

            // 🔹 Times New Roman
            if (familyName.Equals(TimesFamily, StringComparison.OrdinalIgnoreCase))
            {
                if (isBold && isItalic) return new FontResolverInfo($"{TimesFamily}-BoldItalic");
                if (isBold) return new FontResolverInfo($"{TimesFamily}-Bold");
                if (isItalic) return new FontResolverInfo($"{TimesFamily}-Italic");
                return new FontResolverInfo($"{TimesFamily}-Regular");
            }

            return null;
        }

        public byte[] GetFont(string faceName)
        {
            var asm = typeof(CustomFontResolver).Assembly;

            // 🔹 LiberationSans
            if (faceName == $"{LiberationFamily}-Bold")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.LiberationSans-Bold.ttf",
                    "Reverse.Fonts.LiberationSans_Bold.ttf");

            if (faceName == $"{LiberationFamily}-Regular")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.LiberationSans-Regular.ttf",
                    "Reverse.Fonts.LiberationSans_Regular.ttf");

            if (faceName == $"{TimesFamily}-Regular")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.TimesNewRoman-Regular.ttf");

            if (faceName == $"{TimesFamily}-Bold")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.TimesNewRoman-Bold.ttf");

            if (faceName == $"{TimesFamily}-Italic")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.TimesNewRoman-Italic.ttf");

            if (faceName == $"{TimesFamily}-BoldItalic")
                return LoadResourceBytes(asm,
                    "Reverse.Fonts.TimesNewRoman-BoldItalic.ttf");

            throw new FileNotFoundException($"Fonte não encontrada: {faceName}");
        }

        private static byte[] LoadResourceBytes(System.Reflection.Assembly asm, params string[] resourceNames)
        {
            foreach (var name in resourceNames)
            {
                using (var s = asm.GetManifestResourceStream(name))
                {
                    if (s != null)
                    {
                        using var ms = new MemoryStream();
                        s.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            return null;
        }
    }
}