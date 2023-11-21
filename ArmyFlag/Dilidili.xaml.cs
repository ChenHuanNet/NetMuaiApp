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
    /// ����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        var dilidiliPCSources = await dilidiliPCSourceservice.FindLikeNameAsync(txtName.Text?.Trim());
        if (!dilidiliPCSources.Any())
        {
            await DisplayAlert("��ʾ", "����û�е�ǰ�������", "ȡ��");
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
    /// ����
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
                await DisplayAlert("��ʾ", "��������������", "ȡ��");
                return;
            }
            var dilidiliPCSources = await dilidiliPCSourceservice.AnalysisAsync(name);
            if (!dilidiliPCSources.Any())
            {
                await DisplayAlert("��ʾ", "��������û�н��", "ȡ��");
                return;
            }
            this.sourceList.ItemsSource = null;
            this.sourceList.ItemsSource = dilidiliPCSources;
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", ex.ToString(), "ȡ��");
        }

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
            try
            {
                await dilidiliPCSourceservice.ResetDataAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("����", ex.ToString(), "ȡ��");
            }

        }
    }

    /// <summary>
    /// ��������������
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