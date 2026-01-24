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
docker compose down
docker compose up --build

### quick rebuild
docker compose up --build --force-recreate