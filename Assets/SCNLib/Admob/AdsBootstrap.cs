public partial class Bootstrap
{

    partial void PreloadAds()
    {
        AdsManager.Instance.Preload(transform);
    }

}
