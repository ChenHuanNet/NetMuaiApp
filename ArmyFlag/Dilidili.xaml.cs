using Army.Service;
using Army.Service.ApiClients;
using Army.Infrastructure.Extensions;
using Army.Domain.Consts;
using Army.Infrastructure.Models;
using Army.Domain.Models;
using Snowflake.Core;
using System.Collections.ObjectModel;
using ArmyFlag.ViewModels;

namespace ArmyFlag;

public partial class Dilidili : ContentPage
{

    private readonly DilidiliPCSourceService dilidiliPCSourceservice;
    private readonly IdWorker _idWorker;
    private readonly DilidiliDetail _dilidiliDetail;
    private readonly string _searchUrl;

    private VideoViewModel VideoViewModel { get; set; } = new VideoViewModel();

    public Dilidili(DilidiliPCSourceService dilidiliPCSourceService, IDilidiliSourceApi dilidiliSourceApi, IdWorker idWorker, DilidiliDetail dilidiliDetail)
    {
        InitializeComponent();

        dilidiliPCSourceservice = dilidiliPCSourceService;
        _idWorker = idWorker;
        _dilidiliDetail = dilidiliDetail;

        this.sourceList.ItemsSource = VideoViewModel.DilidiliPCSourcesOb;
    }



    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var item = (DilidiliPCSource)e.Item;
        if (await DisplayAlert("是否跳转？", $"跳转到详情，选集和播放源", "确定", "取消"))
        {
            await dilidiliPCSourceservice.SaveAsync(item);

            await Navigation.PushAsync(_dilidiliDetail);
            await _dilidiliDetail.Init(item);
        }
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
    }


    private void Loading(bool isLoading)
    {

        loadingIndicator.IsRunning = isLoading;
        loadingIndicator.IsVisible = isLoading;
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnNetSearch_Clicked(object sender, EventArgs e)
    {
        try
        {
            Loading(true);
            string name = txtName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Loading(false);
                await DisplayAlert("提示", "请输入搜索内容", "取消");
                return;
            }

            var dilidiliPCSourcesDb = await dilidiliPCSourceservice.FindLikeNameAsync(txtName.Text?.Trim());

            var dilidiliPCSourcesNet = await dilidiliPCSourceservice.AnalysisAsync(name);
            if (!dilidiliPCSourcesNet.Any())
            {
                await DisplayAlert("提示", "搜索没有结果", "取消");
                Loading(false);
                return;
            }

            if (dilidiliPCSourcesDb.Any())
            {
                dilidiliPCSourcesNet.ForEach((item) =>
                {
                    var dbItem = dilidiliPCSourcesDb.FirstOrDefault(x => x.Name == item.Name);
                    if (dbItem != null)
                    {
                        item.Id = dbItem.Id;
                    }
                });
            }

            dilidiliPCSourcesNet.Reverse();
            foreach (var item in dilidiliPCSourcesNet)
            {
                VideoViewModel.DilidiliPCSourcesOb.Insert(0, item);
            }


            Loading(false);
        }
        catch (Exception ex)
        {
            await DisplayAlert("错误", ex.ToString(), "取消");
        }

    }

    /// <summary>
    /// 通知表数据 会清掉所有的
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("确认清空?", "数据和缓存文件都将清理，不可恢复！", "确认", "取消"))
        {
            try
            {
                await dilidiliPCSourceservice.ResetDataAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("错误", ex.ToString(), "取消");
            }

        }
    }

}