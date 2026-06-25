# 🎓 UniManage - University Course Management System

UniManage is a web-based University Course Management System developed using **ASP.NET Core MVC** and **SQL Server**. The system provides a centralized platform for administrators, lecturers, and students to manage academic activities efficiently.

This project was developed as part of the **CS6004ES - Application Development** module.

---

## 📖 Overview

UniManage simplifies university administration by providing role-based access to academic services. The system allows administrators to manage users, courses, and modules, lecturers to manage teaching activities, and students to access learning resources and submit assignments.

---

## ✨ Features

### 👨‍💼 Administrator

- Secure Login
- Dashboard with statistics
- Manage Students
- Manage Lecturers
- Manage Administrators
- Manage Courses (Programs)
- Manage Modules
- Assign Modules to Courses
- View Enrollment Requests
- Approve/Reject Enrollments
- Generate Reports
- View System Statistics

---

### 👨‍🏫 Lecturer

- Secure Login
- Dashboard
- View Assigned Modules
- Upload Learning Materials
- Create Assignments
- View Student Submissions
- Grade Assignments
- View Reports
- Student Communication

---

### 👨‍🎓 Student

- Secure Login & Registration
- Dashboard
- Browse Available Programs
- Request Enrollment
- View Enrolled Modules
- Download Learning Materials
- Submit Assignments
- View Grades & Feedback
- Update Profile
- Change Password
- Messaging System

---

### 💬 Communication Module

- Private Messaging
- Group Messaging
- Notification System
- Unread Message Counter

---

### 📊 Reporting

- Student Reports
- Lecturer Reports
- Course Statistics
- Assignment Statistics
- Enrollment Statistics

---

## 🛠️ Technologies Used

| Technology | Purpose |
|------------|---------|
| ASP.NET Core MVC | Web Application Framework |
| C# | Backend Development |
| Entity Framework Core | ORM |
| SQL Server | Database |
| Razor Views | Frontend |
| HTML5 | Structure |
| CSS3 | Styling |
| JavaScript | Client-side Interactions |
| Bootstrap | Responsive Design |

---

## 🏗️ System Architecture

```
Presentation Layer
       │
       ▼
ASP.NET MVC Controllers
       │
       ▼
Business Logic
       │
       ▼
Entity Framework Core
       │
       ▼
SQL Server Database
```

---

## 👥 User Roles

- Administrator
- Lecturer
- Student

Each role has different permissions through role-based authorization.

---

## 📂 Project Structure

```
UniManage
│
├── Controllers/
├── Models/
├── Views/
│   ├── Admin
│   ├── Lecturer
│   ├── Student
│   ├── Account
│   └── Shared
│
├── wwwroot/
│   ├── css
│   ├── js
│   ├── images
│   └── uploads
│
├── Data/
├── ViewModels/
└── Program.cs
```

---

## 🗄️ Database

The system uses **Microsoft SQL Server**.

Main tables include:

- Users
- Students
- Lecturers
- Courses
- Modules
- CourseModules
- Enrollments
- Assignments
- AssignmentSubmissions
- Grades
- Materials
- Notifications
- Departments
- Messages
- Groups

---

## 🔒 Security Features

- Authentication
- Role-Based Authorization
- Password Protection
- Session Management
- Input Validation
- File Upload Validation
- SQL Injection Protection (Entity Framework)
- Cross-Site Request Forgery (CSRF) Protection

---

## 📸 Screenshots

Add screenshots here after deployment.

Example:

```
screenshots/
│
├── login.png
├── admin-dashboard.png
├── student-dashboard.png
├── lecturer-dashboard.png
└── reports.png
```

---

## 🚀 Installation

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio

---

### Clone Repository

```bash
git clone https://github.com/yourusername/UniManage.git
```

---

### Configure Database

Open **appsettings.json**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=UniManageDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### Update Database

Run:

```bash
Update-Database
```

or

```bash
dotnet ef database update
```

---

### Run Project

```bash
dotnet run
```

or simply press

```
F5
```

inside Visual Studio.

---

## 📌 Future Improvements

- Email Notifications
- Attendance Management
- Timetable Management
- Online Exams
- Payment Integration
- Mobile Application
- Two-Factor Authentication
- Data Visualization Dashboard

---

## 📚 Learning Outcomes

This project demonstrates:

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server Integration
- Authentication & Authorization
- CRUD Operations
- File Upload Handling
- Session Management
- Role-Based Access Control
- MVC Architecture
- Responsive Web Design

---

## 👨‍💻 Developer

**Pramod Kaushal Fernando**

Software Engineering Undergraduate


