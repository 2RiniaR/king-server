.PHONY: migrate
migrate:
	dotnet ef migrations add $(name)
	dotnet ef database update

.PHONY: run
run:
	dotnet run

.PHONY: push
push:
	docker build . -t $(app_name)
	docker push $(app_name)
