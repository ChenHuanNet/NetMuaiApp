using Army.Domain.Models;
using Army.Service;

namespace ArmyFlag;

public partial class DilidiliDetail : ContentPage
{

    private readonly DilidiliPCSourceItemService _dilidiliPCSourceItemService;
    private DilidiliPCSource _dilidiliPCSource;
    private List<DilidiliPCSourceItem> _dilidiliPCSourceItems;
    private List<DilidiliPCSourceItem> _showPCSourceItems;

    public DilidiliDetail(DilidiliPCSourceItemService dilidiliPCSourceItemService)
    {
        InitializeComponent();
        _dilidiliPCSourceItemService = dilidiliPCSourceItemService;
    }




    public async Task Init(DilidiliPCSource dilidiliPCSource)
    {
        _dilidiliPCSource = dilidiliPCSource;
        this.Title = _dilidiliPCSource.Name;
        lblSource.Text = $"{_dilidiliPCSource.PlaySource},共({dilidiliPCSource.CurrentMaxNum})";

        await AnalysisAsync();
    }

    public async Task AnalysisAsync()
    {
        var ids = _dilidiliPCSource.Url.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var mediaId = ids[ids.Length - 2];
        var detailId = ids[ids.Length - 1].Split('.')[0];

        _dilidiliPCSourceItems = await _dilidiliPCSourceItemService.AnalysisAsync(_dilidiliPCSource.Id, _dilidiliPCSource.PlaySource, mediaId, detailId);

        await DisplayAlert("提示", "数据加载完成", "取消");
    }

    private void sourceItemList_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private void btnSearch_Clicked(object sender, EventArgs e)
    {
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
}