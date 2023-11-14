using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Services;

using ShareLoc.Client.BL.Services;
using ShareLoc.Shared.Common.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace ShareLoc.Client.App.ViewModels;
public sealed partial class CreatePlaceViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IAlertService _alertService;
	private readonly ApiClient _apiClient;
	private readonly LocalDbService _localDbService;

	[ObservableProperty]
	public ImageSource? photo;
	[ObservableProperty]
	public string mapHtml = "";
	[ObservableProperty]
	public string message = "Guess where I am!";

	public Location? Location
	{
		get => _location;
		set
		{
			_location = value;
			MapHtml = GenerateHtml();
		}
	}
	private Location? _location;
	private byte[]? _photoBytes;
	private PlaceRequest? _placeRequest;
	private Guid? _localPlaceId;

	public CreatePlaceViewModel(ApiClient apiClient, INavigationService navigationService, LocalDbService localDbService, IAlertService alertService)
	{
		_apiClient = apiClient;
		_navigationService = navigationService;
		_localDbService = localDbService;
		_alertService = alertService;
	}

	protected override async Task LoadAsync(CancellationToken ct)
	{
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			Location = await GetLocation();
			Photo = await TakePhoto();

		});

		if (Location is null)
		{
			await _alertService.ShowAlertAsync("Error", "Location not found", "OK");
			await _navigationService!.ReturnToRootAsync();
			return;
		}
		if (Photo is null)
		{
			await _navigationService!.ReturnToRootAsync();
			return;
		}

		_placeRequest = new PlaceRequest()
		{
			Image = _photoBytes!,
			Latitude = Location!.Latitude,
			Longitude = Location!.Longitude,
			Message = Message,
		};

		_localPlaceId = await SavePlaceRequest();
		if (_localPlaceId is null)
		{
			await _alertService.ShowAlertAsync("Error", "Error while creating place", "OK");
			await _navigationService!.ReturnToRootAsync();
			return;
		}
	}

	private async Task<Guid?> SavePlaceRequest()
	{
		var response = await _localDbService.SavePlaceAsync(_placeRequest!);
		if (!response.IsT0)
		{
			return null;
		}

		return response.AsT0.Value;
	}

	private string GenerateHtml()
	{
		return $@"
		<![CDATA[
            <html>
                <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
	            <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
                <script src=""https://api.mapy.cz/loader.js""></script>
                <script>
                    function getMarkerObject(color) {{
	                    const svgCode = `<svg fill=""${{color}}"" viewBox=""0 0 1920 1920"" width=""30"" height=""30"" xmlns=""http://www.w3.org/2000/svg"" stroke=""#00ff40""><g id=""SVGRepo_bgCarrier"" stroke-width=""0""></g><g id=""SVGRepo_tracerCarrier"" stroke-linecap=""round"" stroke-linejoin=""round""></g><g id=""SVGRepo_iconCarrier""> <path d=""M956.952 0c-362.4 0-657 294.6-657 656.88 0 180.6 80.28 347.88 245.4 511.56 239.76 237.96 351.6 457.68 351.6 691.56v60h120v-60c0-232.8 110.28-446.16 357.6-691.44 165.12-163.8 245.4-331.08 245.4-511.68 0-362.28-294.6-656.88-663-656.88"" fill-rule=""evenodd""></path> </g></svg>`;
	                    const marker = JAK.mel(""div"");
	                    marker.innerHTML = svgCode;
	                    return marker;
                    }}
                        
                    function loadMap() {{
	                    const center = SMap.Coords.fromWGS84('{Location!.Longitude.ToString()}', '{Location!.Latitude.ToString()}');
	                    map = new SMap(JAK.gel(""map""), center, 12);

	                    map.addDefaultLayer(SMap.DEF_BASE).enable();
	                    map.addDefaultControls();

	                    const sync = new SMap.Control.Sync();
	                    map.addControl(sync);

	                    targetLayer = new SMap.Layer.HUD();
	                    map.addLayer(targetLayer);
	                    targetLayer.enable();

						var markerLayer = new SMap.Layer.Marker();
			            map.addLayer(markerLayer);
			            markerLayer.enable();
		
						var znacka = new SMap.Marker(center, null, {{anchor: {{left:10, bottom: 1}}}});
			            markerLayer.addMarker(znacka);
                    }}
                    Loader.lang = ""en"";
                    Loader.load();
                        
                    window.onload = async function() {{loadMap();}};
                </script>
                <body>
                    <div id=""map"" style=""width:100%; height:100%;""></div>
                </body>
            </html>
		";
	}

	public async Task<Location?> GetLocation()
	{
		return await Geolocation.GetLocationAsync();
	}
	public async Task<ImageSource?> TakePhoto()
	{
		if (!MediaPicker.IsCaptureSupported)
		{
			await _alertService.ShowAlertAsync("Error", "Camera not supported", "OK");
			await _navigationService!.ReturnToRootAsync();
			return null;
		}

		var photo = await MediaPicker.CapturePhotoAsync();
		if (photo is null)
		{
			await _alertService.ShowAlertAsync("Error", "Photo could not be taken", "OK");
			await _navigationService!.ReturnToRootAsync();
			return null;
		}

		var stream = await photo.OpenReadAsync();
		_photoBytes = new byte[stream.Length];
		await stream.ReadAsync(_photoBytes, 0, (int)stream.Length);
		return ImageSource.FromStream(() => { stream.Position = 0; return stream; });
	}

	[RelayCommand]
	public async Task ShareUrl()
	{
		var response = await _apiClient.CreatePlaceAsync(_placeRequest!, CancellationToken.None);

		response.Switch(
			async (serverId) =>
			{
				await _localDbService.SharePlaceAsync(_localPlaceId!.Value, serverId, DateTime.UtcNow);
				await Share.RequestAsync(new ShareTextRequest
				{
					// eg. a2934fa2-6f7e-4ac9-8210-681814ac86c4
					Text = $"https://localhost:7058/{serverId}",
					Title = Message
				});
				await _navigationService!.ReturnToRootAsync();
				return;
			},
			async (validationErrors) =>
			{
				await _alertService.ShowAlertAsync("Error", $"Invalid values given: {validationErrors}", "OK");
				return;
			},
			async (error) =>
			{
				await _alertService.ShowAlertAsync("Error", $"Error while creating place: {error}", "OK");
				return;
			}
		);
	}
}
