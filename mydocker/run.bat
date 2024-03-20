docker build -t dockerizedmvcnbg .. -f ../dockerizedmvcnbg/Dockerfile  
docker tag dockerizedmvcnbg iracleous/dockerizedmvcnbg:0.0.1
docker push iracleous/dockerizedmvcnbg:0.0.1
docker pull iracleous/dockerizedmvcnbg:0.0.1

docker compose  -f .\docker-compose.yml -p myirac up -d