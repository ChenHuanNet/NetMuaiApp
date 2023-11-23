using Army.Domain.Consts;
using Army.Domain.Dto;
using Army.Domain.Models;
using Army.Service;
using ArmyFlag.ViewModels;
using ArmyFlag.Extensions;

namespace ArmyFlag;

public partial class DilidiliDetail : ContentPage
{

    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private readonly MyCollectionService _myCollectionService;
    private readonly DilidiliVideo _dilidiliVideo;
    private DilidiliPCSource _dilidiliPCSource;
    private List<DilidiliPCSourceItem> _dilidiliPCSourceItems;
    private List<DilidiliPCSourceItem> _showPCSourceItems;

    private List<VideoSourceDto> _videoSourceList;
    private string _sourceVal = "";


    public DilidiliDetail(DilidiliPCSourceItemService dilidiliPCSourceItemService, DilidiliVideo dilidiliVideo, MyCollectionService myCollectionService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
        _dilidiliVideo = dilidiliVideo;
        _videoSourceList = VideoViewModel.VideoSourceList;
        _myCollectionService = myCollectionService;
    }




    public async Task Init(DilidiliPCSource dilidiliPCSource)
    {


        _dilidiliPCSource = dilidiliPCSource;
        this.Title = _dilidiliPCSource.Name;
        lblSource.Text = dilidiliPCSource.CurrentMaxText;
        videoSourcePicker.ItemsSource = _videoSourceList;
    }

    private void Loading(bool isLoading)
    {

        loadingIndicator.IsRunning = isLoading;
        loadingIndicator.IsVisible = isLoading;
    }


    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        Loading(true);
        int.TryParse(txtName.Text?.Trim(), out int num);

        if (num <= 0)
        {
            Loading(false);
            await DisplayAlert("��ʾ", "��������ȷ�ļ���", "ȡ��");
            return;
        }
        if (string.IsNullOrWhiteSpace(_sourceVal))
        {
            Loading(false);
            await DisplayAlert("��ʾ", "��ѡ�񲥷�Դ", "ȡ��");
            return;
        }

        var detailId = _dilidiliPCSource.DetailUrl.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
        if (string.IsNullOrWhiteSpace(detailId))
        {
            Loading(false);
            await DisplayAlert("��ʾ", "�����쳣", "ȡ��");
            return;
        }

        string url = $"{AppConfigHelper.DiliDiliSourceHost}/tv/{detailId}/{num}.html?qp={_sourceVal}";

        var model = await _dilidiliPCSourceItemService.FindAsync(_dilidiliPCSource.Id, _sourceVal, num.ToString());
        if (model != null)
        {
            if (File.Exists(model.Url))
            {
                Loading(false);
                //�����صĵ�ַ
                await Navigation.PushAsync(_dilidiliVideo);
                await _dilidiliVideo.Init(model);
                return;
            }
            else
            {
                //���ص�ַû�ˣ���ɾ��
                await _dilidiliPCSourceItemService.DeleteByIdAsync(model.Id);
            }

        }

        webView.Source = url;

    }


    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
    }


    private void videoSourcePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedIndex >= 0)
        {
            var item = (VideoSourceDto)picker.SelectedItem;
            _sourceVal = item.Value;
        }
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

            var mu38 = await _dilidiliPCSourceItemService.AnalysisAsync(html, _sourceVal);

            webView.Source = null;
            if (string.IsNullOrWhiteSpace(mu38))
            {
                Loading(false);
                await DisplayAlert("��ʾ", "��֧�ִ˲���Դ", "ȡ��");
                _videoSourceList.RemoveAll(x => x.Value == _sourceVal);
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
                    await DisplayAlert("��ʾ", $"��֧�ִ˲���Դ[{(_videoSourceList.FirstOrDefault(x => x.Value == _sourceVal)?.Content)}]��MU38��ַ��������", "ȡ��");
                    _videoSourceList.RemoveAll(x => x.Value == _sourceVal);
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

            int.TryParse(txtName.Text?.Trim(), out int num);

            await Navigation.PushAsync(_dilidiliVideo);
            await _dilidiliVideo.Init(_dilidiliPCSource.Id, _sourceVal, num.ToString(), tsFiles, _dilidiliPCSource.Name);
        }
    }

    private async void btnCollect_Clicked(object sender, EventArgs e)
    {
        long sourceId = _dilidiliPCSource.Id;
        var model = await _myCollectionService.FindBySourceIdAsync(sourceId);
        if (model == null)
        {
            //�ղ�
        }
    }
}