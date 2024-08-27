# Product Management System

This project is a **Product Management System** built using **ASP.NET Core**. It allows users to create, edit, and delete products, with features like image handling, form validation, and enhanced security measures.

## Features

- **Dynamic Product Management:**
  - Add, edit, and delete products easily.
  - User-friendly interface for seamless interaction.

- **Image Handling:**
  - Securely upload and manage product images.
  - Images are stored on the server with unique filenames to prevent conflicts.

- **Form Validation:**
  - Server-side validation using Data Annotations to ensure data integrity.
  - Custom validation for file types and sizes, ensuring only valid images are uploaded.

- **Security:**
  - Integrated security features including data protection and anti-forgery tokens.
  - Form validation helps prevent malicious data submissions.

- **Dependency Injection:**
  - Built-in Dependency Injection to manage services, ensuring modularity and testability.

- **Professional UI Design:**
  - Clean and modern interface designed using Razor Views and Tag Helpers.
  - Responsive design with a focus on usability and aesthetics.

## Technologies Used

- **ASP.NET Core:** The main framework used for building the application.
- **Razor Pages:** For creating dynamic views and managing the user interface.
- **Entity Framework Core:** For interacting with the database.
- **Dependency Injection:** For managing service lifetimes and dependencies.
- **Data Annotations:** For model validation and ensuring data integrity.
- **IIS Hosting:** The project is configured to run on IIS for production deployment.
- **Security:** Integrated features like anti-forgery tokens and secure file handling.
