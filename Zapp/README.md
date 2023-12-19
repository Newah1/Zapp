> ## Zap

Grabs your up-to-date electricity consumption from pplelectric.com. 

To start, simply add PPLUsername & PPLPassword to your environment variables. 

Selenium will log in as your user and retrieve the security token handed off to you by PPL. 

It will retry a few times if it runs into any security issues. 

The response includes information about estimated bills, kWh sums and billing period. 
