# OData Testing Examples

## Prerequisites

1. Start the API server: `dotnet run` from the API project directory
2. The API will be available at: `https://localhost:7296` or `http://localhost:5073`

## Test OData Endpoints

### 1. Get OData Metadata

```bash
curl -X GET "https://localhost:7296/odata/$metadata"
```

### 2. Get All Categories

```bash
curl -X GET "https://localhost:7296/odata/Categories"
```

### 3. Get Categories with Filtering

```bash
# Filter by category name
curl -X GET "https://localhost:7296/odata/Categories?$filter=contains(CategoryName, 'tech')"

# Filter by exact name
curl -X GET "https://localhost:7296/odata/Categories?$filter=CategoryName eq 'Electronics'"
```

### 4. Get Categories with Sorting

```bash
# Sort by name ascending
curl -X GET "https://localhost:7296/odata/Categories?$orderby=CategoryName"

# Sort by creation date descending
curl -X GET "https://localhost:7296/odata/Categories?$orderby=CreatedDateTime desc"
```

### 5. Get Categories with Pagination

```bash
# Get first 5 categories
curl -X GET "https://localhost:7296/odata/Categories?$top=5"

# Skip first 5 and get next 5
curl -X GET "https://localhost:7296/odata/Categories?$skip=5&$top=5"
```

### 6. Get Categories with Field Selection

```bash
# Get only category names and descriptions
curl -X GET "https://localhost:7296/odata/Categories?$select=CategoryName,Description"
```

### 7. Get Categories with Count

```bash
# Get total count
curl -X GET "https://localhost:7296/odata/Categories/$count"

# Get results with count
curl -X GET "https://localhost:7296/odata/Categories?$count=true"
```

### 8. Get Categories with Related Products

```bash
# Expand products relationship
curl -X GET "https://localhost:7296/odata/Categories?$expand=Products"

# Expand products with filtering
curl -X GET "https://localhost:7296/odata/Categories?$expand=Products($filter=UnitPrice gt 20)"
```

### 9. Complex Queries

```bash
# Combined filtering, sorting, and pagination
curl -X GET "https://localhost:7296/odata/Categories?$filter=contains(CategoryName, 'e')&$orderby=CategoryName&$top=10"

# Get categories with products count
curl -X GET "https://localhost:7296/odata/Categories?$expand=Products($count=true)"
```

### 10. Product Queries

```bash
# Get all products
curl -X GET "https://localhost:7296/odata/Products"

# Filter products by price
curl -X GET "https://localhost:7296/odata/Products?$filter=UnitPrice gt 15 and UnitPrice lt 50"

# Get products with category information
curl -X GET "https://localhost:7296/odata/Products?$expand=Category"

# Get discontinued products
curl -X GET "https://localhost:7296/odata/Products?$filter=Discontinued eq true"
```

### 11. CRUD Operations

#### Create Category

```bash
curl -X POST "https://localhost:7296/odata/Categories" \
  -H "Content-Type: application/json" \
  -d '{
    "CategoryName": "Test Category",
    "Description": "Test category description"
  }'
```

#### Update Category

```bash
curl -X PUT "https://localhost:7296/odata/Categories('{category-id}')" \
  -H "Content-Type: application/json" \
  -d '{
    "CategoryName": "Updated Category",
    "Description": "Updated description"
  }'
```

#### Delete Category

```bash
curl -X DELETE "https://localhost:7296/odata/Categories('{category-id}')"
```

## Browser Testing

You can also test these endpoints directly in your browser:

1. **Metadata**: `https://localhost:7296/odata/$metadata`
2. **All Categories**: `https://localhost:7296/odata/Categories`
3. **Filtered Categories**: `https://localhost:7296/odata/Categories?$filter=contains(CategoryName, 'e')`
4. **Sorted Categories**: `https://localhost:7296/odata/Categories?$orderby=CategoryName`
5. **Paginated Categories**: `https://localhost:7296/odata/Categories?$top=5`

## PowerShell Testing

### Test OData Endpoints with PowerShell

```powershell
# Get all categories
$response = Invoke-RestMethod -Uri "https://localhost:7296/odata/Categories" -Method GET
$response.value

# Get filtered categories
$response = Invoke-RestMethod -Uri "https://localhost:7296/odata/Categories?`$filter=contains(CategoryName, 'e')" -Method GET
$response.value

# Create a new category
$body = @{
    CategoryName = "PowerShell Category"
    Description = "Created via PowerShell"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:7296/odata/Categories" -Method POST -Body $body -ContentType "application/json"
$response
```

## Expected Response Format

### Categories Response

```json
{
    "@odata.context": "https://localhost:7296/odata/$metadata#Categories",
    "value": [
        {
            "Id": "category-id",
            "CategoryName": "Electronics",
            "Description": "Electronic devices and gadgets",
            "Picture": null,
            "PictureLink": null,
            "CreatedDateTime": "2024-01-01T00:00:00Z",
            "UpdatedDateTime": null
        }
    ]
}
```

### Products Response

```json
{
    "@odata.context": "https://localhost:7296/odata/$metadata#Products",
    "value": [
        {
            "Id": "product-id",
            "ProductName": "Laptop",
            "SupplierId": "supplier-id",
            "CategoryId": "category-id",
            "QuantityPerUnit": "1 unit",
            "UnitPrice": 999.99,
            "UnitsInStock": 10,
            "UnitsOnOrder": 0,
            "ReorderLevel": 5,
            "Discontinued": false,
            "CreatedDateTime": "2024-01-01T00:00:00Z",
            "UpdatedDateTime": null
        }
    ]
}
```

## Error Handling

OData will return appropriate HTTP status codes and error messages:

### 400 Bad Request (Invalid Query)

```json
{
    "error": {
        "code": "400",
        "message": "The query specified in the URI is not valid."
    }
}
```

### 404 Not Found

```json
{
    "error": {
        "code": "404",
        "message": "Resource not found."
    }
}
```

## Performance Tips

1. **Use $select** to limit returned fields
2. **Use $top** to limit result size
3. **Use $filter** to reduce data transfer
4. **Combine operations** for efficient queries
5. **Monitor query performance** for complex operations
