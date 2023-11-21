using Army.Domain.Models;
using Army.Service;
using CommunityToolkit.Maui.Views;
using System;
using System.Net.Http;
using System.Reflection;

namespace ArmyFlag;

public partial class DilidiliVideo : ContentPage
{
    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private List<string> tsFiles = new List<string>();

    private string _localVideoUrl;

    private long _sourceId;

    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
        _sourceId = item.Id;
        await StartPlayback();
    }



    private async Task LoadTsFiles()
    {
        // Load ts files into tsFiles list
        tsFiles = new List<string>();
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/Ov2WRI1m.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/q9ZhHzW8.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/GhfsYD1z.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/b4rFzQbP.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/D4miBakT.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/pyEPZoZR.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/4hrhXfxd.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/YfKWTRbl.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/PZnrxD4P.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/wbhaA4hV.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/FUgMUCrm.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/QvZ7r5e5.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/d30gB5Ws.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/gSLmWHxz.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/aBKrBIyg.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/M5ovNCxM.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/qKMkEcJL.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/pKrFMUWP.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/JlDRysgf.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/4QdtOIPQ.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/tz4UIzlI.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/FqGLt6qW.ts");

        lblProgress.Text = $"正在下载资源0/{tsFiles.Count}";
        tsFiles = await _dilidiliPCSourceItemService.DownloadVideos(_sourceId, tsFiles, (index, total) =>
        {
            lblProgress.Text = $"正在下载资源{index}/{total}";
        });

        lblProgress.Text = $"正在解析资源0/{tsFiles}";
        _localVideoUrl = _dilidiliPCSourceItemService.MergeTsVideo(_sourceId, tsFiles, (index, total) =>
        {
            lblProgress.Text = $"正在解析资源{index}/{total}";
        });
    }


    private async Task StartPlayback()
    {
        await LoadTsFiles();
        PlayTsFiles();
    }

    private void PlayTsFiles()
    {
        mediaElement.Source = new Uri(_localVideoUrl);
        mediaElement.Play();
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        await StartPlayback();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        _dilidiliPCSourceItemService.ClearFileCache(_sourceId);
    }
}