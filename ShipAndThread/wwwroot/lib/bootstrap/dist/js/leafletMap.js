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

    window.addRouteStopMarkers = (mapId, routeStops) => {
        const mapObj = window.maps[mapId];
        if (!mapObj) return;

        mapObj.markers.forEach(marker => mapObj.map.removeLayer(marker));
        mapObj.markers = [];

        let latLngs = [];

        routeStops.forEach(stop => {
            if (stop.latitude && stop.longitude) {
                // Create a marker for each stop
                const marker = L.marker([stop.latitude, stop.longitude])
                    .addTo(mapObj.map)
                    .bindPopup(`Stop ${stop.stopNumber} - ${stop.timestamp}`);

                mapObj.markers.push(marker);

                // Add this stop's coordinates to the latLngs array
                latLngs.push([stop.latitude, stop.longitude]);
            }
        });

        // Add polyline if there are multiple stops
        if (latLngs.length > 1) {
            L.polyline(latLngs, { color: 'blue', weight: 4 }).addTo(mapObj.map);
        }
    }
    
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

    window.centerMap = (mapId, lat, lng) => {
        const mapObj = window.maps[mapId];
        if (mapObj && mapObj.map) {
            mapObj.map.setView([lat, lng], mapObj.map.getZoom());
        }
    }
}