using System;
using System.Xml.Linq;


namespace ArmyFlag;

public partial class BarcodeQrCode : ContentPage
{

    public event Action<int, string> SetBarcodeResult;
    private int _num;
    public bool isOk = false;


    public BarcodeQrCode(int i)
    {
        _num = i;
        InitializeComponent();
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        if (isOk)
        {
            return;
        }
        Dispatcher.Dispatch(() =>
        {
            SetBarcodeResult.Invoke(_num, e.Results[0].Value);
        });
    }
}

