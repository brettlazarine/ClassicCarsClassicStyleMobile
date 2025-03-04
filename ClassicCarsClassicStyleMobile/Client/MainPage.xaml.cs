using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Shiny.BluetoothLE;
using Zeroconf;

namespace Client;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly IBleManager _bleManager;
    public ObservableCollection<ScanResult> Results { get; } = new();
    
    private string _piStatus;
    public string PiStatus
    {
        get => _piStatus;
        set
        {
            _piStatus = value;
            OnPropertyChanged(nameof(PiStatus)); // Notify UI of changes
        }
    }
    
    public MainPage(IBleManager bleManager)
    {
        _bleManager = bleManager;
        BindingContext = this;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Scan();
            await ProbeForRaspberryPi();
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
                Debug.WriteLine($"***** Scanned for id: {_result.Peripheral.Uuid} && name: {_result.Peripheral.Name} *****");
                
                if (!Results.Any(x => x.Peripheral.Uuid.Equals(_result.Peripheral.Uuid)))
                {
                    Results.Add(_result);
                }
            });
    }

    public async Task ProbeForRaspberryPi()
    {
        IReadOnlyList<IZeroconfHost> results = 
            await ZeroconfResolver.ResolveAsync("_http._tcp.local.", TimeSpan.FromSeconds(5), 10, 2000);
        
        foreach (var result in results)
        {
            Debug.WriteLine($"***** Zeroconf Host: {result.DisplayName} *****");
            Debug.WriteLine($"***** Zeroconf IP: {result.IPAddress} *****");
            Debug.WriteLine($"***** Zeroconf Id: {result.Id} *****");

            if (result.DisplayName.Contains("cccs"))
            {
                Preferences.Set("RPiIP", result.IPAddress);
                Debug.WriteLine($"***** Preferences Set: {result.IPAddress} *****");
            }
        }
    }

    public async void VerifyRPiConnectionCommand(object? sender, EventArgs eventArgs)
    {
        var piIp = Preferences.Get("RPiIP", string.Empty);
        Debug.WriteLine($"***** Pi IP: {piIp} *****");
        if (string.IsNullOrEmpty(piIp))
        {
            PiStatus = "No Raspberry Pi IP found";
            return;
        }

        using HttpClient client = new();
        var url = $"http://{piIp}:8080/";
        Debug.WriteLine($"***** URL: {url} *****");

        try
        {
            var res = await client.GetAsync(url);
            Debug.WriteLine($"***** Pi API Verify Results: {res.StatusCode} *****");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("***** MainPage VerifyRPiConnectionCommand() *****");
            Debug.WriteLine(ex.Message);
            Debug.WriteLine("********************");
        }
    }
}