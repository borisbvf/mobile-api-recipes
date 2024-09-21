# Recipes
Sample application for storing and editing recipes. Includes a mobile application and a backend service with a database.

## Technologies
* Mobile Application: .NET 8.0, .NET MAUI
* Backend Service: Python 3.11, FastAPI, SQLAlchemy
* Database: PostgreSQL

## Features
#### Mobile Application
* XAML UI
* REST API
* MVVM (Model-View-ViewModel) without CommunityToolkit library
* Email-based authentication
* Animation while loading the app
* Login Flow (LoadingPage -> LoginPage <-> MainPage) with Shell
* Light, dark, and system themes
* Localization with two available languages (English and Russian)
* Platforms
  * Android
  * Windows
#### Backend Service:
* Email-based authentication
* Code and authorization token generation
* Authorization token required for all data requests
* ORM layer for database interaction (SQLAlchemy)
* API documentation (FastAPI)

## Compilation and running
#### Mobile Application
.NET 8 and .NET MAUI are required.  
Open `/MobileApp/RecipeApp.sln` in Microsoft Visual Studio.  
Change constant **BaseUrl** in `/MobileApp/Services/RecipeService.cs` to backend service address.  
Build the project.
#### Backend Service
Docker and Docker Compose are required.  
Create in the main directory file `.env`. It should contain following parameters:  
```
PG_PSWD=
EMAIL_SMTP_SERVER=
EMAIL_SENDER=
EMAIL_PASSWORD=
```
`PG_PSWD` is a Postgresql password.  
The other three parameters are for configuring email that is used to send registration messages.  
Run command in the CLI:
```
docker compose up -d
```
## Future plan
* Ingredients with units
* Tags or categories
* Photo or image for a recipe
* Share a recipe with other apps
* Export of the recipes
