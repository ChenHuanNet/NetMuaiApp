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
    private List<string> _tsFiles = new List<string>();

    private string _localVideoUrl;

    private long _sourceId;
    private string _source;
    private DilidiliPCSourceItem _current;

    public DilidiliVideo(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }

    public async Task Init(DilidiliPCSourceItem item)
    {
        _current = item;
        _localVideoUrl = item.Url;
        lblNum.Text = $"{item.Name} {item.Num}";
        mediaElement.Source = new Uri(_localVideoUrl);
        mediaElement.Play();
        lblProgress.Text = $"解析完成";
    }


    public async Task Init(long sourceId, string source, string num, List<string> tsFiles, string name)
    {
        _sourceId = sourceId;
        _source = source;
        _tsFiles = tsFiles;

        lblNum.Text = $"{name} {num}";

        try
        {
            lblProgress.Text = $"正在下载资源0/{_tsFiles.Count}";
            _tsFiles = await _dilidiliPCSourceItemService.DownloadVideos(_sourceId, _source, num, _tsFiles, (index, total) =>
            {
                lblProgress.Text = $"正在下载资源{index}/{total}";
            });

            lblProgress.Text = $"正在解析资源0/{_tsFiles.Count}";
            _localVideoUrl = _dilidiliPCSourceItemService.MergeTsVideo(_sourceId, _source, num, _tsFiles, (index, total) =>
            {
                lblProgress.Text = $"正在解析资源{index}/{total}";
            });

            mediaElement.Source = new Uri(_localVideoUrl);
            mediaElement.Play();

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

            lblProgress.Text = $"解析完成";
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", ex.ToString(), "取消");
        }
    }




    private void Button_Clicked_1(object sender, EventArgs e)
    {
        _dilidiliPCSourceItemService.ClearFileCache(_sourceId);
    }
}