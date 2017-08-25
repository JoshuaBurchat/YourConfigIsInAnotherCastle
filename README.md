# YourConfigurationIsInAnotherCastle
A simple way of redirecting web configuration to another data store, services which enable editing of said data store.

### Development road map
- Client applications for managing data.
- Unit tests for all projects.
- Documentation, as highlighted below.
- Refactoring and cleaning up for public use.
- Ensuring all Nuget packages are stable and up to date.


### Documentation to come
- **Purpose**
	- What this library can do
    - The advantages/vision of this route
    - Who will benefit
    - Nuget Packages
- **Core**
	- Tutorial of the core library and how to manually implement it
- **Migrations** 
	- Tutorial of the migrations library and how to migrate your existing project in 5 steps.
    - WPF client application for advanced migrations
- **Service**
	- The WebAPI service that will allow interaction with the database. 
    - Tutorial on how to setup and use it through Nuget
- **Client Application**
	- 4 different client application examples, using https://github.com/mozilla-services/react-jsonschema-form as an editor for the configuration JSON.
    	This library works with React and Bootstrap so all examples will requires these be included. 
        
        The different examples are used so that you can choose the option that may fit best with your existing technologies.
    	1. Basic JQuery client
        2. Angular client 
        3. React client
        
	Each of these clients will be included in this repo, but may also have an installable Nuget package.
    
    There will also be a full **MVC + WebAPI** project that contains both the **Service and the React Client**, and it can be used as it to manage your data.
    
    

    
    

