![logo](https://github.com/user-attachments/assets/08491b95-85e4-4d1f-bf85-df530ce50e8c)




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

## 🎯 **Key Advantages**

- 📊 Comprehensive reporting & analytics
- 📈 Enhanced user experience with a modern and responsive interface
- 📱 User-friendly design with real-time updates
- 🔐 Robust user security measures
- 📈 Flexible and scalable system architecture

---

## 🏗 **System Architecture**

Bookify is a comprehensive ASP.NET Core web application designed for efficient book rental management, following Clean Architecture principles. It offers key functionalities such as user authentication and authorization,book handling, and rental automation, along with advanced features like background job processing through Hangfire, detailed logging via Serilog, and integrated WhatsApp and email notifications.

The system is built using industry-standard patterns and principles including Repository Pattern, Unit of Work, SOLID Principles,and Dependency Injection, ensuring maintainability, scalability, and testability. Repository Pattern and Unit of Work are utilized for clean separation of data access and transaction management, while SOLID principles ensure modularity and flexibility.Dependency Injection is leveraged to manage system dependencies, promoting loose coupling and ease of extension.

Bookify's architecture ensures a clear separation between business logic, data access, and UI/Controllers layers, making it a robust and maintainable solution for book rental services.

---

## 🔑 **Key Functionalities**

### 📚 **Book Management**

- Inventory monitoring
- Categorization of books
- Image uploads
- Real-time stock tracking

### 🔄 **Rental System**

- Rental processing
- Due date management
- Automated fine calculation
- SMS/WhatsApp notifications

### 👤 **User Management**

- Role-based permissions
- Secure login/authentication
- Email verification and profile management

---

## 🛠 **Technology Stack**

- **Backend Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML, CSS, Bootstrap, jQuery, JavaScript (Ajax), DataTables
- **File Storage**: Cloudinary
- **Background Tasks**: Hangfire
- **Mapping**: AutoMapper
- **WhatsApp API**: Twilio

---

## 🔐 **Security**

### Authentication:

- ASP.NET Core Identity with custom claim providers.
- Secure password storage (hashed).
- Email-based account confirmation and password recovery.

### Data Protection:

- .NET built-in data protection with Hashids.
- SSL/TLS encryption, CSRF protection, and XSS safeguards.
- Audit logs for tracking critical events.

---

## 🛠 **Setup Instructions**

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
- **Hangfire Dashboard** is accessible at `/hangfire` (requires admin access).

---

## 🌐 **Demo Link**

Visit the live demo: [EleashBookifyDemo](http://eleashbookify.runasp.net/)

**Admin Credentials**:

- **Username**: `AdminTest@bookify.com`
- **Password**: `P@AdminEleashBookifyTest12,@}`

![Logo_sm](https://github.com/user-attachments/assets/ae015915-4a2d-49c3-b2bc-96a29443cd3d)
