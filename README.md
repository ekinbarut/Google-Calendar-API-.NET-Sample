# Google-Calendar-API-.NET-Sample

Google Calendar API sample for .NET

What it does is basically getting the public holidays from a public calendar and inserts it into a service account calendar.

To authenticate for the public calendar, you need to go to http://console.developers.google.com with your google account and enable Calendar API and get a json file named "credentials.json". Make sure to include the json file in the project and set "Copy to output folder" to always in the file properties. The authenticator will read it from the bin file on runtime.

To authenticate for the service account, you need to go to http://console.developers.google.com and create a service account, allow the google calendar API to access your service account and get a credential key in p12 format from the credentials section of your service account. You could also choose to authenticate through a json file like authenticating as a user, but i chose the p12 file, because why not. It works the same way as the json file so you need to include it in the project and set copy to output folder to always.

The application doesn't do much, but google authentication is a b**** and took a huge portion of my time so I wanted to share. Don't try to authenticate to google with a json string if you're using a service account, only way to authenticate is through reading a physical file. (I wish someone has told me about this before) 
