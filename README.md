# Delivery Management System — Final Project

**Course:** Introduction to Functional Programming  
**Group:** IT-2513  
**Programming Language:** C#  
**Framework:** .NET 8  

## 1. Project Overview

Delivery Management System is a C# console application developed for the Introduction to Functional Programming final project.

The application manages delivery orders, calculates delivery costs, validates user input, tracks delivery statuses, generates reports, and saves data using JSON.

The project integrates concepts and functionality from previous assignments, including functional pipelines, immutable data, pattern matching, Result and Maybe types, LINQ, extension methods, lazy evaluation, and native C interoperability.

## 2. Team Members

| Student | Assigned Responsibilities |
|---|---|
| Muktar Aikorkem | Core delivery logic, models, validation, pricing, functional pipeline |
| Nauryzgali Yerassyl | Delivery lifecycle testing, workflow validation, automated tests |
| Bizhan Elnur | LINQ reports, extension methods, native C integration |
| Assylkhan Sansyzbay | JSON persistence, file handling, application integration |

These responsibilities describe the team's agreed division of work. Individual contribution reports should describe the work actually completed and verified by each student.

## 3. Main Features

The application supports six delivery workflows:

1. Register a delivery order.
2. Confirm an order and calculate its price.
3. Dispatch a confirmed delivery.
4. Mark a dispatched order as delivered.
5. Cancel an eligible delivery.
6. Reprice a confirmed delivery using a selected pricing policy.

Additional functionality includes:

- Input validation and domain error handling
- Immutable delivery models
- Safe delivery status transitions
- Standard and express pricing policies
- LINQ-based reporting and aggregation
- JSON saving and loading
- Missing and malformed file handling
- Lazy evaluation using `yield return`
- Native C library integration through P/Invoke
- Automated workflow and file-handling tests

## 4. Project Structure

```text
ifp_final/
├── DeliveryApp/
│   ├── DeliveryApp.csproj
│   ├── Program.cs
│   ├── Models.cs
│   ├── InputParser.cs
│   ├── DeliveryOperations.cs
│   ├── DeliveryPipeline.cs
│   ├── DeliveryLedgerReports.cs
│   ├── Extensions.cs
│   ├── NativeMethods.cs
│   ├── LedgerJsonStore.cs
│   ├── FinalDemo.cs
│   └── samples/
│       ├── deliveries.json
│       ├── empty.json
│       ├── invalid.json
│       └── malformed.json
├── DeliveryTests/
│   ├── DeliveryTests.csproj
│   └── Program.cs
├── native/
│   └── delivery_native.c
├── scripts/
│   ├── build-native-windows.bat
│   └── build-native-linux.sh
├── README.md
├── OWNERSHIP.md
└── .gitignore
```

<img width="335" height="902" alt="image" src="https://github.com/user-attachments/assets/194c5a09-8af7-4bda-8b8a-e8de35622b85" />



## 5. Functional Programming Concepts

The project demonstrates:

- **Immutability:** Delivery records are represented using immutable data structures.
- **Pure functions:** Delivery calculations and transformations are separated from file I/O.
- **Result and Maybe:** Explicit handling of successful operations, errors, and missing values.
- **Map and Bind:** Composition of operations that may fail.
- **Pattern matching:** Handling delivery states and operation results.
- **Higher-order functions:** Configurable pricing policies using `Func`.
- **Function pipelines:** Combining multiple processing stages.
- **LINQ:** Filtering, sorting, grouping, and aggregating deliveries.
- **Extension methods:** Reusable operations for formatting and processing data.
- **Lazy evaluation:** Deferred processing using `yield return`.

## 6. Requirements

- .NET SDK 8
- Visual Studio or Visual Studio Code
- A C compiler for building the native library
- Git

## 7. How to Run

Clone the repository:

```bash
git clone https://github.com/Aikorkem07/ifp_final.git
cd ifp_final
```

Build the application:

```bash
dotnet build DeliveryApp/DeliveryApp.csproj
```

Run the final project demonstration:

```bash
dotnet run --project DeliveryApp -- final
```

Run the original delivery calculator demonstration:

```bash
dotnet run --project DeliveryApp -- demo
```

The original demonstration uses a native C library. Build the library for your operating system before running that mode.

For Windows, run:

```bat
scripts\build-native-windows.bat
```

For Linux, run:

```bash
bash scripts/build-native-linux.sh
```

## 8. Automated Testing

The test project contains automated checks for delivery operations, invalid states, pricing, reports, and file handling.

Run the tests using:

```bash
dotnet run --project DeliveryTests
```

The tests use a custom assertion runner and report failures through the process exit code.

## 9. Error Handling

The application handles several domain and file errors, including:

- Invalid delivery data
- Duplicate delivery IDs
- Invalid delivery status transitions
- Missing JSON files
- Malformed JSON data

Expected domain failures are represented explicitly instead of relying only on exceptions.

## 10. Reports

The project includes LINQ-based reports for:

- Active delivery orders sorted by price and ID
- Delivery counts and totals grouped by status
- Delivery quotes and totals grouped by native delivery zone

## 11. Team Contributions

**Muktar Aikorkem:** Core delivery models, input validation, registration, confirmation, repricing, and functional processing.

**Nauryzgali Yerassyl:** Automated tests for delivery workflows, cancellation, dispatching, delivery completion, and invalid transitions.

**Bizhan Elnur:** Delivery reports, LINQ operations, extension methods, and native C interoperability.

**Assylkhan Sansyzbay:** JSON persistence, sample data, file error handling, and application integration.

## 12. Project Status

The source code and automated tests are included in the project archive.

Final compilation, execution, and test results must be verified in a local .NET 8 environment before submission.

## 13. AI Assistance

AI-assisted tools were used during project preparation and documentation. All generated or assisted material must be reviewed, tested, and understood by the team. The final contribution reports should accurately describe individual work and follow the instructor's AI-use requirements.

## 14. Repository

GitHub: https://github.com/Aikorkem07/ifp_final
<img width="1920" height="1200" alt="image" src="https://github.com/user-attachments/assets/a51b86d5-a2d3-499b-9976-6a30f129acb5" />
<img width="797" height="138" alt="image" src="https://github.com/user-attachments/assets/c8279913-784d-42ee-93b2-db6f581a0c20" />

