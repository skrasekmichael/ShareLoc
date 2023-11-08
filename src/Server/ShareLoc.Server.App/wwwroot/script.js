window.onload = async function() {
	loadMap();

	myId = getCookie("guesserId");
	myName = localStorage.getItem("guesserName") ?? null;
	displayName();

	if (typeof myGuessInfo !== "undefined") {
		const guesses = await fetchGuessesAsync();
		renderGuesses(guesses);

		const correctCoords = SMap.Coords.fromWGS84(myGuessInfo.correctLongitude, myGuessInfo.correctLatitude);
		const guessCoords = SMap.Coords.fromWGS84(myGuessInfo.guessLongitude, myGuessInfo.guessLatitude);
		mapDisplayGuessResult(correctCoords, guessCoords);
	}
}

function getCookie(name) {
	const value = `; ${document.cookie}`;
	const parts = value.split(`; ${name}=`);
	if (parts.length === 2)
		return parts.pop().split(';').shift();
}

function loadMap() {
	const center = SMap.Coords.fromWGS84(15.91790, 49.52655);
	map = new SMap(JAK.gel("map"), center, 6);

	map.addDefaultLayer(SMap.DEF_BASE).enable();
	map.addDefaultControls();

	const sync = new SMap.Control.Sync();
	map.addControl(sync);

	targetLayer = new SMap.Layer.HUD();
	map.addLayer(targetLayer);
	targetLayer.enable();

	const middle = getMarkerObject("var(--color-guessing-marker)");
	middle.style.marginTop = "-30px";
	middle.style.marginLeft = "-13px";
	targetLayer.addItem(middle, { left: "50%", top: "50%" });
}

async function locationGuessedAsync() {
	if (myName == null) {
		changeNameDialog(guessLocationAsync);
	} else {
		guessLocationAsync();
	}
}

async function guessLocationAsync() {
	const guessCoords = map.getCenter();
	const [long, lat] = guessCoords.toWGS84();

	const request = {
		GuesserId: myId,
		Latitude: lat,
		Longitude: long,
		Name: myName
	};

	try {
		const response = await fetch(`/api/places/${placeId}/guesses`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(request),
		});

		const data = await response.json();
		const correctCoords = SMap.Coords.fromWGS84(data.correctLongitude, data.correctLatitude);

		const guesses = await fetchGuessesAsync();
		renderGuesses(guesses);

		mapDisplayGuessResult(correctCoords, guessCoords);
	} catch (error) {
		console.error("Error:", error);
	}
}

function getTooltipMarker(coords, text) {
	const box = JAK.mel("div", {});
	box.className = "tooltip-box";

	const tooltip = JAK.mel("div", {});
	tooltip.className = "tooltip";
	tooltip.innerHTML = text;

	const tooltipArrow = JAK.mel("div", {});
	tooltipArrow.className = "tooltip-arrow";

	box.appendChild(tooltip);
	box.appendChild(tooltipArrow);

	return new SMap.Marker(coords, null, { url: box });
}

function getMarkerObject(color) {
	const svgCode = `<svg fill="${color}" viewBox="0 0 1920 1920" width="30" height="30" xmlns="http://www.w3.org/2000/svg" stroke="#00ff40"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M956.952 0c-362.4 0-657 294.6-657 656.88 0 180.6 80.28 347.88 245.4 511.56 239.76 237.96 351.6 457.68 351.6 691.56v60h120v-60c0-232.8 110.28-446.16 357.6-691.44 165.12-163.8 245.4-331.08 245.4-511.68 0-362.28-294.6-656.88-663-656.88" fill-rule="evenodd"></path> </g></svg>`;
	const marker = JAK.mel("div");
	marker.innerHTML = svgCode;
	return marker;
}

function getMarker(coords, color = "var(--color-marker)")
{
	return new SMap.Marker(coords, null, {
		url: getMarkerObject(color),
		anchor: { left: 15, bottom: 5 }
	});
}

function mapDisplayGuessResult(correctCoords, guessCoords) {
	map.syncPort();

	markersLayer = new SMap.Layer.Marker();
	map.addLayer(markersLayer);
	markersLayer.enable();

	const correctMarker = getMarker(correctCoords, "var(--color-place-marker)");
	markersLayer.addMarker(correctMarker);

	const guessMarker = getMarker(guessCoords);
	markersLayer.addMarker(guessMarker);

	const geometryLayer = new SMap.Layer.Geometry();
	map.addLayer(geometryLayer);
	geometryLayer.enable();
	
	const [centerCoords, zoom] = map.computeCenterZoom([guessCoords, correctCoords], false);
	const distance = guessCoords.distance(correctCoords);

	const points = calculateCurvePoints(guessCoords, centerCoords, correctCoords, distance);
	const line = new SMap.Geometry(SMap.GEOMETRY_POLYLINE, null, points, {
		color: "black",
		width: 2,
		style: 1
	});

	geometryLayer.addGeometry(line);

	markersLayer.addMarker(getTooltipMarker(centerCoords, formatDistance(distance)));
	map.setCenterZoom(centerCoords, zoom, true);

	targetLayer.disable();
	map.removeLayer(map.controlLayer);
	map.getContainer().style.pointerEvents = "none";
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

function formatDistance(distance) {
	if (distance > 1000) {
		distance /= 1000;
		return distance.toFixed(2) + " km";
	} else {
		return distance.toFixed(2) + " m";
	}
}

function toggleClass(elem, className) {
	elem.classList.toggle(className);
}

async function fetchGuessesAsync() {
	try
	{
		const response = await fetch(`/api/places/${placeId}/guesses`, {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
			}
		});

		return await response.json()
	} catch(error) {
		console.error("Error:", error);
		return null;
	}
}

function renderGuesses(guesses)
{
	if (guesses == null)
		return;

	const main = document.getElementById("main");
	main.classList = "guesses";

	const container = document.getElementById("guesses");

	let table = "<div class='table'>";

	table += `<div class="head">Name</div>`;
	table += `<div class="head">Score</div>`;
	table += `<div class="head">Distance</div>`;

	guesses.forEach(guess => {
		className = "";
		if (guess.guesserId == myId)
			className = "guess";

		table += `<div class="${className}">${guess.name}</div>`;
		table += `<div class="${className}">${guess.score}</div>`;
		table += `<div class="${className}">${formatDistance(guess.distance ?? 0)}</div>`;
	});
	table += "</div>";

	container.innerHTML = table;
}

function displayName() {
	if (myName == null)
		return;

	const container = document.getElementById("guesser-name")
	container.innerHTML = myName;
}

function changeNameDialog(callback = null) {
	const container = document.getElementById("input-name-dialog");
	toggleClass(container, "visible");

	const input = document.getElementById("input-name");
	input.value = myName ?? "";

	const form = document.getElementById("input-name-form");
	const btn = document.getElementById("btn-change-name");

	const eventHandler = () => {
		if (input.value == "")
			return;

		myName = input.value;
		localStorage.setItem("guesserName", myName);

		displayName();
		toggleClass(container, "visible");

		if (callback != null)
			callback();
	};

	btn.onclick = eventHandler;
	form.onsubmit = (event) => {
		event.preventDefault();
		eventHandler();
	};
}

function handled(event) {
	event.stopPropagation();
}
