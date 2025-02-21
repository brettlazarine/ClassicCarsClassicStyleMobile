
using System.Diagnostics;
using Shiny.BluetoothLE;

namespace Client;

public partial class MainPage : ContentPage
{
    private readonly IBleManager _bleManager;
    public List<ScanResult> Results { get; } = new();
    
    public MainPage(IBleManager bleManager)
    {
        _bleManager = bleManager;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Scan();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("***** MainPage OnAppearing() *****");
            Debug.WriteLine(ex.Message);
            Debug.WriteLine("********************");
        }
    }

    public void Scan()
    {
        if (!_bleManager.IsScanning)
        {
            _bleManager.StopScan();
        }
        
        Results.Clear();

        _bleManager.Scan()
            .Subscribe(_result =>
            {
                try
                {
                    Results.Add(_result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("***** MainPage Scan() *****");
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine("********************");
                }
            });
    }
}