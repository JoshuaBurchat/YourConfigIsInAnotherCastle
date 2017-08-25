Included in this package is a series of scripts and html that will give your application the ability to make use of the WebAPI restful services (Also provided with this package).


Setup Steps:
1) Copy either the contents or the whole file management.html into a view or html page you wish to display the content in.
	a) You will note that the script tags are included for the required libraries
	https://cdn.polyfill.io/v2/polyfill.min.js
    https://unpkg.com/react@15.3.1/dist/react.min.js
    https://unpkg.com/react-dom@15.3.1/dist/react-dom.min.js
    https://unpkg.com/react-jsonschema-form/dist/react-jsonschema-form.js
	The code has been tested against these versions but feel free to update and test out newer releases.

	b) The script tags supporting this html are also included and have been placed in the same directory as this file
	camel-object-formatter.js
	react-jsonschema-form-wrapper.js
	yourconfig-data-services.js
	yourconfig-viewmodels.js
	yourconfig-ui-controller.js
2) In your HTML page/layout you will also need to include Bootstrap JS for styles and dialog functionality. Do this through either Nuget or a CDN of your choosing.

3) You will notice that a controller WebAPI ConfigController.cs has also been added to the project in the Controllers folder. If you have a different routing system you
will need to ensure that you modify the URLs located in the yourconfig-data-services.js script

4) After the html and the scripts have been added to your view you should be able to execute the application and manage your configuration.
*Note this assumes you have already setup your database and configuration, if not please see other tutorials at https://github.com/JoshuaBurchat/YourConfigIsInAnotherCastle, in particular the migrations section.










Script/HTML File Descriptions

management.html
- This contains the html and script includes that will provide you with a working configuration management screen

camel-object-formatter.js
- Used to convert .Net formatted JSON (capital on property names) to camel case for a more JS feel. This can be removed if you have already configured WebAPI to to be camel case formatted.
* Note if this file is removed it will not break any other code, it is designed to have a fall back.. so delete away as long as you have camel cased your JSON.

react-jsonschema-form-wrapper.js
- This script is used to wrap around the react-jsonschema-form library created by Mozilla, this is just to make it easier to use within the other scripts, and make it replaceable if need be
- Check out the library at their git location, this is honestly the key feature of this UI component.
https://github.com/mozilla-services/react-jsonschema-form


yourconfig-data-services.js
- The Data services module will interact with the WebAPI http requests. This is simply to encapsulate the UIs data access layer. 

yourconfig-viewmodels.js
- All logic related to handling the model will be taken care of in the view model scripts. These scripts will not include any UI features, and therefore the UI can be replaced while still have the same functionality

yourconfig-ui-controller.js
- This controller will bring all of the components together with the UI, binding events and recieving messages for UI updates. 
- It will us all previously mentioned scripts.
