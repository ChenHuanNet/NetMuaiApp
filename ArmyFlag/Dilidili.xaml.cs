using Army.Service;
using Army.Service.ApiClients;
using Army.Infrastructure.Extensions;
using Army.Domain.Consts;
using Army.Infrastructure.Models;
using Army.Domain.Models;
using Snowflake.Core;
using System.Collections.ObjectModel;

namespace ArmyFlag;

public partial class Dilidili : ContentPage
{

    private readonly DilidiliPCSourceService dilidiliPCSourceservice;
    private readonly IdWorker _idWorker;
    private readonly DilidiliDetail _dilidiliDetail;
    private readonly string _searchUrl;


    public Dilidili(DilidiliPCSourceService dilidiliPCSourceService, IDilidiliSourceApi dilidiliSourceApi, IdWorker idWorker, DilidiliDetail dilidiliDetail)
    {
        InitializeComponent();

        dilidiliPCSourceservice = dilidiliPCSourceService;
        _idWorker = idWorker;
        _dilidiliDetail = dilidiliDetail;

    }



    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var item = (DilidiliPCSource)e.Item;
        if (await DisplayAlert("是否跳转？", $"跳转到详情查看或继续解析！{(item.Id == 0 ? "当前数据尚未保存，会导致后续解析也无法保存，确定继续跳转？" : "")}", "确定", "取消"))
        {
            await Navigation.PushAsync(_dilidiliDetail);
            await _dilidiliDetail.Init(item);
        }
    }

    /// <summary>
    /// 查询本地库
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            await DisplayAlert("提示", "请输入搜索内容", "取消");
            return;
        }
        var dilidiliPCSources = await dilidiliPCSourceservice.FindLikeNameAsync(txtName.Text?.Trim());
        if (!dilidiliPCSources.Any())
        {
            await DisplayAlert("提示", "本地没有当前搜索结果", "取消");
            return;
        }
        this.sourceList.ItemsSource = null;
        this.sourceList.ItemsSource = dilidiliPCSources;
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
        this.sourceList.ItemsSource = null;
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
            string name = txtName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("提示", "请输入搜索内容", "取消");
                return;
            }
            var dilidiliPCSources = await dilidiliPCSourceservice.AnalysisAsync(name);
            if (!dilidiliPCSources.Any())
            {
                await DisplayAlert("提示", "网络搜索没有结果", "取消");
                return;
            }
            this.sourceList.ItemsSource = null;
            this.sourceList.ItemsSource = dilidiliPCSources;
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
        if (await DisplayAlert("确认重置?", "重置后，数据将清理，不可恢复！", "确认", "取消"))
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

    /// <summary>
    /// 保存解析后的数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var data = this.sourceList.ItemsSource;
        if (data != null && data is List<DilidiliPCSource> list)
        {
            await dilidiliPCSourceservice.SaveAsync(list);
        }
    }
}