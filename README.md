## 📋 Prerequisites

Before running this application, ensure you have the following installed:

- **Visual Studio 2022** (Community, Professional, or Enterprise) or **Visual Studio Code**
- **.NET 8.0 SDK** or later
- **SQL Server 2022** (LocalDB, Express, or full instance)

### System Requirements

- Windows 10/11 or Windows Server 2019/2022
- 2GB free disk space

## 🚀 Quick Start Guide

### 1. Extract the Project

1. Download the SAIS project zip file
2. Extract the contents to your desired location
3. Open the solution in Visual Studio 2022 or Visual Studio Code

### 2. Configure Database Connection

The project includes two configuration files:

- `appsettings.Development.json` - Development settings
- `appsettings.Production.json` - Production settings

**For Development (Default):**
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SAISConnection": "Server=(localdb)\\mssqllocaldb;Database=SAIS_DB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**For Production:**
Update the connection string in `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "SAISConnection": "Server=YOUR_SERVER;Database=SAIS_DB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;MultipleActiveResultSets=true"
  }
}
```

**Database Credentials:**

- **Development**: Uses Windows Authentication (Trusted_Connection=true)
- **Production**: Requires SQL Server username and password
- **Database Name**: SAIS_DB (can be changed as needed)

### 3. Build and Run

```bash
dotnet build
dotnet run
```

**Note**: Database migrations will run automatically on first startup, creating the database and tables with seed data. The application automatically detects the environment and loads the appropriate configuration file.

### 4. Access the Application

Navigate to `https://localhost:5001` in your browser.

**Note**: The application will automatically use the appropriate configuration file based on the environment (Development/Production).

### Environment Configuration

**Development Environment (Default):**

- Uses `appsettings.json`
- Windows Authentication
- LocalDB for development
- Debug logging enabled
- Works with both Visual Studio 2022 and Visual Studio Code
- Automatic database creation and seeding on first run
- Environment automatically detected by ASP.NET Core

**Production Environment:**

- Uses `appsettings.Production.json`
- SQL Server authentication
- Full SQL Server instance
- Optimized logging
- Environment automatically detected by ASP.NET Core

To run in Production mode:

```bash
dotnet run --environment Production
```

## ✨ Features Implemented

### Core Functionality

- ✅ **Applicant Management**: Complete CRUD operations with personal information, contact details, and geographic data
- ✅ **Application Processing**: Streamlined workflow for handling social assistance applications
- ✅ **Officer Management**: Manage social workers and officers processing applications
- ✅ **Program Management**: Configure and manage social assistance programs
- ✅ **Geographic Hierarchy**: Full County → SubCounty → Location → SubLocation → Village structure

### Advanced Features

- ✅ **Combined Registration**: Register applicant and create application in one step
- ✅ **Smart Age/DOB Calculation**: Automatic calculation between age and date of birth
- ✅ **Cascading Dropdowns**: Dynamic geographic selection with AJAX
- ✅ **Multiple Phone Numbers**: Support for multiple contact numbers per applicant
- ✅ **Search & Filtering**: Advanced search across all entities
- ✅ **Pagination**: Configurable pagination (default page size: 2)
- ✅ **Export Capabilities**: Excel (CSV) and PDF export functionality
- ✅ **Preloader System**: Visual feedback during form submissions
- ✅ **Success Notifications**: Toast-style alerts for user feedback

### Data Validation

- ✅ **Unique ID Numbers**: Prevent duplicate applicant registrations
- ✅ **Date Validation**: Date of birth cannot be in the future
- ✅ **Required Fields**: Comprehensive validation for all mandatory fields
- ✅ **Business Rules**: Age range validation (0-120 years)
- ✅ **Duplicate Prevention**: Applicants cannot apply for the same program twice

## 📖 How to Use the Application

### For Data Entry Clerks

#### Option 1: Combined Registration & Application (Recommended)

1. Navigate to **Applicants → Register & Apply (Quick)**
2. Fill in applicant details:
   - Enter personal information (Name, ID Number)
   - Select either Age OR Date of Birth (the other calculates automatically)
   - Choose gender and marital status
   - Use cascading dropdowns for geographic selection
   - Add multiple phone numbers as needed
