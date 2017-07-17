# Bunifirebase
Firebase Db/Auth/ Storage Library for .Net Winform users

#Initialize Firebase - Paste the web config object
 ```csharp
    Firebase.initialize(@" {
                                    apiKey: 'AIzaSyBmhPr2YWgQEq40nw4ut4598li1d1aaxM',
                                    authDomain: 'myapp-7ec0a.firebaseapp.com',
                                    databaseURL: 'https://reactapp-7ec0a.firebaseio.com',
                                    projectId: 'myapp-7ec0a',
                                    storageBucket: 'myapp-7ec0a.appspot.com',
                                    messagingSenderId: '159614956476'
                                  }", this);
                                  ```
#root reference
 ```csharp
  root = Firebase.database();
 ```
#reference and events 
 ```csharp
    name = root.child("name");
             name.onValue += Name_onValue;
 ```
#sample event
 ```csharp
  private void Name_onValue(object sender, EventArgs e)
        {
             txtuser.Text = ((firebaseRef)sender).Value;
        }
 ```
