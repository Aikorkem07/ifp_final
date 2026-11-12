# Project Ownership and Team Contributions

**Project:** Delivery Management System  
**Course:** Introduction to Functional Programming  
**Group:** IT-2513  
**Team Members:** 4

## 1. Team Responsibilities

The project is divided into four areas of responsibility. Each member is responsible for understanding, implementing, testing, and explaining their assigned components.

### 1. Muktar Aikorkem — Core Application Development

**Main responsibilities:**
- Core delivery models and immutable data structures
- Input validation and domain rules
- Delivery registration, confirmation, and pricing
- Delivery operations and functional processing
- Integration of the core application logic

**Assigned files:**
- `DeliveryApp/Models.cs`
- `DeliveryApp/InputParser.cs`
- `DeliveryApp/DeliveryOperations.cs`
- `DeliveryApp/DeliveryPipeline.cs`

### 2. Nauryzgali Yerassyl — Automated Testing

**Main responsibilities:**
- Testing delivery workflows
- Testing dispatch and delivery completion
- Testing order cancellation
- Testing invalid state transitions
- Verifying expected results and error handling

**Assigned files:**
- `DeliveryTests/Program.cs`
- `DeliveryTests/DeliveryTests.csproj`

The test runner is shared by the team. Yerassyl is responsible for coordinating and verifying the delivery lifecycle tests.

### 3. Bizhan Elnur — Reports and Native Integration

**Main responsibilities:**
- LINQ-based reports and aggregations
- Data filtering, sorting, and grouping
- Custom extension methods
- Native C library integration using P/Invoke

**Assigned files:**
- `DeliveryApp/DeliveryLedgerReports.cs`
- `DeliveryApp/Extensions.cs`
- `DeliveryApp/NativeMethods.cs`
- `native/delivery_native.c`

### 4. Assylkhan Sansyzbay — JSON Persistence and Application Integration

**Main responsibilities:**
- JSON data storage and retrieval
- Handling missing and malformed files
- Preparing sample data
- Application entry point and demonstration workflows

**Assigned files:**
- `DeliveryApp/LedgerJsonStore.cs`
- `DeliveryApp/FinalDemo.cs`
- `DeliveryApp/Program.cs`
- `DeliveryApp/samples/deliveries.json`
- `DeliveryApp/samples/empty.json`
- `DeliveryApp/samples/invalid.json`
- `DeliveryApp/samples/malformed.json`

## 2. Shared Responsibilities

All team members participate in:

- Reviewing and understanding the complete application
- Debugging and integration
- Testing the final project
- Preparing the group report
- Preparing for the individual defense

The repository contains one integrated application. Shared project configuration, build scripts, and documentation are maintained collaboratively.

## 3. GitHub Collaboration

All team members use the same repository:

https://github.com/Aikorkem07/ifp_final

Each member should commit their actual contributions using descriptive commit messages.

## 4. Individual Contributions

Each student prepares an individual contribution report describing:

- Tasks actually completed
- Files and functions worked
