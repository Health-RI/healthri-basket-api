#!/usr/bin/env bash

set -euo pipefail

ENVIRONMENT=${ENVIRONMENT:-test} # test|acc|prod

case "${ENVIRONMENT}" in
  test)
    DEFAULT_RESOURCE_GROUP="healthri-test"
    DEFAULT_CONTAINERAPP_ENV="health-ri-aca-env-test"
    DEFAULT_APP_NAME="basket-api-test"
    ;;
  acc)
    DEFAULT_RESOURCE_GROUP="healthri-uac"
    DEFAULT_CONTAINERAPP_ENV="health-ri-aca-env-acc"
    DEFAULT_APP_NAME="basket-api-acc"
    ;;
  prod|prd)
    ENVIRONMENT="prod"
    DEFAULT_RESOURCE_GROUP="healthri"
    DEFAULT_CONTAINERAPP_ENV="health-ri-aca-env"
    DEFAULT_APP_NAME="basket-api"
    ;;
  *)
    echo "Unknown ENVIRONMENT '${ENVIRONMENT}'. Use test, acc, prod." >&2
    exit 1
    ;;
 esac

RESOURCE_GROUP=${RESOURCE_GROUP:-${DEFAULT_RESOURCE_GROUP}}
CONTAINERAPP_ENV=${CONTAINERAPP_ENV:-${DEFAULT_CONTAINERAPP_ENV}}
APP_NAME=${APP_NAME:-${DEFAULT_APP_NAME}}
IMAGE=${IMAGE:-}
LOCATION=${LOCATION:-westeurope}
TARGET_PORT=${TARGET_PORT:-8080}
CPU=${CPU:-1.0}
MEMORY=${MEMORY:-2.0Gi}
MIN_REPLICAS=${MIN_REPLICAS:-1}
MAX_REPLICAS=${MAX_REPLICAS:-1}
WORKLOAD_PROFILE_NAME=${WORKLOAD_PROFILE_NAME:-}

POSTGRES_RESOURCE_GROUP=${POSTGRES_RESOURCE_GROUP:-${RESOURCE_GROUP}}

DB_CONNECTION_STRING=${DB_CONNECTION_STRING:-}
OPENID_AUTHORITY=${OPENID_AUTHORITY:-}
OPENID_ISSUER=${OPENID_ISSUER:-}
OPENID_AUDIENCE=${OPENID_AUDIENCE:-}

GHCR_USERNAME=${GHCR_USERNAME:-}
GHCR_PASSWORD=${GHCR_PASSWORD:-}

REQUIRED_VARS=(
  IMAGE
  DB_CONNECTION_STRING
  OPENID_AUTHORITY
  OPENID_ISSUER
  OPENID_AUDIENCE
)

missing=0
for var in "${REQUIRED_VARS[@]}"; do
  if [ -z "${!var}" ]; then
    echo "Missing required env var: ${var}" >&2
    missing=1
  fi
done

if [ "${missing}" -ne 0 ]; then
  echo "Set the missing required env vars before running this script." >&2
  exit 1
fi

if ! az containerapp env show --name "${CONTAINERAPP_ENV}" --resource-group "${RESOURCE_GROUP}" >/dev/null 2>&1; then
  echo "Container Apps environment ${CONTAINERAPP_ENV} not found in ${RESOURCE_GROUP}." >&2
  echo "Create it first or point CONTAINERAPP_ENV/RESOURCE_GROUP to an existing environment." >&2
  exit 1
fi

SECRET_ARGS=("db-connection-string=${DB_CONNECTION_STRING}")

ENV_VARS=(
  "DB_CONNECTION_STRING=secretref:db-connection-string"
  "OPENID_AUTHORITY=${OPENID_AUTHORITY}"
  "OPENID_ISSUER=${OPENID_ISSUER}"
  "OPENID_AUDIENCE=${OPENID_AUDIENCE}"
)

CREATE_ARGS=(
  --name "${APP_NAME}"
  --resource-group "${RESOURCE_GROUP}"
  --environment "${CONTAINERAPP_ENV}"
  --image "${IMAGE}"
  --target-port "${TARGET_PORT}"
  --ingress external
  --cpu "${CPU}"
  --memory "${MEMORY}"
  --min-replicas "${MIN_REPLICAS}"
  --max-replicas "${MAX_REPLICAS}"
  --env-vars "${ENV_VARS[@]}"
  --secrets "${SECRET_ARGS[@]}"
)

UPDATE_ARGS=(
  --name "${APP_NAME}"
  --resource-group "${RESOURCE_GROUP}"
  --image "${IMAGE}"
  --cpu "${CPU}"
  --memory "${MEMORY}"
  --min-replicas "${MIN_REPLICAS}"
  --max-replicas "${MAX_REPLICAS}"
  --set-env-vars "${ENV_VARS[@]}"
)

if [ -n "${WORKLOAD_PROFILE_NAME}" ]; then
  CREATE_ARGS+=(--workload-profile-name "${WORKLOAD_PROFILE_NAME}")
  UPDATE_ARGS+=(--workload-profile-name "${WORKLOAD_PROFILE_NAME}")
fi

REGISTRY_ARGS=()
if [ -n "${GHCR_USERNAME}" ] && [ -n "${GHCR_PASSWORD}" ]; then
  REGISTRY_ARGS+=(--registry-server ghcr.io --registry-username "${GHCR_USERNAME}" --registry-password "${GHCR_PASSWORD}")
fi

if az containerapp show --name "${APP_NAME}" --resource-group "${RESOURCE_GROUP}" >/dev/null 2>&1; then
  echo "Container App ${APP_NAME} exists; updating configuration."

  if [ -n "${GHCR_USERNAME}" ] && [ -n "${GHCR_PASSWORD}" ]; then
    echo "Updating registry credentials..."
    az containerapp registry set \
      --name "${APP_NAME}" \
      --resource-group "${RESOURCE_GROUP}" \
      --server ghcr.io \
      --username "${GHCR_USERNAME}" \
      --password "${GHCR_PASSWORD}"
  fi

  echo "Updating secrets..."
  az containerapp secret set \
    --name "${APP_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --secrets "${SECRET_ARGS[@]}"

  echo "Ensuring ingress configuration..."
  az containerapp ingress enable \
    --name "${APP_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --type external \
    --target-port "${TARGET_PORT}"

  az containerapp update "${UPDATE_ARGS[@]}"
else
  echo "Creating Container App ${APP_NAME}..."
  if [ ${#REGISTRY_ARGS[@]} -gt 0 ]; then
    CREATE_ARGS+=("${REGISTRY_ARGS[@]}")
  fi
  az containerapp create "${CREATE_ARGS[@]}"
fi

echo "Deployment of Container App ${APP_NAME} completed."
