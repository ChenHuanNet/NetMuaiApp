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
        lblSource.Text = dilidiliPCSource.CurrentMaxText;
        videoSourcePicker.ItemsSource = _videoSourceList;
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

        var detailId = _dilidiliPCSource.DetailUrl.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
        if (string.IsNullOrWhiteSpace(detailId))
        {
            await DisplayAlert("提示", "解析异常", "取消");
            return;
        }

        string url = $"{AppConfigHelper.DiliDiliSourceHost}/tv/{detailId}/{num}.html?qp={_sourceVal}";

        var model = await _dilidiliPCSourceItemService.FindAsync(_dilidiliPCSource.Id, _sourceVal, num.ToString());
        if (model != null)
        {
            if (File.Exists(model.Url))
            {
                //读本地的地址
                await Navigation.PushAsync(_dilidiliVideo);
                await _dilidiliVideo.Init(model);
                return;
            }
            else
            {
                //本地地址没了，就删了
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

            lblProgress.Text = "正在解析mu38地址";


            var webView = sender as WebView;

            string html = await webView.EvaluateJavaScriptAsync("document.documentElement.outerHTML");

            html = DecodeEncodedNonAsciiCharacters(html);

            var mu38 = await _dilidiliPCSourceItemService.AnalysisAsync(html, _sourceVal);

            webView.Source = null;
            if (string.IsNullOrWhiteSpace(mu38))
            {
                await DisplayAlert("提示", "不支持此播放源", "取消");
                _videoSourceList.RemoveAll(x => x.Value == _sourceVal);
                return;
            }


            lblProgress.Text = "正在解析ts地址";
            List<string> tsFiles = new List<string>();
            try
            {
                tsFiles = await _dilidiliPCSourceItemService.GetTsVideos(mu38);
                if (tsFiles.Count <= 0)
                {
                    await DisplayAlert("提示", $"不支持此播放源[{(_videoSourceList.FirstOrDefault(x => x.Value == _sourceVal)?.Content)}]或MU38地址解析出错", "取消");
                    _videoSourceList.RemoveAll(x => x.Value == _sourceVal);
                    return;
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", ex.ToString(), "取消");
            }

            if (!tsFiles.Any())
            {
                lblProgress.Text = "解析失败";
                return;
            }

            lblProgress.Text = "解析成功";

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