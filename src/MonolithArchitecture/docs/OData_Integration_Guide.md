# OData Integration Guide

## Overview

This project has been enhanced with OData support to provide powerful, flexible querying capabilities for the API. OData (Open Data Protocol) allows clients to perform complex queries using URL parameters without requiring custom API endpoints.

## OData Endpoints

### Categories

-   **Base URL**: `/odata/Categories`
-   **Entity**: `Categories`
-   **Supported Operations**: GET, POST, PUT, DELETE

### Products

-   **Base URL**: `/odata/Products`
-   **Entity**: `Products`
-   **Supported Operations**: GET (read-only for now)

## Query Examples

### Basic Queries

1. **Get all categories**:

    ```
    GET /odata/Categories
    ```

2. **Get a specific category**:

    ```
    GET /odata/Categories('category-id')
    ```

3. **Get all products**:
    ```
    GET /odata/Products
    ```

### Filtering ($filter)

1. **Filter categories by name**:

    ```
    GET /odata/Categories?$filter=CategoryName eq 'Electronics'
    ```

2. **Filter categories containing text**:

    ```
    GET /odata/Categories?$filter=contains(CategoryName, 'tech')
    ```

3. **Filter products by price range**:

    ```
    GET /odata/Products?$filter=UnitPrice gt 10 and UnitPrice lt 50
    ```

4. **Filter products by category**:

    ```
    GET /odata/Products?$filter=CategoryId eq 'category-id'
    ```

5. **Filter discontinued products**:
    ```
    GET /odata/Products?$filter=Discontinued eq true
    ```

### Sorting ($orderby)

1. **Sort categories by name**:

    ```
    GET /odata/Categories?$orderby=CategoryName
    ```

2. **Sort products by price (descending)**:

    ```
    GET /odata/Products?$orderby=UnitPrice desc
    ```

3. **Multiple sort criteria**:
    ```
    GET /odata/Products?$orderby=CategoryId, UnitPrice desc
    ```

### Pagination ($top, $skip)

1. **Get first 10 categories**:

    ```
    GET /odata/Categories?$top=10
    ```

2. **Skip first 20 and get next 10**:

    ```
    GET /odata/Categories?$skip=20&$top=10
    ```

3. **Pagination with sorting**:
    ```
    GET /odata/Categories?$orderby=CategoryName&$skip=0&$top=20
    ```

### Selecting Fields ($select)

1. **Get only category names**:

    ```
    GET /odata/Categories?$select=CategoryName
    ```

2. **Get specific product fields**:
    ```
    GET /odata/Products?$select=ProductName,UnitPrice,CategoryId
    ```

### Counting ($count)

1. **Get total count of categories**:

    ```
    GET /odata/Categories/$count
    ```

2. **Get count with filter**:

    ```
    GET /odata/Products/$count?$filter=UnitPrice gt 20
    ```

3. **Include count in results**:
    ```
    GET /odata/Categories?$count=true
    ```

### Expanding Related Data ($expand)

1. **Get categories with their products**:

    ```
    GET /odata/Categories?$expand=Products
    ```

2. **Get products with category details**:

    ```
    GET /odata/Products?$expand=Category
    ```

3. **Expand with filtering**:
    ```
    GET /odata/Categories?$expand=Products($filter=UnitPrice gt 20)
    ```

### Complex Queries

1. **Categories with products count and filtering**:

    ```
    GET /odata/Categories?$expand=Products($count=true)&$filter=Products/$count gt 5
    ```

2. **Products with category details, filtered and sorted**:

    ```
    GET /odata/Products?$expand=Category&$filter=UnitPrice gt 15&$orderby=UnitPrice desc&$top=20
    ```

3. **Search across multiple fields**:
    ```
    GET /odata/Categories?$filter=contains(CategoryName, 'tech') or contains(Description, 'tech')
    ```

## OData Functions and Operators

### String Functions

-   `contains(field, 'value')` - Contains text
-   `startswith(field, 'value')` - Starts with text
-   `endswith(field, 'value')` - Ends with text
-   `length(field)` - String length
-   `tolower(field)` - Convert to lowercase
-   `toupper(field)` - Convert to uppercase

### Date Functions

-   `year(field)` - Extract year
-   `month(field)` - Extract month
-   `day(field)` - Extract day
-   `now()` - Current date/time

### Math Functions

-   `round(field)` - Round number
-   `floor(field)` - Floor number
-   `ceiling(field)` - Ceiling number

### Logical Operators

-   `and` - Logical AND
-   `or` - Logical OR
-   `not` - Logical NOT

### Comparison Operators

-   `eq` - Equal
-   `ne` - Not equal
-   `gt` - Greater than
-   `ge` - Greater than or equal
-   `lt` - Less than
-   `le` - Less than or equal

## CRUD Operations

### Create (POST)

```json
POST /odata/Categories
Content-Type: application/json

{
  "CategoryName": "New Category",
  "Description": "Category description"
}
```

### Update (PUT)

```json
PUT /odata/Categories('category-id')
Content-Type: application/json

{
  "CategoryName": "Updated Category",
  "Description": "Updated description"
}
```

### Delete (DELETE)

```
DELETE /odata/Categories('category-id')
```

## Configuration

OData is configured in `Program.cs` with the following settings:

-   Maximum results per page: 50 (default)
-   Maximum top value: 1000
-   Supported query options: $select, $filter, $orderby, $count
-   Route prefix: `/odata`

## Best Practices

1. **Use pagination** for large datasets to improve performance
2. **Select only needed fields** to reduce payload size
3. **Use filtering** to reduce data transfer
4. **Combine operations** for efficient queries
5. **Consider indexing** frequently filtered fields in the database

## Error Handling

OData will return appropriate HTTP status codes:

-   `200 OK` - Successful query
-   `400 Bad Request` - Invalid query syntax
-   `404 Not Found` - Resource not found
-   `500 Internal Server Error` - Server error

## Metadata

You can access the OData metadata document at:

```
GET /odata/$metadata
```

This provides information about available entities, properties, and relationships.

## Example Client Usage

### JavaScript/TypeScript

```javascript
// Get filtered and sorted categories
const response = await fetch(
    '/odata/Categories?$filter=contains(CategoryName, "tech")&$orderby=CategoryName'
);
const data = await response.json();

// Get categories with product count
const categoriesWithProducts = await fetch(
    "/odata/Categories?$expand=Products($count=true)"
);
const result = await categoriesWithProducts.json();
```

### C# Client

```csharp
// Using OData client libraries
var query = container.Categories
    .Where(c => c.CategoryName.Contains("tech"))
    .OrderBy(c => c.CategoryName);
```

## Performance Considerations

1. **Indexing**: Ensure database indexes exist for frequently filtered fields
2. **Pagination**: Always use $top and $skip for large datasets
3. **Selective Fields**: Use $select to reduce payload size
4. **Caching**: Consider implementing caching for frequently accessed data
5. **Query Complexity**: Monitor and limit complex queries that might impact performance

## Security

-   OData queries are validated server-side
-   Maximum query depth and complexity are limited
-   Authentication and authorization are enforced as per regular API endpoints
-   Input validation is performed on all operations

## Integration with Existing API

OData endpoints work alongside existing REST API endpoints:

-   Use OData for complex queries and reporting
-   Use REST API for simple CRUD operations
-   Both share the same underlying data model and business logic
