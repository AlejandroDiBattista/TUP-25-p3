const URL = "https://api.openweathermap.org/data/2.5/weather?q=salta&appid=30d38b26954359266708f92e1317dac0&units=metric&lang=es";
const res   = await fetch(URL);
const data  = await res.json();

const temp  = data.main.temp;
const clima = data.weather[0].description;
console.log(`Clima en: Salta\nTemperatura: ${temp} °C\nCondición: ${clima}`);