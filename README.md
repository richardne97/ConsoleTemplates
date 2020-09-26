# ConsoleTempletes
Console Application Templetes for .net framework with following features

**JwtWebApiSelfHost**
  
  A .net framework 4.6.2 self-host web api program template for micro-service structure.

  1. Implement trace listener for NLog, the Trace.WriteLine output will be recorded in log files throughout the entire console program and components.
  2. Auto restart while the program encounters an unhandled global exception. And it will be recorded as a Fatal event in log files.
  3. Auto register http server for windows os http.sys. The port can be set in settings.
  4. Disable control box for the console window in case of unexpected closing.
  5. Custom models are ready to be defined in the Model folder.
  6. Custom controllers are ready to be implemented in the Controllers folder.
  7. Web Api Controllers are protected by JWT authentication. An OAuth2 server for JWT format template is on the depository WebTemplates.
  8. JWT parameters (Audience, Issuer, and Security Key) are defined in settings.
  9. Injection ready.
  10. Swagger help page ready. The configurable parameters such as Titel, Company, Description, and Version strings are defined in Startup.cs.
   
  

