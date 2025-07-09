# healthri-basket-api

## Testing the API

To test the API:

1. Run the application:
   ```bash
   docker compose up -d
   ```

2. Access the Swagger UI at: `http://localhost:8080/swagger`

3. User UUIDs follow the structure: `00000000-0000-0000-0000-000000000001`

## Creating a New Basket

To create a new basket, use the POST endpoint:

**Endpoint:** `POST /api/v1/baskets/{userUuid}`

The payload is just the name you want to give the basket.

**Example payload:**
```json
"My Sample Basket"
```

**Complete example:**
```bash
curl -X POST "http://localhost:8080/api/v1/baskets/00000000-0000-0000-0000-000000000001" \
  -H "Content-Type: application/json" \
  -d '"My Sample Basket"'
```
