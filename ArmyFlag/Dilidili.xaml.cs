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

    private ObservableCollection<DilidiliPCSource> DilidiliPCSourcesOb { get; set; }

    public Dilidili(DilidiliPCSourceService dilidiliPCSourceService, IDilidiliSourceApi dilidiliSourceApi, IdWorker idWorker, DilidiliDetail dilidiliDetail)
    {
        InitializeComponent();

        dilidiliPCSourceservice = dilidiliPCSourceService;
        _idWorker = idWorker;
        _dilidiliDetail = dilidiliDetail;

    }


    private void btnClear_Clicked(object sender, EventArgs e)
    {
        txtName.Text = string.Empty;
        this.sourceList.ItemsSource = null;
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

            DilidiliPCSourcesOb = new ObservableCollection<DilidiliPCSource>(dilidiliPCSourcesNet);
            this.sourceList.ItemsSource = dilidiliPCSourcesNet;

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

    private async void sourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count <= 0)
        {
            return;
        }
        var item = (DilidiliPCSource)e.CurrentSelection[0];
        if (await DisplayAlert("�Ƿ���ת��", $"��ת�����飬ѡ���Ͳ���Դ", "ȷ��", "ȡ��"))
        {
            await dilidiliPCSourceservice.SaveAsync(item);

            await Navigation.PushAsync(_dilidiliDetail);
            await _dilidiliDetail.Init(item);
        }
    }
}