# Docker Setup für DictionaryApi

## Voraussetzungen
- Docker Desktop installiert
- Docker Compose installiert

## Lokale Entwicklung starten

### Mit Docker Compose (empfohlen)


## build rebuild

rebuild container

### clean rebuild
cd D:\Development\Words\DictionaryApi

docker network create dictionary-network
docker compose down
docker compose -f docker-compose.libretranslate.yml up -d
docker compose -f docker-compose.yml up -d --build

### quick rebuild
docker compose up --build --force-recreate