Programming Exercise 

The purpose of this task is to provide talking points for an interview, where the skill level is assessed. The interview will be on the method and understanding, and to a lesser extent on syntax and style. 

Domain 

The problem domain is a FX Exchange, where an amount in one currency is exchanged to another amount in another currency. This is commonly done using an ISO currency pair, e.g. EUR/DKK, where EUR is the main currency and DKK is money currency. Each currency pair is associated with an exchange rate, where 1 of the main currency can be exchanged to given amount in the money currency. For instance: EUR/DKK 7,4394, would denote that 1 EUR can be exchange with 7,4394 DKK. 

Task 
Using the exchange rates below, denoting the amount Danish kroner (DKK) required to purchase 100 in the mentioned currency, make a command line tool that is able to take a ISO currency pair and an amount, and write the exchanged amount to the console 

 

Currency 

ISO 

Amount 

Euro 

EUR 

743,94 

Amerikanske dollar 

USD 

663,11 

Britiske pund 

GBP 

852,85 

Svenske kroner 

SEK 

76,10 

Norske kroner 

NOK 

78,40 

Schweiziske franc 

CHF 

683,58 

Japanske yen 

JPY 

5,9740 

 

 

It is expected that a currency pair can contain any combination of the mentioned currencies (including DKK), e.g. EUR/USD. If a currency pair has the same ISO currency as
both main and money, then the passed-in amount should be returned. If a currency pair contains an unknown currency, then an appropriate error message should be written to 
console. 

It is expected that the code includes tests for the solution elements. 

Please note, that the firewall will restrict executables to be sent via email. As such you should only send the source-code if you decide to send it via email. 
Best way would be a public repository in GitHub for us to clone. We will build the application during the evaluation of your solution. 

Requirement 

Please provide a solution in either C# 

You may use any frameworks (testing, mocking etc.) if it is freeware and available for download from nuget.org or embedded in your solution. 

You’re welcome to hardcode exchange values for purpose of speed, but we always encourage candidates to think about real life applications and use real data sources, 
but you need to make sure that data source is available during testing. Same goes for persistence layer – do not setup any databases or data stores that demands special 
ad-hoc setup to run. 
