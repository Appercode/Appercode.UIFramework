using Android.Content.Res;
using Android.Graphics;
using Appercode.UI.Internals;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.IO.Path;

namespace Appercode.UI.Controls.NativeControl
{
    internal class FontManager
    {
        private readonly Dictionary<string, Typeface> cache = new Dictionary<string, Typeface>();
        private readonly Lazy<Dictionary<string, string>> namesCache = new Lazy<Dictionary<string, string>>();
        private readonly AssetManager assets;
        private readonly AsyncLazy<IList<string>> rootItems;
        private readonly AsyncLazy<IList<string>> fontItems;
        private Task currentLoadingTask;

        public FontManager(AssetManager assetManager)
        {
            if (assetManager == null)
            {
                throw new ArgumentNullException(nameof(assetManager));
            }

            this.assets = assetManager;
            this.rootItems = new AsyncLazy<IList<string>>(this.GetRootItems);
            this.fontItems = new AsyncLazy<IList<string>>(this.GetFontItems);
        }

        public async Task<Typeface> GetFont(FontFamily fontFamily)
        {
            // this method expected to be called on the UI thread only,
            // so no race condition against the currentLoadingTask is expected
            if (this.currentLoadingTask != null)
            {
                // wait until previous loading operation is finished
                await this.currentLoadingTask;
            }

            var loadingTask = this.LoadFont(fontFamily.Source);
            this.currentLoadingTask = loadingTask;
            return await loadingTask;
        }

        private async Task<Typeface> LoadFont(string fontName)
        {
            Typeface result;
            if (this.cache.TryGetValue(fontName, out result) == false)
            {
                if (this.namesCache.IsValueCreated == false)
                {
                    result = await new Loader(this.assets, fontName).LoadAsync();
                }

                if (result != null)
                {
                    this.cache[fontName] = result;
                }
                else
                {
                    result = await this.FixNameAndLoad(fontName);
                }

                if (result == null)
                {
                    throw new FileNotFoundException($"Could not load {fontName} font from Assets files.");
                }
            }

            return result;
        }

        private async Task<Typeface> FixNameAndLoad(string sourceFontName)
        {
            string fixedFontName;
            if (this.namesCache.IsValueCreated
                && this.namesCache.Value.TryGetValue(sourceFontName, out fixedFontName))
            {
                return this.cache[fixedFontName];
            }

            var fontName = GetFileNameWithoutExtension(sourceFontName);
            var matches = (await this.rootItems)
                .Where(i => i.IndexOf(fontName, StringComparison.CurrentCultureIgnoreCase) >= 0);
            if (matches.Any() == false)
            {
                matches = from item in await this.fontItems
                          where item.IndexOf(fontName, StringComparison.CurrentCultureIgnoreCase) >= 0
                          orderby HasExtension(item) descending         // font files could be without extension, so don't filter them out
                          select item;
            }

            foreach (var match in matches)
            {
                if (this.namesCache.IsValueCreated
                    && this.namesCache.Value.ContainsValue(match))
                {
                    this.namesCache.Value[sourceFontName] = match;
                    return this.cache[match];
                }

                var result = await new Loader(this.assets, match).LoadAsync();
                if (result != null)
                {
                    this.namesCache.Value[sourceFontName] = match;
                    this.cache[match] = result;
                    return result;
                }
            }

            return null;
        }

        private async Task<IList<string>> GetRootItems()
        {
            return await this.assets.ListAsync(string.Empty);
        }

        private async Task<IList<string>> GetFontItems()
        {
            var rootItemsValue = await this.rootItems;
            var result = new List<string>(rootItemsValue.Count);
            foreach (var folder in rootItemsValue)
            {
                if (folder.IndexOf("font", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foreach (var item in await this.assets.ListAsync(folder))
                    {
                        result.Add(Combine(folder, item));
                    }
                }
            }

            return result;
        }

        private class Loader
        {
            private readonly AssetManager assets;
            private readonly string fontName;

            public Loader(AssetManager assets, string fontName)
            {
                this.assets = assets;
                this.fontName = fontName;
            }

            public Task<Typeface> LoadAsync()
            {
                return Task.Run((Func<Typeface>)this.Load);
            }

            private Typeface Load()
            {
                try
                {
                    return Typeface.CreateFromAsset(this.assets, this.fontName);
                }
                catch (RuntimeException)
                {
                    return null;
                }
            }
        }
    }
}