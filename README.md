# IPL Merchandise Fan Store

A full-stack ecommerce assessment application for browsing and purchasing IPL franchise merchandise.

The application is built with:

- React
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- xUnit
- Moq
- SQLite In-Memory testing

> Demo application created for technical assessment purposes.
> It is not affiliated with or endorsed by IPL or its franchises.

---

## Features

### Product Catalog

- Browse merchandise across 10 franchises
- Jerseys
- Caps
- Flags
- Autographed merchandise
- Product pricing
- Inventory availability
- Product Details page
- Search by product name or description
- Filter by franchise
- Filter by product type
- Combined filtering
- Sorting

### Authentication

- User registration
- User login
- ASP.NET Core Identity
- JWT authentication
- Authenticated user context
- Protected frontend routes

### Shopping Cart

- Database-persisted cart
- Add products
- Update quantity
- Remove products
- Clear cart
- Stock validation
- Jersey size selection: M, L, XL
- Same jersey with different sizes stored as separate cart lines

### Checkout

Checkout is processed server-side.

During checkout the API:

1. Reloads the authenticated user's cart
2. Revalidates product availability
3. Revalidates inventory
4. Creates an Order
5. Creates OrderItem snapshots
6. Calculates order total
7. Reduces product stock
8. Clears cart items
9. Commits the transaction

### Order History

Authenticated users can:

- View previous orders
- View order number
- View order date
- View status
- View purchased merchandise
- View quantity
- View selected jersey size
- View purchase-time price
- View order total

Users can only access their own orders.

### Recommendations

After checkout, the storefront displays merchandise recommendations based on purchased franchise and product type.

---

# Architecture

The application follows a layered architecture.

```text
React Frontend
      |
      | HTTPS / JSON
      v
ASP.NET Core Web API
      |
      v
Application Layer
  DTOs / Interfaces
      |
      v
Infrastructure Layer
  Service Implementations
  EF Core
  Identity
      |
      v
Domain Layer
  Entities / Enums
      |
      v
SQL Server
```
