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
        if (await DisplayAlert("�Ƿ���ת��", $"��ת�����飬ѡ���Ͳ���Դ", "ȷ��", "ȡ��"))
        {
            await dilidiliPCSourceservice.SaveAsync(item);

            await Navigation.PushAsync(_dilidiliDetail);
            await _dilidiliDetail.Init(item);
        }
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

            var dilidiliPCSourcesDb = await dilidiliPCSourceservice.FindLikeNameAsync(txtName.Text?.Trim());

            var dilidiliPCSourcesNet = await dilidiliPCSourceservice.AnalysisAsync(name);
            if (!dilidiliPCSourcesNet.Any())
            {
                await DisplayAlert("��ʾ", "����û�н��", "ȡ��");
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

            this.sourceList.ItemsSource = null;
            this.sourceList.ItemsSource = dilidiliPCSourcesNet;
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
        if (await DisplayAlert("ȷ�����?", "�����������ɻָ���", "ȷ��", "ȡ��"))
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

}