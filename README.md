#SecuritySystem

##Setting Up Azure
1. Create a .env file in the root of `security_system_web_app` and add:
```
AZURE_STORAGE_ACCOUNT={YOUR_ACCOUNT_NAME}
AZURE_STORAGE_ACCESS_KEY={YOUR_ACCOUNT_KEY}
```
2. Push this to Azure

##Configuring OAuth with PassportJS
1. For each service you'd like to use for authorization, you must register a new app.
  - Facebook: https://developers.facebook.com/apps/
  - GitHub: https://github.com/settings/developers

2. Once you've registered the app, take the credentials assigned to you and add them to your .env file as follows:
```
GITHUB_CLIENT_ID={YOUR_GITHUB_CLIENT_ID}
GITHUB_CLIENT_SECRET={YOUR_GITHUB_CLIENT_SECRET}
FACEBOOK_APP_ID={YOUR_FACEBOOK_APP_ID}
FACEBOOK_APP_SECRET={YOUR_APP_SECRET}
```

3. Rather than creating an entire database structure to house the user information, all users you'd like to be able to access this app must be added manually to 'allowed-users.js' in the following format:
```
var users = {
  "{YOUR_GITHUB_USERNAME}" : "",
  "{YOUR_FACEBOOK_NAME}": ""
};
```
The Facebook name you enter must match the display name on your Facebook account exactly.  This will be case sensitive, and the first and last names should be separated by a space.  Since Passport is searching only for the key in this object, the value associated with each key can be anything you like or, as in this example, just an empty string.


##Setting Up Your Camera

##Debugging

If you are not seeing images appear in BLOB storage, it is possible that your RPi2's time is out of sync. If this is the case, you should sync it before proceeding. Otherwise the Azure authentication will fail.