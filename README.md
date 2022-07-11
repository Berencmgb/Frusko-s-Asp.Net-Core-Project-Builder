# Frusko's Asp.Net Core Solution Builder

Welcome to Frusko's ASP.NET Core Solution Builder. This tool will create a asp.net core solution for you to start your projects with. Simply run the console library, 
input your solution name and where you'd like to save and let the tool work it's magic. This will produce a web project for your front-end, along with an API back-end 
to allow for saving to an SQL database.

## After Use - Additional setup
The tool is designed to allow you to get right into developing. However, due to some faults of my own, some setup may be required. You might need to perform some of the following steps:
- Take the IIS url for the API and place it in the {project}ApiConstants test host url - See ***Known Issues***
- Create a database migration and update the database context.
- Set startup projects in visual studio to web and api projects.

## Known Issues
- Sometimes the API host url will hang. Not sure what causes this. Sometimes it works and sometimes it doesn't. If this isn't working for you, select 'n' when the tool asks if you want to generate the api host url.
