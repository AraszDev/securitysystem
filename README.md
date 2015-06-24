#SecuritySystem


##Setting Up Azure
1. Create a .env file in the root of `security_system_web_app` and add:
```
AZURE_STORAGE_ACCOUNT={YOUR_ACCOUNT_NAME}
AZURE_STORAGE_ACCESS_KEY={YOUR_ACCOUNT_KEY}
```

##Configuring OAuth with PassportJS
1. For each service you'd like to use for authorization, you must register a new app.
  - Facebook: https://developers.facebook.com/apps/
  - GitHub: https://github.com/settings/developers

2. Once you've registered the app, take the credentials assigned to you and add them to your .env file as follows:
```
GITHUB_CLIENT_ID={YOUR_GITHUB_CLIENT_ID}
GITHUB_CLIENT_SECRET={YOUR_GITHUB_CLIENT_SECRET}
FACEBOOK_APP_ID={YOUR_FACEBOOK_APP_ID}
FACEBOOK_APP_SECRET={YOUR_FACEBOOK_APP_SECRET}
```

3. Rather than creating an entire database structure to house the user information, all users you'd like to be able to access this app must be added manually to 'allowed-users.js' in the following format:
```
var users = {
  "{USER_GITHUB_USERNAME}" : "GitHubUsername",
  "{USER_FACEBOOK_EMAIL}" : "FacebookEmail"
};
```
For GitHub auth, you must provide the GitHub username of anyone you'd like to be able to access the site, and for Facebook, you must provide the primary email associated with whichever Facebook user to whom you'd like to grant access.  Because the passport strategies for Facebook and GitHub search only for the key inside of the users object, be sure not to place the actual username and e-mail as the value in the object's key-value pair.

From this point, you can grant access to as many GitHub users as you like, but in order to allow anyone but the Facebook app creator to access the site, you must take your app out of development mode in the app settings on the Facebook Developer's page.


##Setting Up Your Camera

##Debugging

If you are not seeing images appear in BLOB storage, it is possible that your RPi2's time is out of sync. If this is the case, you should sync it before proceeding. Otherwise the Azure authentication will fail.