# Architecture
Two separate applications - React frontend and backend API service. 
Target was changed from deprecated .NET 5 to .NET 6.
Docker file not update and probably not working, but application working correctly (run from visual studio)

# Projects in solution:
- TranslationManagement.API
> - RESTFull API for managing Translators and Jobs - see swagger for details
> - Not all methods implemented for Translators
- TranslationManagement.Core
> - Models and bussiness logic
> - Should be re-used for future implentation of reading from QUEUE
- TranslationManagement.Web
> - Fronted React application
> - Creating job and refresh unassgined jobs
> - Simple list of Translators and Jobs
> - backend service url+port configuration in .env file
- TranslationManagement.Test
> - Tests 

# Caveats:
- statuses as string - should be Enums
- customer as string - should be separate entity
- Translator CreditCardNumber 
- missing validaton, is it good to process and store CreditCardNumber to DB?
- very lower coverage, have to work on test more
- API lists pagination and better filtering missing
- add more comments
- databese versioned also in git - definitelly not common practice! should be replaced with some seeding code for Development
