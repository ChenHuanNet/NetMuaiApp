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
    private int _currentIndex = 0;
    private TimeSpan _currentPosition = TimeSpan.Zero;
    private double totalSeconds = 0;

    private string _localVideoUrl;

    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
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

        lblProgress.Text = $"����������Դ0/{tsFiles.Count}";
        tsFiles = await _dilidiliPCSourceItemService.DownloadVideos(999, tsFiles, (index, total) =>
        {
            lblProgress.Text = $"����������Դ{index}/{total}";
        });

        lblProgress.Text = $"���ڽ�����Դ0/{tsFiles}";
        _localVideoUrl = _dilidiliPCSourceItemService.MergeTsVideo(999, tsFiles, (index, total) =>
        {
            lblProgress.Text = $"���ڽ�����Դ{index}/{total}";
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


}