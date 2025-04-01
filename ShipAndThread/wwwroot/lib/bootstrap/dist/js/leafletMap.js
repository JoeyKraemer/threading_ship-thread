window.initMap = (mapId, latitude, longitude, zoom) => {
    console.log("Initializing Leaflet Map:", mapId, latitude, longitude, zoom);
    
    var map = L.map(mapId).setView([latitude, longitude], zoom);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    L.marker([latitude, longitude]).addTo(map)
        .bindPopup('Selected Location')
        .openPopup();

    setTimeout(() => {
        map.invalidateSize();
    }, 500);

    window.addEventListener("resize", () => {
        setTimeout(() => map.invalidateSize(), 200);
    });
};
