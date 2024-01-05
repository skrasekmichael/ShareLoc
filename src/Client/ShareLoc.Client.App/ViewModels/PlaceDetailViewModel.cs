using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ShareLoc.Client.App.Models;
using ShareLoc.Shared.Common.Models;

namespace ShareLoc.Client.App.ViewModels;

public sealed partial class PlaceDetailViewModel : BaseViewModel
{
	private PlaceModel? _placeModel = null;
	public PlaceModel? Model
	{
		get => _placeModel;
		set
		{
			SetProperty(ref _placeModel, value);

			if (value is null)
				return;

			Dispatch(() => MapWebView.Source = new HtmlWebViewSource
			{
				Html = GenerateHtml(value.Longitude, value.Latitude, value.LocalId == Guid.Empty)
			});
		}
	}

	[ObservableProperty]
	private bool _isEnlarged = false;

	[RelayCommand]
	private void TapImage() => IsEnlarged = !IsEnlarged;

	public WebView MapWebView { get; } = new();

	public async Task ShowGuessAsync(GuessModel guess)
	{
		await MapWebView.EvaluateJavaScriptAsync($$"""mapDisplayGuessResult('{{guess.Longitude}}', '{{guess.Latitude}}');""");
	}

	public async Task HideGuessAsync()
	{
		await MapWebView.EvaluateJavaScriptAsync("showLocation();");
	}

	private static string GenerateHtml(double longitude, double latitude, bool moveable) => $$$"""
			<![CDATA[
			<html>
				<script src="https://api.mapy.cz/loader.js"></script>
				<script>
					function getMarkerObject(color) {
						const svgCode = `<svg fill="${color}" viewBox="0 0 1920 1920" width="30" height="30" xmlns="http://www.w3.org/2000/svg" stroke="#00ff40"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M956.952 0c-362.4 0-657 294.6-657 656.88 0 180.6 80.28 347.88 245.4 511.56 239.76 237.96 351.6 457.68 351.6 691.56v60h120v-60c0-232.8 110.28-446.16 357.6-691.44 165.12-163.8 245.4-331.08 245.4-511.68 0-362.28-294.6-656.88-663-656.88" fill-rule="evenodd"></path></g></svg>`;
						const marker = JAK.mel("div");
						marker.innerHTML = svgCode;
						return marker;
					}

					function getMarker(coords, color) {
						return new SMap.Marker(coords, null, {
							url: getMarkerObject(color),
							anchor: { left: 14, bottom: 1 }
						});
					}

					function loadMap() {
						correctCoords = SMap.Coords.fromWGS84('{{{longitude}}}', '{{{latitude}}}');
						map = new SMap(JAK.gel("map"), correctCoords, 12);

						map.addDefaultLayer(SMap.DEF_BASE).enable();
						{{{(moveable ? "map.addDefaultControls();" : "")}}}

						const sync = new SMap.Control.Sync();
						map.addControl(sync);

						targetLayer = new SMap.Layer.Marker();
						map.addLayer(targetLayer);
						targetLayer.enable();

						markersLayer = new SMap.Layer.Marker();
						map.addLayer(markersLayer);
						markersLayer.enable();

						geometryLayer = new SMap.Layer.Geometry();
						map.addLayer(geometryLayer);
						geometryLayer.enable();
		
						const placeMarker = getMarker(correctCoords, "#ff0051");
						targetLayer.addMarker(placeMarker);
					}

					function showLocation() {
						markersLayer.removeAll();
						geometryLayer.removeAll();
						map.setCenterZoom(correctCoords, 12, true);
					}

					function mapDisplayGuessResult(long, lat) {
						const guessCoords = SMap.Coords.fromWGS84(long, lat);

						markersLayer.removeAll();
						geometryLayer.removeAll();
						map.syncPort();

						const guessMarker = getMarker(guessCoords, "#1d3746");
						markersLayer.addMarker(guessMarker);

						const [centerCoords, zoom] = map.computeCenterZoom([guessCoords, correctCoords], false);
						const distance = guessCoords.distance(correctCoords);

						const points = calculateCurvePoints(guessCoords, centerCoords, correctCoords, distance);
						const line = new SMap.Geometry(SMap.GEOMETRY_POLYLINE, null, points, {
							color: "black",
							width: 2,
							style: 1
						});

						geometryLayer.addGeometry(line);

						map.setCenterZoom(centerCoords, zoom, true);
					}

					function calculateCurvePoints(startPoint, middlePoint, endPoint, distance) {
						const numPoints = Math.ceil(distance / 20000);
						const curvePoints = [];

						for (let i = 0; i <= numPoints; i++) {
							const t = i / numPoints;
							const x = interpolate(startPoint.x, middlePoint.x, endPoint.x, t);
							const y = interpolate(startPoint.y, middlePoint.y, endPoint.y, t);
							curvePoints.push(new SMap.Coords(x, y));
						}

						return curvePoints;
					}

					function interpolate(start, middle, end, t) {
						return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * middle + t * t * end;
					}

					Loader.lang = "en";
					Loader.load();

					window.onload = async function() { loadMap(); };
				</script>
				<body style="margin: 0">
					<div id="map" style="width:100%; height:100%;"></div>
				</body>
			</html>
			""";
}
