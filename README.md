## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Admin AAN Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-admin-aan-web?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3341&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-admin-aan-web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-admin-aan-web)

This web solution is part of Apprentice Ambassador Network (AAN) project. AAN requires admin users to create and manage events which are organised by the ambassadors. Also there is a need to manage ambassadors (members). These two journies are enabled in this solution. 

## How It Works

The web module uses default landing dashboard on admin portal [das-admin-service](https://github.com/SkillsFundingAgency/das-admin-service). 

There are two types of admin users, pure admin users who mainly manage events and then there are regional chairs who can also manage ambassadors. The users have to have a working DFE Sign-in to authenticate and they should have at least one of the following claims: 
- AMM Manage members role
- AME Manage events role

## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* An Azure Active Directory account with the appropriate roles as per the [config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-tools-servicebus-support/SFA.DAS.Tools.Servicebus.Support.json).
* The Outer API [das-apim-endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/AdminAan) should be available either running locally or accessible in an Azure tenancy.
* The Inner API [das-aan-hub-api](https://github.com/SkillsFundingAgency/das-aan-hub-api) should be available either running locally or accessible in an Azure tenancy.

### Config
You can find the latest config file in [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-admin-aan-web/SFA.DAS.AdminAan.Web.json)

In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.AdminAan.Web,SFA.DAS.Provider.DfeSignIn",
  "EnvironmentName": "LOCAL",
  "ResourceEnvironmentName": "LOCAL",
  "cdn": {
    "url": "https://das-test-frnt-end.azureedge.net"
  }
} 
```

## Technologies
* .NetCore 6.0
* NUnit
* Moq
* FluentAssertions
* RestEase
* MediatR
