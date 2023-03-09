# weather-app 🌤️

### Kafka Setup 

Run locally via docker

```bash
doc
docker-compose up
```

Create topic

```bash
docker exec --interactive --tty broker-1 \
kafka-topics --bootstrap-server broker-1:9092 \
                       --create --topic weather \
                       --partitions 3 \
                       --replication-factor 3
```

Can be later deleted via

```bash
docker exec --interactive --tty broker-1 \
kafka-topics --bootstrap-server broker-1:9092 \
                       --delete --topic weather
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

Read current messages via

```bash
docker exec --interactive --tty broker-1 \
kafka-console-consumer --bootstrap-server broker-1:9092 \
                       --topic weather
```

Read all messages via

```bash
docker exec --interactive --tty broker-1 \
kafka-console-consumer --bootstrap-server broker-1:9092 \
                       --topic weather \
                       --from-beginning
```

Read specific partition via

```bash
docker exec --interactive --tty broker-1 \
kafka-console-consumer --bootstrap-server broker-1:9092 \
                       --topic weather \
                       --partition 1
```

### Questions

```bash
docker exec --interactive --tty broker-1 \
kafka-topics --bootstrap-server broker-1:9092 \
                       --describe --topic weather
```



```
docker compose up
```

```bash
docker exec --interactive --tty broker-1 \
kafka-configs --bootstrap-server broker-1:9092 \
                       --alter --entity-type topics --entity-name weather --add-config min.insync.replicas=2
```

```
docker compose stop broker-2
```

Should still work

```
docker compose stop broker-3
```

Should no longer work

```
broker1     | org.apache.kafka.common.errors.NotEnoughReplicasException: The size of the current ISR Set(1) is insufficient to satisfy the min.isr requirement of 2 for partition weather-0
```

Back to work

```
docker compose start broker-2
```



Reset

```bash
docker exec --interactive --tty broker-1 \
kafka-configs --bootstrap-server broker-1:9092 \
                       --alter --entity-type topics --entity-name weather --delete-config min.insync.replicas
```

Now one node is sufficient again



# TODO

Analyze how the following things are related

* Number of Brokers
* Number of Partitions
* Number of Replicas
* in.sync.replica Configuration