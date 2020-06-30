# RabbitMQ docker-compose

## Installation
```
docker-compose up
```

## Test
```
open http://$(machine_ip):15672/
```
and use the login

```
username: rabbitmq
password: rabbitmq
```

## Mutual authentication

Certificates organization.

                   [rootca]
                       |
                       |
                   _______________
                  /\              \
                 /  \              \
                /    \              \
               /      \              \
              /        \              \
        [rabbitmq]    [producer]    [consumer]