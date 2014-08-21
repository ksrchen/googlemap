var map;
var kmlLayer;
var drawingManager;
var selectedShape;

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
            $("#legend").text("Too many matches...please zoom in to narrow your search")
        }
    });

    drawingManager = new google.maps.drawing.DrawingManager({
        drawingControl: true,
        drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_RIGHT,
            drawingModes: [
              google.maps.drawing.OverlayType.POLYGON,
            ]
        }
    });

    drawingManager.setMap(map);

    google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
        if (e.type != google.maps.drawing.OverlayType.MARKER) {
            // Switch back to non-drawing mode after drawing a shape.
            drawingManager.setDrawingMode(null);

            var newShape = e.overlay;
            newShape.type = e.type;
            setSelection(newShape);
            refresh();
        }
    });
}

function boundsChanged(e) {
    refresh();
}

function refresh() {
    var polygon;
    var bound = map.getBounds();

    var northEast = bound.getNorthEast();
    var southWest = bound.getSouthWest();

    if (selectedShape) {
        polygon = 'POLYGON((';
        var path = selectedShape.getPath();
        path.forEach(function (el, index) {
            polygon = polygon + el.lng() + ' ' + el.lat() + ','
        })

        var firstPoint = path.getAt(0);
        polygon = polygon + firstPoint.lng() + ' ' + firstPoint.lat();

        polygon = polygon + '))';

    } else {
        polygon = 'POLYGON((' +
            northEast.lng() + ' ' + northEast.lat() + ',' +
            southWest.lng() + ' ' + northEast.lat() + ',' +
            southWest.lng() + ' ' + southWest.lat() + ',' +
            northEast.lng() + ' ' + southWest.lat() + ',' +
            northEast.lng() + ' ' + northEast.lat() +
            '))';
    }
    var url = 'http://kmlservice.azurewebsites.net/api/property?polygon=' + encodeURIComponent(polygon) + '&propertyType=' + $("#propertyType").val();
    kmlLayer.setUrl(url);
    $("#legend").text("Refreshing....");
    $("#legend").fadeIn();
}

function setSelection(shape) {
    if (selectedShape) {
        selectedShape.setMap(null);
    }
    selectedShape = shape;
}

google.maps.event.addDomListener(window, 'load', initialize);

$(function () {
    $("#propertyType").change(function () {
        refresh();
    });

    $("#polygonTool").click(function () {
        drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    });

    $("#handTool").click(function () {
        drawingManager.setDrawingMode(null);
    });
    $("#clear").click(function () {
        setSelection(null);
        refresh();
    });
});
