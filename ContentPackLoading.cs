using RoR2.ContentManagement;
using System.Collections;

namespace Crown_Mod;

public class ContentPackLoader
{
    public ContentPack additionalContent;

    private class CustomContentPackProvider : IContentPackProvider
    {
        public CustomContentPackProvider(ContentPack contentPack)
        {
            this.contentPack = contentPack;
        }

        public string identifier { get { return contentPack.identifier; } }

        protected ContentPack contentPack;

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        { // Not actually loading async lol
            args.ReportProgress(1.0f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            args.ReportProgress(1.0f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1.0f);
            yield break;
        }
    }

    private void OnAddContentPack(ContentManager.AddContentPackProviderDelegate _addContentDelegate)
    {
        CustomContentPackProvider myProvider = new CustomContentPackProvider(additionalContent);

        try
        {
            _addContentDelegate.Invoke(myProvider);

            Plugin.Logger.LogInfo("Sucessfully supplied the content provider");
        }
        catch
        {
            Plugin.Logger.LogWarning("Could not load the content pack.");
        }
    }

    public void SetupLoadPack()
    { 
        ContentManager.collectContentPackProviders += OnAddContentPack;
    }

    public ContentPackLoader(ContentPack _additionalContent)
    {
        additionalContent = _additionalContent;
    }
}