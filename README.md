# weather-app 🌤️

## Exercise Backend & Database

### Setup

Run locally via docker

```bash
cd WeatherApp
mkdir WeatherApp.Backend/data
docker compose down --remove-orphans
docker-compose up
```

> ⚠️ Make sure the broker count of the cluster is 3. If for some reasons one or more brokers are missing try to stop & restart docker. 

### Weather Producer & Aggregator

Configure the weather producer/aggregator app under `WeatherApp/WeatherProducer/config/`

Configure kafka via `WeatherApp/WeatherProducer/config/kafka.json`

```json
{
  "servers": "localhost:29092,localhost:39092,localhost:49092", // Initial list of brokers as a CSV list of broker host or host:port
  "schemaRegistry": "http://localhost:8085", // URL to schema registry
  "weatherTopic": "weather",
  "averageWeatherTable": "average-weather",
  "averageTemperatureTopic": "average-temperature",
  "averageWindspeedTopic": "average-windspeed",
  "averageWindchillTopic": "average-windchill",
  "replications": 3, // Number of replications to write 
  "streamApplicationId": "weather-aggregator" // Application id for the stream processing
}
```

Configure cities of which weather data is tracked via  `WeatherApp/WeatherProducer/config/cities.json`

```json
{
  "cities": [
    {
      "partition": 0, // Partiton to write (should be unique)
      "key": "Vienna", // Message key identifier
      "latitude": "48.21",
      "longitude": "16.37"
    },
    ...
  ]
}
```

Generate code from schema

> 🪧Generated code is tracked in the repository, manual generation is not necessary.

```bash
cd WeatherApp/WeatherApp.Kafka.Schemas
dotnet tool install --global Apache.Avro.Tools
```

```bash
avrogen -s .\Schemafiles\Weather.avsc  .  --namespace "weather.serialization.avro:WeatherApp.Kafka.Schemas.Avro" --skip-directories 
avrogen -s .\Schemafiles\AverageWeather.avsc  .  --namespace "weather.serialization.avro:WeatherApp.Kafka.Schemas.Avro" --skip-directories 
```

Run weather producer/aggregator app

```bash
cd WeatherApp/WeatherProducer
dotnet run
```

> ⚠️ The app tries to delete all topics & recreate them at startup. Creating topics can sometimes fail when deleted topics are marked but not yet deleted. If problems arise, try to modify delay in `WeatherApp/WeatherProducer/producer/TopicCreator.cs`.

After a successful start topics & messages should be listed in the kakfa ui located at http://localhost:8080.

![image-20230318225255760](.img/image-20230318225255760.png)

### Weather Backend & Database

Configure backend via `WeatherApp/WeatherApp.Backend/appsetings.json`

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://root:root@localhost:27017"
  },
  "Kafka": {
    "Servers": "localhost:29092,localhost:39092,localhost:49092",
    "GroupId": "default",
    "TopicName": "aggregated-weather",
    "SchemaRegistryUrl": "http://localhost:8085"
  }
}
```

Run weather backend

```bash
cd WeatherApp.Backend
dotnet run
```

After a successful start weather data should be persisted in mongodb. One can check this is easily via the mongodb shell:

```bash
mongosh "mongodb://root:root@localhost:27017"
```

With this query all records should be returned

```bash
use weatherAppDb
db.Weather.find()
```

```json
[
  {
    _id: ObjectId("642444150a019fc9b14f573a"),
    City: 'Vienna',
    Date: ISODate("2023-03-27T13:58:45.549Z"),
    AverageTemperature: '10.49',
    AverageWindspeed: '6.99',
    AverageWindchill: '9.79'
  },
  ...
]
```

### Questions

* [db.md](./db.md)
* [backend.md](./backend.md)



## Exercise Streaming

See: https://github.com/mse-software-frameworks/weather-app/tree/exercise-streaming

## Exercise Kafka Setup 

See https://github.com/mse-software-frameworks/weather-app/tree/exercise-setup-kafka
