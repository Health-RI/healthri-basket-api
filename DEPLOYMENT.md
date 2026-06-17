# Deployment Guide

This repository has two deployment paths:

- Local development via `docker compose`
- Azure deployment via GitHub Actions and Azure Container Apps

## Local Development

Run the app locally with:

```bash
docker compose up -d
```

Then open Swagger at:

```text
http://localhost:8080/swagger
```

## Azure Deployment

The main deployment flow is defined in [`.github/workflows/deploy.yml`](.github/workflows/deploy.yml).

That workflow:

1. Builds the image from `healthri-basket-api/Dockerfile`
2. Pushes the image to GHCR as `ghcr.io/<repository>:<git-sha>`
3. Logs into Azure with `AZURE_CREDENTIALS`
4. Runs `scripts/azure/deploy-basket-api.sh`

### Environments

The deploy script maps the selected environment to these defaults:

| Environment | Resource Group | Container Apps Environment | App Name | APIM |
| --- | --- | --- | --- | --- |
| `test` | `healthri-test` | `health-ri-aca-env-test` | `basket-api-test` | Disabled |
| `acc` | `healthri-uac` | `health-ri-aca-env-acc` | `basket-api-acc` | `healthri-api-acc` |
| `prod` | `healthri` | `health-ri-aca-env` | `basket-api` | `healthri-api` |

`prod` is also used when the script is called with `prd`.

### Automated Deployment

The GitHub Actions workflow runs against a GitHub Environment named after the chosen deployment target:

- `test`
- `acc`
- `prod`

For automated deployment, you configure these values once in the GitHub Actions environment secrets and variables. You do not pass them on the command line when you trigger the workflow.

Configure the following secrets and variables in the matching GitHub Actions environment:

Secrets:

- `AZURE_CREDENTIALS`
- `DB_CONNECTION_STRING`
- `OPENID_AUTHORITY`
- `OPENID_ISSUER`
- `OPENID_AUDIENCE`
- `OTEL_EXPORTER_OTLP_HEADERS`

Variables:

- `OTEL_SERVICE_NAME`
- `OTEL_EXPORTER_OTLP_PROTOCOL`
- `OTEL_EXPORTER_OTLP_ENDPOINT` if you use one

Optional registry credentials:

- `GHCR_USERNAME`
- `GHCR_PASSWORD`

Use those only if the GHCR image is private and the Container App needs to pull it directly.

If these values are already configured in the GitHub Actions environment, the workflow injects them automatically during deployment.

### Prerequisites in Azure

Before deployment, the target Azure Container Apps environment must already exist.

The script checks for the environment with:

```bash
az containerapp env show --name <environment> --resource-group <resource-group>
```

If that environment does not exist, deployment stops.

### Manual Deployment

You can also run the script directly after logging into Azure and setting the required variables:

```bash
ENVIRONMENT=prod \
IMAGE=ghcr.io/<owner>/<repo>:<tag> \
DB_CONNECTION_STRING='...' \
OPENID_AUTHORITY='...' \
OPENID_ISSUER='...' \
OPENID_AUDIENCE='...' \
scripts/azure/deploy-basket-api.sh
```

For private GHCR images, add:

```bash
GHCR_USERNAME='...' \
GHCR_PASSWORD='...' \
```

### What The Script Does

The script in [`scripts/azure/deploy-basket-api.sh`](scripts/azure/deploy-basket-api.sh) will:

1. Create the Container App if it does not exist
2. Update the Container App if it already exists
3. Store the database connection string as a secret
4. Configure the OpenID and telemetry environment variables
5. Enable external ingress on port `8080`
6. Configure API Management for `acc` and `prod`

## Notes

- The deployment workflow uses `healthri-basket-api/Dockerfile`.
- The top-level `Dockerfile` is not part of the current deployment workflow.
