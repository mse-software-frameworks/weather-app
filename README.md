# weather-app 🌤️

### Kafka Setup 

Run locally via docker

```bash
cd WeatherApp
docker-compose down
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
  "topic": "weather", // Topic name
  "servers": "localhost:9092,", // Initial list of brokers as a CSV list of broker host or host:port
  "partitions": 1 // Number of partitions to write 
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

Analyze how the following things are related

* Number of Brokers
* Number of Partitions
* Number of Replicas
* in.sync.replica Configuration

Our setup consists of 3 brokers. The topic `weather` consist of 3 partitions, so initially the first broker where the topic was created has 3 `weather` partitions which are identified by 0, 1 and 2. However, as the replication factor is set to 3, the two remaining brokers will also eventually share this configuration.

The default [acknowledge mode for Kafka is `acks=all`](https://www.conduktor.io/kafka/kafka-topic-configuration-min-insync-replicas/), this means the producers considers messages as "written successfully" when the message is accepted by all in-sync replicas (ISR).  With `acks=0` a message would be "written successfully" the moment the message was sent without waiting for the broker to accept it at all. 

On default, a topic has `min.insync.replicas=1` so in our setup we can tolerate two brokers being down.

One can retrieve the current topic configuration via

```bash
docker exec --interactive --tty broker-1 \
kafka-topics --bootstrap-server broker-1:9092 \
                       --describe --topic weather
```

Similarly, one can increase the `min.insync.replicas`, in this case to 2

```bash
docker exec --interactive --tty broker-1 \
kafka-configs --bootstrap-server broker-1:9092 \
                       --alter --entity-type topics --entity-name weather --add-config min.insync.replicas=2
```

Now a message is only "written successfully" when two brokers have accepted it. Stopping one broker should not affect functionality

```bash
docker compose stop broker-2
```

However, stopping a second broker leads to insufficient all in-sync replicas

```bash
docker compose stop broker-3
```

```bash
broker1     | org.apache.kafka.common.errors.NotEnoughReplicasException: The size of the current ISR Set(1) is insufficient to satisfy the min.isr requirement of 2 for partition weather-0
```

Restarting a down broker should restore functionality

```
docker compose start broker-2
```

At the end, one can also remove the `min.insync.replicas` constraint again

```bash
docker exec --interactive --tty broker-1 \
kafka-configs --bootstrap-server broker-1:9092 \
                       --alter --entity-type topics --entity-name weather --delete-config min.insync.replicas
```

Now one functional broker is sufficient again for writing messages.