3. Complete application details:
   - Select the processing officer
   - Choose one or more social assistance programs
   - Set application and signature dates (defaults to today)
4. Click **Register & Apply** to create both applicant and application

#### Option 2: Quick Application (Existing Applicants)

1. Navigate to **Applications → Quick Application**
2. Enter the applicant's ID number
3. If found, applicant details auto-populate
4. Select officer, programs, and dates
5. Save the application

#### Option 3: Alternative Two-Step Process

1. **Register Applicant**: Navigate to **Applicants → Add New**
2. **Create Application**: Navigate to **Applications → New Application**

### For Administrators

#### Managing System Data

1. **Officers**: Navigate to **Management → Officers**
2. **Programs**: Navigate to **Management → Programs**
3. **Geographic Data**: Navigate to **Management → Counties** for hierarchical management
4. **Lookup Data**: Navigate to **Management → Gender Categories** or **Marital Statuses**

#### Generating Reports

1. Navigate to **Reports → Dashboard** for system overview
2. Use **Reports → Applications** for detailed analysis
3. Export data to Excel or PDF formats

## ⚙️ Admin/Configurable Items

### Geographic Management

- **Counties**: Manage county data and all associated children
- **Sub-Counties**: Create and manage sub-counties under counties
- **Locations**: Manage locations within sub-counties
- **Sub-Locations**: Manage sub-locations within locations
- **Villages**: Manage villages within sub-locations

### Lookup Data Management

- **Gender Categories**: Male, Female, Other
- **Marital Statuses**: Single, Married, Divorced, Widowed, Other
- **Social Assistance Programs**: Configurable program types
- **Officers**: Social workers and processing officers

### System Configuration

- **Pagination Settings**: Adjustable page sizes
- **Validation Rules**: Customizable business rules
- **Export Formats**: Configurable report formats

## 🗄️ Database Schema

### Core Entities

```
Applicant
├── Personal Information (Name, ID, DOB, Age)
├── Contact Details (Phone Numbers, Addresses)
├── Geographic Location (Village Reference)
└── Applications (One-to-Many)

Application
├── Applicant Reference
├── Officer Reference
├── Application Dates
└── Applied Programs (Many-to-Many)

Officer
├── Officer Information
└── Applications (One-to-Many)

SocialAssistanceProgram
├── Program Details
└── Applied Programs (Many-to-Many)
```

### Geographic Hierarchy

```
County (1) → SubCounty (Many)
SubCounty (1) → Location (Many)
Location (1) → SubLocation (Many)
SubLocation (1) → Village (Many)
```

## 🔧 Technical Implementation

### Architecture Patterns

- **MVC Pattern**: Clear separation of concerns
- **DTO Pattern**: Data transfer objects for API communication
- **Dependency Injection**: Built-in ASP.NET Core DI container

### Key Technologies

- **Entity Framework Core**: Code-First migrations and LINQ queries
- **Bootstrap 5**: Responsive UI framework
- **jQuery**: Client-side interactions and AJAX
- **Font Awesome**: Icon library for enhanced UI

## 🚧 Known Limitations & Future Improvements

### Current Limitations

- No user authentication/authorization system
- Limited reporting capabilities
- No document upload functionality
- No email/SMS notification system
- No audit trail for data changes

### Planned Future Enhancements

- **User Management**: Role-based access control
- **Advanced Reporting**: Charts, graphs, and analytics
- **Document Management**: File upload and storage
- **Notification System**: Email and SMS alerts
- **Audit Logging**: Complete change tracking
- **API Integration**: RESTful API for external systems
- **Mobile App**: Native mobile application
- **Workflow Management**: Application approval workflows

## 📊 Performance Considerations

- **Database Indexing**: Optimized queries with proper indexes
- **Pagination**: Efficient data loading for large datasets
- **Caching**: Client-side caching for dropdown data
- **Validation**: Client-side validation to reduce server load

## 🔒 Security Features

- **Input Validation**: Comprehensive data validation and sanitization
- **CSRF Protection**: Anti-forgery tokens on all forms
- **SQL Injection Prevention**: Entity Framework parameterized queries
- **XSS Protection**: Razor view encoding
