window.maps = {};

window.initMap = (mapId, latitude, longitude, zoom) => {
    console.log("Initializing Leaflet Map:", mapId, latitude, longitude, zoom);
    
    var map = L.map(mapId).setView([latitude, longitude], zoom);
    window.maps[mapId] = { map: map, markers: [] };

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    setTimeout(() => {
        map.invalidateSize();
    }, 500);

    window.addEventListener("resize", () => {
        setTimeout(() => map.invalidateSize(), 200);
    });

    window.addTruckMarkers = (mapId, truckList) => {
        const mapObj = window.maps[mapId];
        if (!mapObj) return;

        mapObj.markers.forEach(marker => mapObj.map.removeLayer(marker));
        mapObj.markers = [];

        truckList.forEach(truck => {
                const marker = L.circleMarker([truck.latitude, truck.longitude], {
                radius: 9,
                fillColor: truck.color || "gray",
                color: "#000",
                weight: 1,
                opacity: 1,
                fillOpacity: 0.8
            }).addTo(mapObj.map)
                .bindPopup(`Truck ${truck.truckId}`);

            mapObj.markers.push(marker);
        });
    }
}