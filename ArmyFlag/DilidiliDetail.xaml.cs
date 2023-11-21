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
            new VideoSourceDto(){ Content="最大",Value="zd" },
            new VideoSourceDto(){ Content="永久",Value="yj" },
            new VideoSourceDto(){ Content="牛牛",Value="hn" },
            new VideoSourceDto(){ Content="光波",Value="gs" },
            new VideoSourceDto(){ Content="新朗",Value="sn" },
            new VideoSourceDto(){ Content="涡轮",Value="wl" },
            new VideoSourceDto(){ Content="良子",Value="lz" },
            new VideoSourceDto(){ Content="F速",Value="fs" },
            new VideoSourceDto(){ Content="飞飞",Value="ff" },
            new VideoSourceDto(){ Content="百度",Value="bd" },
            new VideoSourceDto(){ Content="酷U",Value="uk" },
            new VideoSourceDto(){ Content="无天",Value="wj" },
            new VideoSourceDto(){ Content="八戒",Value="bj" },
            new VideoSourceDto(){ Content="天空",Value="tk" },
            new VideoSourceDto(){ Content="速速",Value="ss" },
            new VideoSourceDto(){ Content="酷播",Value="kb" },
            new VideoSourceDto(){ Content="闪电",Value="sd" },
            new VideoSourceDto(){ Content="看看",Value="xk" },
            new VideoSourceDto(){ Content="淘淘",Value="tp" },
            new VideoSourceDto(){ Content="精英",Value="jy" }
        };

        _dilidiliPCSource = dilidiliPCSource;
        this.Title = _dilidiliPCSource.Name;
        lblSource.Text = dilidiliPCSource.CurrentMaxNum;
        videoSourcePicker.ItemsSource = _videoSourceList;
    }


    private async void sourceItemList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var item = (DilidiliPCSourceItem)e.Item;
        await Navigation.PushAsync(_dilidiliVideo);
        await _dilidiliVideo.Init(item);
    }

    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        int.TryParse(txtName.Text?.Trim(), out int num);

        if (num <= 0)
        {
            await DisplayAlert("提示", "请输入正确的集数", "取消");
            return;
        }
        if (string.IsNullOrWhiteSpace(_sourceVal))
        {
            await DisplayAlert("提示", "请选择播放源", "取消");
            return;
        }

        var detailId = _dilidiliPCSource.PlaySource.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
        if (string.IsNullOrWhiteSpace(detailId))
        {
            await DisplayAlert("提示", "解析异常", "取消");
            return;
        }

        string url = $"{AppConfigHelper.DiliDiliSourceHost}/tv/{detailId}/{num}.html";


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
            var webView = sender as WebView;

            string html = await webView.EvaluateJavaScriptAsync("document.documentElement.outerHTML");

            var html2 = DecodeEncodedNonAsciiCharacters(html);

            await _dilidiliPCSourceItemService.AnalysisAsync(html);
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