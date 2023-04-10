namespace ArmyFlag;

public partial class MainPage : ContentPage
{
    int count = 0;

    string[] res = new string[2];

    public MainPage()
    {
        InitializeComponent();
    }


    private void Scan_Clicked(object sender, EventArgs e)
    {
        OpenScanWindows(0);
    }


    private void SetData(int i, string text)
    {
        res[i] = text;
        DisplayAlert(i.ToString(), "扫描成功", "取消");

        if (!string.IsNullOrWhiteSpace(res[0]) && !string.IsNullOrWhiteSpace(res[1]))
        {
            var ok1 = int.TryParse(res[0], out int num1);
            var ok2 = int.TryParse(res[1], out int num2);
            if (!ok1 || !ok2)
            {
                DisplayAlert(i.ToString(), "扫描结果有问题，请重新扫描", "取消");
                return;
            }

            Result.Text = num1 > num2 ? "结果1大" : (num1 != num2 ? "结果2大" : "一样大");

            DisplayAlert(i.ToString(), Result.Text, "取消");
        }
    }

    private void Scan2_Clicked(object sender, EventArgs e)
    {
        OpenScanWindows(1);
    }

    private async void OpenScanWindows(int i)
    {
        BarcodeQrCode barcodeQrCode = new BarcodeQrCode(i);
        barcodeQrCode.SetBarcodeResult += SetData;

        await Navigation.PushModalAsync(barcodeQrCode);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        res = new string[2];
    }
}

