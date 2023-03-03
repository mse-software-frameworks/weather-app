# weather-app 🌤️

### Kafka Setup 

Run locally via docker

```bash
docker-compose up
```

Create topic

```bash
docker exec --interactive --tty broker \
kafka-console-producer --bootstrap-server broker:9092 \
                       --topic weather
```

### Weather Producer

Configure Kafka via config found under `WeatherApp/WeatherProducer/config/kafka.json`

```json
{
  "topic": "weather", // topic name
  "servers": "localhost:9092;" // Initial list of brokers as a CSV list of broker host or host:port
}
```

Run producer app

```bash
cd WeatherApp/WeatherProducer
dotnet run
```

Read messages via

```bash
docker exec --interactive --tty broker \
kafka-console-consumer --bootstrap-server broker:9092 \
                       --topic weather \
                       --from-beginning
```

### Questions

# TODO

Analyze how the following things are related

* Number of Brokers
* Number of Partitions
* Number of Replicas
* in.sync.replica Configuration