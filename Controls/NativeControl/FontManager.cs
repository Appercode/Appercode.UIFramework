using Android.Content.Res;
using Android.Graphics;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.IO.Path;

namespace Appercode.UI.Controls.NativeControl
{
    internal class FontManager
    {
        private readonly Dictionary<string, Typeface> cache = new Dictionary<string, Typeface>();
        private readonly Lazy<Dictionary<string, string>> namesCache = new Lazy<Dictionary<string, string>>();

        public Typeface GetFont(AssetManager assets, FontFamily fontFamily)
        {
            var fontName = fontFamily.Source;
            Typeface result;
            if (this.cache.TryGetValue(fontName, out result) == false)
            {
                if (this.namesCache.IsValueCreated == false)
                {
                    result = this.Load(assets, fontName);
                }

                if (result != null)
                {
                    this.cache[fontName] = result;
                }
                else
                {
                    result = this.FixNameAndLoad(assets, fontName);
                }

                if (result == null)
                {
                    throw new FileNotFoundException($"Could not load font with name {fontName} from Assets files.");
                }
            }

            return result;
        }

        private Typeface Load(AssetManager assets, string fontName)
        {
            try
            {
               return Typeface.CreateFromAsset(assets, fontName);
            }
            catch (RuntimeException)
            {
                return null;
            }
        }

        private Typeface FixNameAndLoad(AssetManager assets, string sourceFontName)
        {
            string fixedFontName;
            if (this.namesCache.IsValueCreated
                && this.namesCache.Value.TryGetValue(sourceFontName, out fixedFontName))
            {
                return this.cache[fixedFontName];
            }

            var fontName = GetFileNameWithoutExtension(sourceFontName);
            var rootItems = assets.List(string.Empty);
            var matches = rootItems
                .Where(i => i.IndexOf(fontName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .ToArray();
            if (matches.Length == 0)
            {
                matches =
                    (from folder in rootItems
                     where folder.IndexOf("font", StringComparison.OrdinalIgnoreCase) >= 0
                     from item in assets.List(folder)
                     where item.IndexOf(fontName, StringComparison.CurrentCultureIgnoreCase) >= 0
                     orderby HasExtension(item) descending
                     select Combine(folder, item))
                    .ToArray();
            }

            foreach (var match in matches)
            {
                if (this.namesCache.IsValueCreated
                    && this.namesCache.Value.ContainsValue(match))
                {
                    this.namesCache.Value[sourceFontName] = match;
                    return this.cache[match];
                }

                var result = this.Load(assets, match);
                if (result != null)
                {
                    this.namesCache.Value[sourceFontName] = match;
                    this.cache[match] = result;
                    return result;
                }
            }

            return null;
        }
    }
}