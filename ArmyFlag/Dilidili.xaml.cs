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
    private readonly IDilidiliSourceApi _dilidiliSourceApi;
    private readonly IdWorker _idWorker;

    private List<DilidiliPCSource> _dilidiliPCSources = new List<DilidiliPCSource>();

    public Dilidili(DilidiliPCSourceService dilidiliPCSourceService, IDilidiliSourceApi dilidiliSourceApi, IdWorker idWorker)
    {
        InitializeComponent();

        _dilidiliPCSourceService = dilidiliPCSourceService;
        _dilidiliSourceApi = dilidiliSourceApi;
        _idWorker = idWorker;
    }


    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (await DisplayAlert("是否跳转？", "跳转到详情查看或继续解析！", "确定", "取消"))
        {

        }
    }

    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        var source = await _dilidiliPCSourceService.FindLikeNameAsync(txtName.Text.Trim());
        sourceList.ItemsSource = source;
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
    }

    private void btnClear2_Clicked(object sender, EventArgs e)
    {
        txtUrl.Text = string.Empty;
    }


    private async void btnAnalysis_Clicked(object sender, EventArgs e)
    {
        string prefix = $"{AppConfigHelper.DiliDiliSourceHost}/play/{txtUrl.Text.Trim()}/";

        var html = await _dilidiliSourceApi.GetSourceHtml(txtUrl.Text.Trim());

        string title = html.GetFirstHtmlWithAttr("div", "class", "title").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();

        var sources = html.GetHtmlWithAttr("li", "class", "nav-item");
        var contents = html.GetHtmlWithAttr("div", "role", "tabpanel");

        _dilidiliPCSources = new List<DilidiliPCSource>();
        DilidiliPCSource dilidiliPCSource = null;
        for (int i = 0; i < sources.Count; i++)
        {

            string playSource = sources[i].Trim().GetHtmlLabel("a").RemoveHtmlLabel().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
            List<HtmlALabel> list = new List<HtmlALabel>();
            if (contents.Count > i)
            {
                list = contents[i].GetAHrefAndText();
                list = list.Where(x => x.Href.Contains(prefix)).ToList();
            }

            dilidiliPCSource = new DilidiliPCSource()
            {
                Id = 0,
                Name = title,
                PlaySource = playSource,
                Url = list.Last().Href
            };

            _dilidiliPCSources.Add(dilidiliPCSource);
        }

        this.sourceList.ItemsSource = _dilidiliPCSources;
    }

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