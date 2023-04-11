namespace ArmyFlag;

public partial class MainPage : ContentPage
{
    string[] res = new string[2];

    Dictionary<int, string> armyFlag = new Dictionary<int, string>();

    private BarcodeQrCode barcodeQrCode;
    public MainPage()
    {
        InitializeComponent();

        #region 填充数据
        armyFlag = new Dictionary<int, string>();
        armyFlag.Add(40, "司令");
        armyFlag.Add(39, "军长");
        armyFlag.Add(38, "师长");
        armyFlag.Add(37, "旅长");
        armyFlag.Add(36, "团长");
        armyFlag.Add(35, "营长");
        armyFlag.Add(34, "连长");
        armyFlag.Add(33, "排长");
        armyFlag.Add(32, "工兵");
        armyFlag.Add(0, "炸弹");
        armyFlag.Add(99, "地雷");
        armyFlag.Add(1, "军旗");

        #endregion
    }


    private void Scan_Clicked(object sender, EventArgs e)
    {
        OpenScanWindows(0);
    }


    private async void SetData(int i, string text)
    {
        res[i] = text;

        barcodeQrCode.isOk = true;
        await DisplayAlert(i.ToString(), "扫描成功", "取消");

        await Navigation.PopAsync();

        if (!string.IsNullOrWhiteSpace(res[0]) && !string.IsNullOrWhiteSpace(res[1]))
        {
            var ok1 = int.TryParse(res[0], out int num1);
            var ok2 = int.TryParse(res[1], out int num2);
            if (!ok1 || !ok2)
            {
                await DisplayAlert(i.ToString(), "扫描结果有问题，请重新扫描", "取消");
                return;
            }

            string msg = "";
            if (num1 == 0 || num2 == 0)
            {
                if (num1 == 40 || num2 == 40)
                {
                    //有炸弹就是一样大
                    msg = $"{(num1 == 40 ? "棋子1的" : "")}{(num2 == 40 ? "棋子2的" : "")}司令死了";
                }
                else
                {
                    //有炸弹就是一样大
                    msg = "一样大";
                }
            }
            else if (num1 == 32 && num2 == 99)
            {
                //工兵吃地雷
                msg = "结果1大";
            }
            else if (num1 == 99 && num2 == 32)
            {
                //工兵吃地雷
                msg = "结果2大";
            }
            else
            {
                msg = num1 > num2 ? "结果1大" : (num1 != num2 ? "结果2大" : "一样大");
            }

            Result.Text = msg;

            if (ShowDetail.IsChecked)
            {
                Detail.Text = $"棋子1：{(armyFlag.ContainsKey(num1) ? armyFlag[num1] : num1.ToString())}，棋子2：{(armyFlag.ContainsKey(num2) ? armyFlag[num2] : num2.ToString())}";
            }
            else
            {
                Detail.Text = "";
            }

            await DisplayAlert(i.ToString(), Result.Text, "取消");

        }

    }

    private void Scan2_Clicked(object sender, EventArgs e)
    {
        OpenScanWindows(1);
    }

    private async void OpenScanWindows(int i)
    {
        barcodeQrCode = new BarcodeQrCode(i);
        barcodeQrCode.SetBarcodeResult += SetData;

        await Navigation.PushModalAsync(barcodeQrCode);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        res = new string[2];
        Result.Text = "";
        Detail.Text = "";
    }
}

