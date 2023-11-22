using Army.Domain.Consts;
using Army.Domain.Dto;
using Army.Domain.Models;
using Army.Service;
using System;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace ArmyFlag;

public partial class DilidiliDetail : ContentPage
{

    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private readonly DilidiliVideo _dilidiliVideo;
    private DilidiliPCSource _dilidiliPCSource;
    private List<DilidiliPCSourceItem> _dilidiliPCSourceItems;
    private List<DilidiliPCSourceItem> _showPCSourceItems;

    private List<VideoSourceDto> _videoSourceList;
    private string _sourceVal = "";


    public DilidiliDetail(DilidiliPCSourceItemService dilidiliPCSourceItemService, DilidiliVideo dilidiliVideo)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
        _dilidiliVideo = dilidiliVideo;

    }




    public async Task Init(DilidiliPCSource dilidiliPCSource)
    {
        _videoSourceList = new List<VideoSourceDto>()
        {
            new VideoSourceDto(){ Content="���",Value="zd" },
            new VideoSourceDto(){ Content="����",Value="yj" },
            new VideoSourceDto(){ Content="ţţ",Value="hn" },
            new VideoSourceDto(){ Content="�Ⲩ",Value="gs" },
            new VideoSourceDto(){ Content="����",Value="sn" },
            new VideoSourceDto(){ Content="����",Value="wl" },
            new VideoSourceDto(){ Content="����",Value="lz" },
            new VideoSourceDto(){ Content="F��",Value="fs" },
            new VideoSourceDto(){ Content="�ɷ�",Value="ff" },
            new VideoSourceDto(){ Content="�ٶ�",Value="bd" },
            new VideoSourceDto(){ Content="��U",Value="uk" },
            new VideoSourceDto(){ Content="����",Value="wj" },
            new VideoSourceDto(){ Content="�˽�",Value="bj" },
            new VideoSourceDto(){ Content="���",Value="tk" },
            new VideoSourceDto(){ Content="����",Value="ss" },
            new VideoSourceDto(){ Content="�Ქ",Value="kb" },
            new VideoSourceDto(){ Content="����",Value="sd" },
            new VideoSourceDto(){ Content="����",Value="xk" },
            new VideoSourceDto(){ Content="����",Value="tp" },
            new VideoSourceDto(){ Content="��Ӣ",Value="jy" }
        };

        _dilidiliPCSource = dilidiliPCSource;
        this.Title = _dilidiliPCSource.Name;
        lblSource.Text = dilidiliPCSource.CurrentMaxText;
        videoSourcePicker.ItemsSource = _videoSourceList;
    }


    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        int.TryParse(txtName.Text?.Trim(), out int num);

        if (num <= 0)
        {
            await DisplayAlert("��ʾ", "��������ȷ�ļ���", "ȡ��");
            return;
        }
        if (string.IsNullOrWhiteSpace(_sourceVal))
        {
            await DisplayAlert("��ʾ", "��ѡ�񲥷�Դ", "ȡ��");
            return;
        }

        var detailId = _dilidiliPCSource.DetailUrl.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
        if (string.IsNullOrWhiteSpace(detailId))
        {
            await DisplayAlert("��ʾ", "�����쳣", "ȡ��");
            return;
        }

        string url = $"{AppConfigHelper.DiliDiliSourceHost}/tv/{detailId}/{num}.html?qp={_sourceVal}";

        var model = await _dilidiliPCSourceItemService.FindAsync(_dilidiliPCSource.Id, _sourceVal, num.ToString());
        if (model != null)
        {
            if (File.Exists(model.Url))
            {
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

            html = DecodeEncodedNonAsciiCharacters(html);

            var mu38 = await _dilidiliPCSourceItemService.AnalysisAsync(html, _sourceVal);

            webView.Source = null;
            if (string.IsNullOrWhiteSpace(mu38))
            {
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
                lblProgress.Text = "����ʧ��";
                return;
            }

            lblProgress.Text = "�����ɹ�";

            await Navigation.PushAsync(_dilidiliVideo);
            await _dilidiliVideo.Init(_dilidiliPCSource.Id, _sourceVal, tsFiles);
        }
    }


    static string DecodeEncodedNonAsciiCharacters(string value)
    {
        return Regex.Replace(
         value,
         @"\\u(?<Value>[a-zA-Z0-9]{4})",
         m =>
         {
             return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
         });
    }
}