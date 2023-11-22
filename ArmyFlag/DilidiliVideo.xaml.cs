using Army.Domain.Consts;
using Army.Domain.Models;
using Army.Service;
using Army.Domain.Dto;
using ArmyFlag.ViewModels;
using ArmyFlag.Extensions;

namespace ArmyFlag;

public partial class DilidiliVideo : ContentPage
{
    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;

    private readonly DilidiliPCSourceService _dilidiliPCSourceservice;
    private List<string> _tsFiles = new List<string>();

    private string _localVideoUrl;

    private long _sourceId;
    private string _source;
    private DilidiliPCSourceItem _current;

    private List<VideoSourceDto> _videoSourceList;


    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService, DilidiliPCSourceService dilidiliPCSourceService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
        _videoSourceList = VideoViewModel.VideoSourceList;
        _dilidiliPCSourceservice = dilidiliPCSourceService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
        _current = item;
        _localVideoUrl = item.Url;
        lblNum.Text = $"{item.Name} {item.Num}";
        mediaElement.Source = new Uri(_localVideoUrl);
        mediaElement.Play();
        lblProgress.Text = $"�������";
    }


    public async Task Init(long sourceId, string source, string num, List<string> tsFiles, string name, bool setVideo = true)
    {
        _sourceId = sourceId;
        _source = source;
        _tsFiles = tsFiles;

        lblNum.Text = $"{name} {num}";

        try
        {
            lblProgress.Text = $"����������Դ0/{_tsFiles.Count}";
            _tsFiles = await _dilidiliPCSourceItemService.DownloadVideos(_sourceId, _source, num, _tsFiles, (index, total) =>
            {
                lblProgress.Text = $"����������Դ{index}/{total}";
            });

            lblProgress.Text = $"���ڽ�����Դ0/{_tsFiles.Count}";
            _localVideoUrl = _dilidiliPCSourceItemService.MergeTsVideo(_sourceId, _source, num, _tsFiles, (index, total) =>
            {
                lblProgress.Text = $"���ڽ�����Դ{index}/{total}";
            });

            if (setVideo)
            {
                mediaElement.Source = new Uri(_localVideoUrl);
                mediaElement.Play();
            }


            _current = new DilidiliPCSourceItem()
            {
                Id = 0,
                Source = source,
                SourceId = sourceId,
                Url = _localVideoUrl,
                Name = name,
                Num = num
            };
            await _dilidiliPCSourceItemService.SaveAsync(_current);

            lblProgress.Text = $"�������";
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", ex.ToString(), "ȡ��");
        }
    }




    private void Button_Clicked_1(object sender, EventArgs e)
    {
        _dilidiliPCSourceItemService.ClearFileCache(_sourceId);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        int.TryParse(_current.Num, out int num);
        num++;
        var model = await _dilidiliPCSourceItemService.FindAsync(_current.SourceId, _current.Source, num.ToString());
        if (model == null)
        {
            DilidiliPCSource dilidiliPCSource = await _dilidiliPCSourceservice.FindByIdAsync(_current.SourceId);
            if (dilidiliPCSource == null)
            {
                return;
            }
            var detailId = dilidiliPCSource.DetailUrl.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
            string url = $"{AppConfigHelper.DiliDiliSourceHost}/tv/{detailId}/{num}.html?qp={_current.SourceId}";


            webView.Source = url;
        }
    }

    private void Loading(bool isLoading)
    {

        loadingIndicator.IsRunning = isLoading;
        loadingIndicator.IsVisible = isLoading;
    }

    private async void webView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        var url = e.Url;
        if (url.StartsWith(AppConfigHelper.DiliDiliSourceHost + "/tv"))
        {

            lblProgress.Text = "���ڽ���mu38��ַ";


            var webView = sender as WebView;

            string html = await webView.EvaluateJavaScriptAsync("document.documentElement.outerHTML");

            html = html.DecodeEncodedNonAsciiCharacters();

            var mu38 = await _dilidiliPCSourceItemService.AnalysisAsync(html, _source);

            webView.Source = null;
            if (string.IsNullOrWhiteSpace(mu38))
            {
                Loading(false);
                await DisplayAlert("��ʾ", "��֧�ִ˲���Դ", "ȡ��");
                _videoSourceList.RemoveAll(x => x.Value == _source);
                return;
            }


            lblProgress.Text = "���ڽ���ts��ַ";
            List<string> tsFiles = new List<string>();
            try
            {
                tsFiles = await _dilidiliPCSourceItemService.GetTsVideos(mu38);
                if (tsFiles.Count <= 0)
                {
                    Loading(false);
                    await DisplayAlert("��ʾ", $"��֧�ִ˲���Դ[{(_videoSourceList.FirstOrDefault(x => x.Value == _source)?.Content)}]��MU38��ַ��������", "ȡ��");
                    _videoSourceList.RemoveAll(x => x.Value == _source);
                    return;
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("����", ex.ToString(), "ȡ��");
            }

            if (!tsFiles.Any())
            {
                Loading(false);
                lblProgress.Text = "����ʧ��";
                return;
            }

            lblProgress.Text = "�����ɹ�";
            Loading(false);

            int.TryParse(_current.Num, out int num);
            num++;

            await Init(_current.SourceId, _source, num.ToString(), tsFiles, _current.Name, false);
        }
    }
}