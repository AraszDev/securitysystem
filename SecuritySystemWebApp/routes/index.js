var express = require('express');
var router = express.Router();
var env = require('node-env-file');
env('./.env')
var fs = require('fs');
var azure = require('azure-storage');
var bigInt = require('big-integer');
var blobService = azure.createBlobService();


router.get('/',
  ensureAuthenticated,
  function(req, res) {
    console.log("opening home");
    res.render('index');
});

router.post('/images',
  ensureAuthenticated,
  function(req,res){
    var images = [];
        blobService.listBlobsSegmented('imagecontainer', null, function(error, result, response){
          if(!error){
            images = result;
            // console.log(images.entries)
             for(var i = 0; i < images.entries.length; i++){
                // using npm module bigInt, because the number of .NET ticks
                // is a number with too many digits for vanilla JavaScript
                // to perform accurate math on.
                var ticks = images.entries[i].name.slice(0,18);
                var ticksAtUnixEpoch = bigInt("621355968000000000")
                var ticksInt = bigInt(ticks);
                var ticksSinceUnixEpoch = ticksInt.minus(ticksAtUnixEpoch);
                var milliseconds = ticksSinceUnixEpoch.divide(10000)
                //Converting millisecond to dateTime client side so it will
                //display in the user's local timezone.
                images.entries[i].milliseconds = milliseconds.value
              }
            console.log(images)
            res.send({images: images.entries, storageService: "azure"});
          } else {
            res.send(error);
          }
        })
})


router.get('/image/:imagename', ensureAuthenticated, function(req, res){
  var startDate = new Date();
  var expiryDate = new Date(startDate);
  expiryDate.setMinutes(startDate.getMinutes() + 5);
  startDate.setMinutes(startDate.getMinutes() - 5);

  var sharedAccessPolicy = {
    AccessPolicy: {
      Permissions: azure.BlobUtilities.SharedAccessPermissions.READ,
      Start: startDate,
      Expiry: expiryDate
    },
  };
  var token = blobService.generateSharedAccessSignature('imagecontainer', req.params.imagename, sharedAccessPolicy);
  var tempUrl = blobService.getUrl('imagecontainer', req.params.imagename, token);

  res.send(tempUrl);
});


router.get('/login', function(req, res){
  res.render('login');
});

function ensureAuthenticated(req, res, next) {
  if (req.isAuthenticated() || req.session.passportInitialized === false) {
    return next();
  }
  console.log("not authenticated, going to login page");
  res.redirect('/login');
}

module.exports = router;
