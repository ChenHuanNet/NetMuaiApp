using Army.Service;
using Army.Service.ApiClients;
using Army.Infrastructure.Extensions;
using Army.Domain.Consts;
using Army.Infrastructure.Models;
using Army.Domain.Models;
using Snowflake.Core;

namespace ArmyFlag;

public partial class Dilidili : ContentPage
{

    private readonly DilidiliPCSourceService _dilidiliPCSourceService;
    private readonly IdWorker _idWorker;
    private readonly DilidiliDetail _dilidiliDetail;

    private List<DilidiliPCSource> _dilidiliPCSources = new List<DilidiliPCSource>();

    public Dilidili(DilidiliPCSourceService dilidiliPCSourceService, IDilidiliSourceApi dilidiliSourceApi, IdWorker idWorker, DilidiliDetail dilidiliDetail)
    {
        InitializeComponent();

        _dilidiliPCSourceService = dilidiliPCSourceService;
        _idWorker = idWorker;
        _dilidiliDetail = dilidiliDetail;
    }




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
        _dilidiliPCSources = await _dilidiliPCSourceService.FindLikeNameAsync(txtName.Text?.Trim());
        if (!_dilidiliPCSources.Any())
        {
            await DisplayAlert("提示", "本地没有当前搜索结果", "取消");
            return;
        }
        sourceList.ItemsSource = _dilidiliPCSources;
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
    }

    private void btnClear2_Clicked(object sender, EventArgs e)
    {
        txtUrl.Text = string.Empty;
    }

    /// <summary>
    /// 解析嘀哩嘀哩 pc web网页
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnAnalysis_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUrl.Text))
        {
            await DisplayAlert("提示", "请输入媒体ID", "取消");
            return;
        }
        _dilidiliPCSources = await _dilidiliPCSourceService.AnalysisAsync(txtUrl.Text?.Trim());
        this.sourceList.ItemsSource = _dilidiliPCSources;
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
            await _dilidiliPCSourceService.ResetDataAsync();
        }
    }

    /// <summary>
    /// 保存解析后的数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (_dilidiliPCSources.Count > 0)
        {
            await _dilidiliPCSourceService.SaveAsync(_dilidiliPCSources);
        }
    }
}