![logo](https://github.com/user-attachments/assets/774b1e38-71a9-4598-aa86-39b71d22a1e3)




# 📚 Bookify - A Full-Featured Book Rental System with Clean Architecture & Key Patterns

## 📋 Overview

**Bookify** is an advanced book rental management platform designed to optimize library operations and improve user interaction.The system offers a full suite of features for managing book inventory, handling subscribers, processing rentals, and overseeing administrative tasks.

---

## 📖 **Elements of Contents**

1. [Key Advantages](#key-advantages)
2. [System Architecture](#system-architecture)
3. [Key Functionalities](#key-functionalities)
   - [Book Management](#book-management)
   - [Rental System](#rental-system)
   - [User Management](#user-management)
4. [Technology Stack](#technology-stack)
5. [Security](#security)
6. [Setup Instructions](#setup-instructions)
7. [Demo Link](#demo-link)

---

##  **Key Advantages**
 -🎯Simple rental operations
- 📊 Comprehensive reporting 
- 📱 User-friendly design with real-time updates
- 🔐 Robust user security measures
- 📈 Flexible and scalable system architecture


## 🏗 **System Architecture**

Bookify is a comprehensive ASP.NET Core web application designed for efficient book rental management, following Clean Architecture principles. It offers key functionalities such as user authentication and authorization,book handling, and rental automation, along with advanced features like background job processing through Hangfire, detailed logging via Serilog, and integrated WhatsApp and email notifications🔔📯📣🔊.

The system is built using industry-standard patterns and principles including Repository Pattern, Unit of Work, SOLID Principles,and Dependency Injection, ensuring maintainability, scalability, and testability. Repository Pattern and Unit of Work are utilized for clean separation of data access and transaction management, while SOLID principles ensure modularity and flexibility.Dependency Injection is leveraged to manage system dependencies, promoting loose coupling and ease of extension.

Bookify's architecture ensures a clear separation between business logic, data access, and UI/Controllers layers, making it a robust and maintainable solution for book rental services.

---

## 🔑 **Key Functionalities**

### 📚 **Book Management**

- 🔖Categorization of books
- 🎥Image uploads
- 📊 Stock monitoring


### 🔄 **Rental System**

- Rental processing
- Due date management ⏱️ 🕕
- Automated fine calculation
- Email and WhatsApp notifications 📩 🔔📯📣🔊

### 👤 **User Management**

- 🔐 🔐Role-based permissions
- 🔑🔏 Secure login/authentication
- 📩 🔔 Email verification and profile management 

---
 
## 🛠 **Technology/Libraries Stack**

### **Backend**:
- **Backend Framework**: ASP.NET Core 8.0
- **ORM:** Entity Framework Core
-  **LINQ:** Used for querying and manipulating data efficiently.
- **Database:** SQL Server
- **Authentication** and **authorization:** ASP.NET Core Identity
- **Logging:** Serilog (used for logging errors and exceptions)
- ✅ **Architecture Patterns:** Clean Architecture && Service Layer Pattern
- ✅ **Design Patterns:** Repository Pattern && Unit of Work & Service Layer
- ✅ **SOLID Principles:** some of SOLID Principles  such as the single responsibility principle , Dependency Inversion Principle
- ✅ **Architectural Principles:** Dependency Injection (DI)
 - **File Storage**: Cloudinary
- **Background Tasks**: Hangfire
- **Mapping**: AutoMapper
- **WhatsApp API**: Twilio
- **Image Processing**: ImageSharp (SixLabors.ImageSharp)


### **Frontend**:
- **Markup Language**: HTML
- **Styling**: CSS
- **UI Framework**: Bootstrap
- **JavaScript Libraries**: jQuery, DataTables (Client and Server-side), TinyMCE (for rich text editing), Bootbox.js (for modal dialogs and confirmation boxes), Select2 (for enhanced select boxes), Charts (for data visualization), Typeahead (for search and autocomplete functionality)
- **AJAX Calls**: JavaScript for handling dynamic interactions
- **Export**: DataTables library is used for exporting data to various formats such as Excel, PDF, and CSV.
- **Animations**: Implemented using jqueryand CSS for adding smooth UI animations.
- **Export**: DataTables library is used for exporting data to various formats such as Excel, PDF, and CSV.
- **Pagination and Search**: Implemented with DataTables for dynamic data handling and interaction;
- **Google**: Fonts: Used for custom fonts and typography



  
---

## 🔐 **Security**

### Validations:

- **Client-side Validation**: Using JavaScript and jQuery.
- **Server-side Validatio**n: Using ASP.NET Core's Model Validation.
- Regular Expressions: Used for pattern matching and data validation (e.g., validating email format, phone numbers).
- Data Annotations (Attributes): Applied to models for basic validation (e.g., [Required], [MaxLength],[Remote]).
- Fluent Validation: For some of validations logic in seperated class.
- Fluent API:       for configuring complex some of relationships, indexes, constraints;
  
- **Database Constraints**:
- Unique Key: Ensures fields such as Name ,Title  are unique in table.
- Index: Used to optimize query performance and enforce uniqueness on specific columns.
- Sequence: Used for generating sequential values for unique identifiers in the database Level.

### Authentication:

- ASP.NET Core Identity with custom claim providers.
- Secure password storage (hashed).
- Email based account confirmation, password recovery, and user data updates (e.g., username, email, phone number).

### Data Protection:

- .use boh NET built-in data protection and Hashids.
-  HTTPS (SSL/TLS) encryption and CSRF protection.
  -logs for tracking events , Errors, Exceptions (Serilog Package).

---

## 🔧 **Setup Instructions**

### **Prerequisites**

- **.NET 8.0 SDK**
- **SQL Server**
- **Visual Studio 2022** (or **VS Code**)

### **Installation**

1. **Clone the repository**:
   ```bash
   git clone https://github.com/MohammedAwadEleash/AspDotNetMVCBookifyApplication
   ```
2. **Navigate to the project folder**:
   ```bash
   cd  Bookify

   ```
3. **Restore project dependencies**:
   ```bash
   dotnet restore
   ```
4. **Update the connection string** in `appsettings.json` to match your database settings.
5. **Apply migrations to create the database**:
   ```bash
   dotnet ef database update
   ```
6. **Run the application**:
   ```bash
   dotnet run
   ```

### **Configuration**

- All necessary configuration settings, such as connection strings and API keys, are stored in `appsettings.json`.
- **Serilog** is pre-configured to log data based on the settings in `appsettings.json`.
- **Hangfire Dashboard** is accessible at `/hangfire` (requires admin role).

---

## 🌐 **Demo Link**


**Admin Credentials**:

- **Username**: `AdminTest@Bookify.Com`
- **Password**: `P@ss*w@o#r$$d1>2,3`


Visit the live demo: [EleashBookifyDemo](http://eleashbookify.runasp.net/)
## 🎥 **Project Walkthrough Video**

Watch the full demo of the **EleashBookify** project by clicking the link below:  
📺 [View Demo Video](https://drive.google.com/file/d/1WB5beHCPZcW02aDjc3e5zn_Jm3NNirkD/view?usp=sharing)



## Thank You

A special thanks to everyone who has supported and contributed to the development of this project. Every effort, no matter how small, has made a significant impact💝. 

- (Mohammed Awad Eleash): For creating this project with passion and dedication.
![Logo_sm](https://github.com/user-attachments/assets/b12f147e-8d34-4acb-b859-c2eefe600faa)
