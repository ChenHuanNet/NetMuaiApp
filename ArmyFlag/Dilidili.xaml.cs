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
    }


    private void Loading(bool isLoading)
    {

        loadingIndicator.IsRunning = isLoading;
        loadingIndicator.IsVisible = isLoading;
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
            Loading(true);
            string name = txtName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Loading(false);
                await DisplayAlert("��ʾ", "��������������", "ȡ��");
                return;
            }

            var dilidiliPCSourcesDb = await dilidiliPCSourceservice.FindLikeNameAsync(txtName.Text?.Trim());

            var dilidiliPCSourcesNet = await dilidiliPCSourceservice.AnalysisAsync(name);
            if (!dilidiliPCSourcesNet.Any())
            {
                await DisplayAlert("��ʾ", "����û�н��", "ȡ��");
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
        if (await DisplayAlert("ȷ�����?", "���ݺͻ����ļ������������ɻָ���", "ȷ��", "ȡ��"))
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