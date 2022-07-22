# Frusko's Asp.Net Core Solution Builder

Welcome to Frusko's ASP.NET Core Solution Builder. This tool will create a asp.net core solution for you to start your projects with. Simply run the console library, 
input your solution name and where you'd like to save and let the tool work it's magic. This will produce a web project for your front-end, along with an API back-end 
to allow for saving to an SQL database.

## After Use - Additional setup
The tool is designed to allow you to get right into developing. However, due to some faults of my own, some setup may be required. You might need to perform some of the following steps:
- Take the IIS url for the API and place it in the {project}ApiConstants test host url - See ***Known Issues***
- Create a database migration and update the database context.
- Want to use IIS Express with Visual Studio? Set web project as startup project. Select dropdown on the build project button (the green play button) and select IIS Express. Repeat for the API project too and then IIS will be in the system tray when you debug.
- Set startup projects in visual studio to web and api projects.

## Known Issues
- Sometimes the API host url will hang. Not sure what causes this. Sometimes it works and sometimes it doesn't. If this isn't working for you, select 'n' when the tool asks if you want to generate the api host url - pending development.
- Razor Compilation doesn't work by default - fix incoming
- Gulpfile doesn't move bootstrap icon fonts - fix incoming
- There is no BaseViewModel - fix incoming
- The register and login pages have no style - pending development.
- Result with generic parameter throws an error when mapping between model and DTO - pending development

## Upcoming Updates
- Improved Includes system using IQueryable.
