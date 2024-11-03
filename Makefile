.PHONY: migrate
migrate:
	dotnet ef migrations add $(name)
	dotnet ef database update

.PHONY: run
run:
	dotnet run

.PHONY: docker-build
docker-build:
	docker build . -t $(app_name)

.PHONY: docker-push
docker-push:
	docker push $(app_name)
