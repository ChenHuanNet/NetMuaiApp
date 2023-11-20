using Army.Domain.Models;
using Army.Service;
using CommunityToolkit.Maui.Views;

namespace ArmyFlag;

public partial class DilidiliVideo : ContentPage
{
    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private List<string> tsFiles = new List<string>();
    private int _currentIndex = 0;
    private TimeSpan _currentPosition = TimeSpan.Zero;
    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
        await StartPlayback();
    }

    private void PlayTsFiles()
    {
        mediaElement.Source = new Uri(tsFiles[_currentIndex]);
        mediaElement.Play();
    }

    private void mediaElement_MediaEnded(object sender, EventArgs e)
    {
        // Ԥ������һ�� ts �ļ�
        var nextIndex = (_currentIndex + 1) % tsFiles.Count;
        mediaElement.Source = new Uri(tsFiles[nextIndex]);
        mediaElement.Play();
        // ��¼��ǰ����λ��
        _currentPosition = mediaElement.Position;
    }

    private void mediaElement_MediaOpened(object sender, EventArgs e)
    {
        // ��ת����һ�� ts �ļ��Ľ���λ�ã���������һ�� ts �ļ�
        mediaElement.SeekTo(_currentPosition);
        mediaElement.Play();
    }

    private async Task LoadTsFiles()
    {
        // Load ts files into tsFiles list
        tsFiles = new List<string>();
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/Ov2WRI1m.ts");
        tsFiles.Add("https://s9.fsvod1.com/20231112/G0O0JQeo/2000kb/hls/q9ZhHzW8.ts");
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

        tsFiles = await _dilidiliPCSourceItemService.DownloadVideos(999, tsFiles);

        LoadMedia(tsFiles[0]);
    }


    private async void LoadMedia(string path)
    {
        // �첽����ý���ļ�
        mediaElement.Source = new Uri(path);

        // �ȴ�һ��ʱ�䣬ȷ����һ�� ts �ļ��Ѿ��������
        await Task.Delay(5000);

        // ��ת����һ�� ts �ļ��Ľ���λ�ã���������һ�� ts �ļ�
        mediaElement.SeekTo(_currentPosition);
        _currentIndex = (_currentIndex + 1) % tsFiles.Count;
        LoadMedia(tsFiles[_currentIndex]);
    }

    private async Task StartPlayback()
    {
        await LoadTsFiles();
        PlayTsFiles();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await StartPlayback();
    }
}