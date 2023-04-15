using Army.Domain.Models;
using Army.Service;

namespace ArmyFlag;

public partial class DilidiliVideo : ContentPage
{
    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private List<string> urls = new List<string>();
    private int index = 0;
    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/bnby5f2r.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/T9DQOaIW.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/AdeMY2yW.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/SWU1QvFU.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/Z3stVbt7.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/OSlzgMiO.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/3zbcYkhW.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/X5CLyS8Z.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/ueZ8KdwF.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/6lgbAFja.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/4Z0PZGpe.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/XIZBt4IH.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/mnWRWHNy.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/5FTl1tWS.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/6XMJr4qj.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/s0zQlOm0.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/CeJWnPeQ.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/PaPxipCN.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/nffEhV8i.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/VJu8vKEj.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/9laZbYRm.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/bJtLWnk3.ts");
        urls.Add("https://hey10.cjkypo.com/20230408/1zx8tvGf/1000kb/hls/2ysICRLC.ts");

        urls = await _dilidiliPCSourceItemService.DownloadVideos(item.SourceId, urls);
        if (urls.Count > index)
        {
            mediaElement.Source = urls[index];
            mediaElement.IsVisible = true;
            mediaElement.Play();
        }

        if (urls.Count > index + 1)
        {
            mediaElement1.Source = urls[index + 1];
            mediaElement1.Stop();
            mediaElement1.IsVisible = false;
        }

    }

    private void mediaElement_MediaEnded(object sender, EventArgs e)
    {
        index++;
        if (urls.Count > index)
        {
            if ((index + 1) % 2 > 0)
            {
                //一号播放器播放
                mediaElement.Stop();
                mediaElement.Play();
                mediaElement.IsVisible = true;
                mediaElement1.IsVisible = false;

                if (urls.Count > index + 1)
                {
                    mediaElement1.Source = urls[index + 1];
                }
            }
            else
            {
                //二号播放器播放
                mediaElement.Stop();
                mediaElement1.Play();
                mediaElement1.IsVisible = true;
                mediaElement.IsVisible = false;

                if (urls.Count > index + 1)
                {
                    mediaElement.Source = urls[index + 1];
                }
            }

            decimal progress = Convert.ToDecimal(index + 1) / urls.Count;
            lblProgess.Text = Math.Round(progress, 2) * 100 + "%";
        }
    }
}