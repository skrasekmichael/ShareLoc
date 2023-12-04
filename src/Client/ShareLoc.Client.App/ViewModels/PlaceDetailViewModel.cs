using CommunityToolkit.Mvvm.ComponentModel;

using ShareLoc.Client.App.Models;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class PlaceDetailViewModel : BaseViewModel
{
	private PlaceModel? _placeModel = null;
	public PlaceModel? PlaceModel
	{
		get => _placeModel;
		set
		{
			if (value == null) return;

			SetProperty(ref _placeModel, value, nameof(PlaceModel));

			MapHtml = GenerateHtml(value.Longitude, value.Latitude);
			Image = value.Image;
			Message = value.Message;
			IsShared = value.IsShared;
		}
	}

	[ObservableProperty]
	private string? _mapHtml = string.Empty;
	[ObservableProperty]
	private byte[]? _image = null;
	[ObservableProperty]
	private string? _message = "Guess where I am!";
	[ObservableProperty]
	private bool _isShared = false;

	private static string GenerateHtml(double longitude, double latitude)
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
	                    const center = SMap.Coords.fromWGS84('{longitude}', '{latitude}');
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
}
