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
        if (await DisplayAlert("�Ƿ���ת��", $"��ת������鿴�����������{(item.Id == 0 ? "��ǰ������δ���棬�ᵼ�º�������Ҳ�޷����棬ȷ��������ת��" : "")}", "ȷ��", "ȡ��"))
        {
            await Navigation.PushAsync(_dilidiliDetail);
            await _dilidiliDetail.Init(item);
        }
    }

    /// <summary>
    /// ��ѯ���ؿ�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnSearch_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            await DisplayAlert("��ʾ", "��������������", "ȡ��");
            return;
        }
        _dilidiliPCSources = await _dilidiliPCSourceService.FindLikeNameAsync(txtName.Text?.Trim());
        if (!_dilidiliPCSources.Any())
        {
            await DisplayAlert("��ʾ", "����û�е�ǰ�������", "ȡ��");
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
    /// ������������ pc web��ҳ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnAnalysis_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUrl.Text))
        {
            await DisplayAlert("��ʾ", "������ý��ID", "ȡ��");
            return;
        }
        _dilidiliPCSources = await _dilidiliPCSourceService.AnalysisAsync(txtUrl.Text?.Trim());
        this.sourceList.ItemsSource = _dilidiliPCSources;
    }

    /// <summary>
    /// ֪ͨ������ ��������е�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("ȷ������?", "���ú����ݽ��������ɻָ���", "ȷ��", "ȡ��"))
        {
            await _dilidiliPCSourceService.ResetDataAsync();
        }
    }

    /// <summary>
    /// ��������������
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