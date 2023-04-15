using Army.Domain.Models;
using Army.Service;

namespace ArmyFlag;

public partial class DilidiliDetail : ContentPage
{

    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private readonly DilidiliVideo _dilidiliVideo;
    private DilidiliPCSource _dilidiliPCSource;
    private List<DilidiliPCSourceItem> _dilidiliPCSourceItems;
    private List<DilidiliPCSourceItem> _showPCSourceItems;

    private bool isInit = false;

    public DilidiliDetail(DilidiliPCSourceItemService dilidiliPCSourceItemService, DilidiliVideo dilidiliVideo)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
        _dilidiliVideo = dilidiliVideo;
    }




    public async Task Init(DilidiliPCSource dilidiliPCSource)
    {
        _dilidiliPCSource = dilidiliPCSource;
        this.Title = _dilidiliPCSource.Name;
        lblSource.Text = $"{_dilidiliPCSource.PlaySource},共({dilidiliPCSource.CurrentMaxNum})";
        isInit = false;
        await AnalysisAsync();
    }

    public async Task AnalysisAsync()
    {
        var ids = _dilidiliPCSource.Url.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var mediaId = ids[ids.Length - 2];
        var detailId = ids[ids.Length - 1].Split('.')[0];

        _dilidiliPCSourceItems = await _dilidiliPCSourceItemService.AnalysisAsync(_dilidiliPCSource.Id, _dilidiliPCSource.PlaySource, mediaId, detailId);

        await DisplayAlert("提示", "数据加载完成", "取消");
        isInit = true;
    }

    private async void sourceItemList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var item = (DilidiliPCSourceItem)e.Item;
        await Navigation.PushAsync(_dilidiliVideo);
        await _dilidiliVideo.Init(item);
    }

    private void btnSearch_Clicked(object sender, EventArgs e)
    {
        if (!isInit)
        {
            return;
        }
        int.TryParse(txtName.Text?.Trim(), out int num);
        if (num == 0)
        {
            _showPCSourceItems = _dilidiliPCSourceItems.Take(10).ToList();
        }
        else
        {
            _showPCSourceItems = _dilidiliPCSourceItems.Where(x => x.Sort > num - 5 && x.Sort < num + 5).OrderByDescending(x => x.Sort).ToList();
        }
        sourceItemList.ItemsSource = _showPCSourceItems;
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (!isInit)
        {
            return;
        }
        await _dilidiliPCSourceItemService.SaveAsync(_dilidiliPCSourceItems);
        await DisplayAlert("提示", "保存完成", "取消");
    }
}