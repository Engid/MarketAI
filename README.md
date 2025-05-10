# MarketAI

A prototype Background Agent for exploring AI capabilities in .NET.

I created this as a way to learn Semantic Kernel. 

THIS IS NOT FINACIAL ADVICE. INVESTING IN STOCKS INVOLVES RISK. DO NOT MAKE DECISIONS BASED ON THIS DEMO.

## Design

This is implemented as a background worker that could be deployed in the cloud.
The workflow reaches out to AlphaVantage for their News and Sentiment analysis of 
articles about a Stock (in this case MSFT). The AI is then asked to review the summaries,
and give a final summary and prediciton about the stock. 


### Todo:
- wire up to blob storage, or a document DB
- switch to Azure OpenAI api for security
- give the AI more tools. 

# License 
I am making this available under the MIT license. Use it as you wish. 
