let center = window.mapCenter || [48.8866527839977, 2.34310679732974];


function init() {
	let map = new ymaps.Map('map', {
		center: center,
		zoom: 17
	});
console.log('Map initialized with center:', center);
	map.controls.remove('geolocationControl'); // удаляем геолокацию
  map.controls.remove('searchControl'); // удаляем поиск
  map.controls.remove('trafficControl'); // удаляем контроль трафика
  map.controls.remove('typeSelector'); // удаляем тип
  map.controls.remove('fullscreenControl'); // удаляем кнопку перехода в полноэкранный режим
  map.controls.remove('zoomControl'); // удаляем контрол зуммирования
  map.controls.remove('rulerControl'); // удаляем контрол правил
    map.behaviors.disable([
        'scrollZoom',
        'drag',
        'dblClickZoom',
        'multiTouch',
        'rightMouseButtonMagnifier',
        'leftMouseButtonMagnifier',
        'keyboard'
    ]);

    // Добавляем метку в центр карты
    // Используем значения из Razor
    let placemark = new ymaps.Placemark(center, {
        hintContent: window.mapHint || 'Объект недвижимости',
        balloonContent: window.mapBalloon || 'Здесь находится объект'
    });

    map.geoObjects.add(placemark);
}

ymaps.ready(init);