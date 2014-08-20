var map;
var kmlLayer;
function initialize() {
    var mapOptions = {
        zoom: 10,
        center: new google.maps.LatLng(34.052235, -118.243683)
    };
    map = new google.maps.Map(document.getElementById('map-canvas'),
        mapOptions);

    navigator.geolocation.getCurrentPosition(function (position) {
        var coords = new google.maps.LatLng(
          position.coords.latitude,
          position.coords.longitude
      );
        map.setCenter(coords);
    });

    kmlLayer = new google.maps.KmlLayer({
        url: 'http://kmldata.azurewebsites.net/property.xml',
        preserveViewport: true,
    });
    kmlLayer.setMap(map);

    google.maps.event.addListener(kmlLayer, 'click', function (kmlEvent) {
        var text = kmlEvent.featureData.description;
        //alert(text);
    });

    google.maps.event.addListener(map, 'bounds_changed', boundsChanged);
    google.maps.event.addListener(kmlLayer, 'status_changed', function (ev) {
        if (kmlLayer.status === google.maps.KmlLayerStatus.OK) {
            $("#legend").fadeOut();
        } else {
            $("#legend").text("Too many matches...please zoom in in narrow the search")
        }
    });


}

function boundsChanged(e) {
    refresh();
}

function refresh() {

    var bound = map.getBounds();

    var northEast = bound.getNorthEast();
    var southWest = bound.getSouthWest();

    var polygon = 'POLYGON((' +
        northEast.lng() + ' ' + northEast.lat() + ',' +
        southWest.lng() + ' ' + northEast.lat() + ',' +
        southWest.lng() + ' ' + southWest.lat() + ',' +
        northEast.lng() + ' ' + southWest.lat() + ',' +
        northEast.lng() + ' ' + northEast.lat() +
        '))';

    var url = 'http://kmlservice.azurewebsites.net/api/property?polygon=' + encodeURIComponent(polygon) + '&propertyType=' + $("#propertyType").val();
    kmlLayer.setUrl(url);
    $("#legend").text("Refreshing....");
    $("#legend").fadeIn();
}

google.maps.event.addDomListener(window, 'load', initialize);

$(function () {
    $("#propertyType").change(function () {
        refresh();
    });
});
