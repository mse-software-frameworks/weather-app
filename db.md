# Database

We decided to use MongoDB to persist our aggregated datasets. The database run in it's own Docker container and is deployed by utilizing our `docker-compose.yml`.

## Why did we choose MongoDB?

We choose MongoDB for its flexibility and ease to setup. Especially for small scale applications and quick prototypes, NoSQL databases shine by how quickly they are to get up and running and how little utility code has to be written until the first data can be persisted to the database.

NoSQL databases are also cheaper to operate in the cloud than their SQL counterparts (at least for small scale apps like ours) and still provide the benefit of being highly horizontally scalable if demand increases in the future.