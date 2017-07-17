// ***********************************************************************
// Assembly         : netFirebase
// Author           : KIM TOO FLEX
// Created          : 07-16-2017
//
// Last Modified By : KIM TOO FLEX
// Last Modified On : 07-16-2017
// ***********************************************************************
// <copyright file="Bunifirebase.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bunifu.Framework.Firebase
{
    /// <summary>
    /// Class Firebase.
    /// </summary>
    public static class Firebase
    {
        /// <summary>
        /// The browser
        /// </summary>
        static WebBrowser browser;
        /// <summary>
        /// The HTML
        /// </summary>
        static string HTML = "";
        /// <summary>
        /// The history
        /// </summary>
        static List<firebaseRef> history = new List<firebaseRef>();

        /// <summary>
        /// The configuration data
        /// </summary>
        public static string configData = null;

        /// <summary>
        /// The is busy
        /// </summary>
        public static bool isBusy = true;
        /// <summary>
        /// The security
        /// </summary>
        internal static int security = 10;
        /// <summary>
        /// The ji
        /// </summary>
        static JSinterface ji = new JSinterface();
        /// <summary>
        /// Initializes the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="Parent">The parent.</param>
        public static void initialize(string config, Form Parent)
        {
            history.Clear();

            browser = new WebBrowser();
            HTML = readResorce("firebase.html").Replace("//[config]", config);
            browser.Name = "firebase_host";
            browser.Visible = false;
            browser.ObjectForScripting = ji;
            Parent.Controls.Add(browser);
            browser.DocumentCompleted += Browser_DocumentCompleted;

            //load html
            configData = config;

        }

        static string readResorce(string filename)
        {
            try
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();
                StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("Bunifirebase." + filename));

                return _textStreamReader.ReadToEnd();
            }
            catch
            {
                MessageBox.Show("Error accessing firebase resources!");
                return null;
            }
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public static void AddChild(firebaseRef child)
        {
            history.Add(child);

        }

        /// <summary>
        /// Listens this instance.
        /// </summary>
        public static void Listen()
        {
            string html = compile();
            browser.DocumentText = html;
            //  MessageBox.Show(html);
        }


        /// <summary>
        /// Handles the DocumentCompleted event of the Browser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WebBrowserDocumentCompletedEventArgs" /> instance containing the event data.</param>
        private static void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isBusy = false;
        }

        /// <summary>
        /// Databases this instance.
        /// </summary>
        /// <returns>childRef.</returns>
        /// <exception cref="Exception">firebase database not initialized with configuration</exception>
        public static firebaseRef database()
        {

            if (configData == null)
            {
                throw new Exception("firebase database not initialized with configuration");

            }

            return new firebaseRef();
        }


        /// <summary>
        /// Executes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="Event">The event.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string exec(firebaseRef sender, string Event, string value)
        {
            try
            {
                return browser.Document.InvokeScript(sender.name + "_" + Event, new string[] { value }).ToString();

            }
            catch (Exception)
            {

                return null;
            }
        }
        /// <summary>
        /// Sets the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string set(string path, string value)
        {
            try
            {
                return browser.Document.InvokeScript("set", new string[] { path, value }).ToString();

            }
            catch (Exception)
            {

                return null;
            }
        }
        /// <summary>
        /// Removes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string remove(string path)
        {
            try
            {
                return browser.Document.InvokeScript("remove", new string[] { path }).ToString();

            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// Pushes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string push(string path, string value)
        {
            try
            {
                return browser.Document.InvokeScript("push", new string[] { path, value }).ToString();

            }
            catch (Exception)
            {

                return null;
            }
        }
        /// <summary>
        /// Updates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string update(string path, string value)
        {
            try
            {
                return browser.Document.InvokeScript("update", new string[] { path, value }).ToString();

            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public static void disconnect()
        {

        }

        /// <summary>
        /// Compiles this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string compile()
        {
            string compiled = HTML;
            foreach (firebaseRef item in history)
            {
                compiled = compiled.Replace("//[code]", item.code + "\n//[code]");
            }

            return compiled;
        }



        //JS ENTRY POINT
        /// <summary>
        /// Class JSinterface.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        [ComVisible(true)]
        public class JSinterface
        {
            /// <summary>
            /// Ons the value.
            /// </summary>
            /// <param name="Value">The value.</param>
            /// <param name="sender">The sender.</param>
            public void onValue(String Value, string sender)
            {
                foreach (firebaseRef item in Firebase.history)
                {
                    if (item.name == sender)
                    {
                        //invoke events
                        item.performOnChange(Value);
                        break;
                    }
                }
            }
            /// <summary>
            /// Ons the child added.
            /// </summary>
            /// <param name="Value">The value.</param>
            /// <param name="sender">The sender.</param>
            public void onChildAdded(String Value, string sender)
            {
                foreach (firebaseRef item in Firebase.history)
                {
                    if (item.name == sender)
                    {
                        //invoke events
                        item.performOnChildAdded(Value);
                        break;
                    }
                }
            }
            /// <summary>
            /// Ons the child changed.
            /// </summary>
            /// <param name="Value">The value.</param>
            /// <param name="sender">The sender.</param>
            public void onChildChanged(String Value, string sender)
            {
                foreach (firebaseRef item in Firebase.history)
                {
                    if (item.name == sender)
                    {
                        //invoke events
                        item.performOnChildChanged(Value);
                        break;
                    }
                }
            }
            /// <summary>
            /// Ons the child removed.
            /// </summary>
            /// <param name="Value">The value.</param>
            /// <param name="sender">The sender.</param>
            public void onChildRemoved(String Value, string sender)
            {
                foreach (firebaseRef item in Firebase.history)
                {
                    if (item.name == sender)
                    {
                        //invoke events
                        item.performOnChildRemoved(Value);
                        break;
                    }
                }
            }

        }
        /// <summary>
        /// Class setUpFirebase.
        /// </summary>
        public class setUpFirebase
        {


            /// <summary>
            /// Gets the emb version.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public static int GetEmbVersion()
            {
                int ieVer = GetBrowserVersion();

                if (ieVer > 9)
                    return ieVer * 1000 + 1;

                if (ieVer > 7)
                    return ieVer * 1111;

                return 7000;
            } // End Function GetEmbVersion

            /// <summary>
            /// Fixes the browser version.
            /// </summary>
            public static void FixBrowserVersion()
            {
                string appName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                FixBrowserVersion(appName);
            }

            /// <summary>
            /// Fixes the browser version.
            /// </summary>
            /// <param name="appName">Name of the application.</param>
            public static void FixBrowserVersion(string appName)
            {
                FixBrowserVersion(appName, GetEmbVersion());
            }
            /// <summary>
            /// Fixes the browser version.
            /// </summary>
            /// <param name="appName">Name of the application.</param>
            /// <param name="ieVer">The ie ver.</param>
            public static void FixBrowserVersion(string appName, int ieVer)
            {
                FixBrowserVersion_Internal("HKEY_LOCAL_MACHINE", appName + ".exe", ieVer);
                FixBrowserVersion_Internal("HKEY_CURRENT_USER", appName + ".exe", ieVer);
                FixBrowserVersion_Internal("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", ieVer);
                FixBrowserVersion_Internal("HKEY_CURRENT_USER", appName + ".vshost.exe", ieVer);
            } // End Sub FixBrowserVersion 

            /// <summary>
            /// Fixes the browser version internal.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="appName">Name of the application.</param>
            /// <param name="ieVer">The ie ver.</param>
            private static void FixBrowserVersion_Internal(string root, string appName, int ieVer)
            {
                try
                {
                    //For 64 bit Machine 
                    if (Environment.Is64BitOperatingSystem)
                        Microsoft.Win32.Registry.SetValue(root + @"\Software\Wow6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, ieVer);
                    else  //For 32 bit Machine 
                        Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, ieVer);


                }
                catch (Exception)
                {
                    // some config will hit access rights exceptions
                    // this is why we try with both LOCAL_MACHINE and CURRENT_USER
                }
            } // End Sub FixBrowserVersion_Internal 

            /// <summary>
            /// Gets the browser version.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public static int GetBrowserVersion()
            {
                // string strKeyPath = @"HKLM\SOFTWARE\Microsoft\Internet Explorer";
                string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
                string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };

                int maxVer = 0;
                for (int i = 0; i < ls.Length; ++i)
                {
                    object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                    string strVal = System.Convert.ToString(objVal);
                    if (strVal != null)
                    {
                        int iPos = strVal.IndexOf('.');
                        if (iPos > 0)
                            strVal = strVal.Substring(0, iPos);

                        int res = 0;
                        if (int.TryParse(strVal, out res))
                            maxVer = Math.Max(maxVer, res);
                    } // End if (strVal != null)

                } // Next i

                return maxVer;
            } // End Function GetBrowserVersion 


        }
    }


    /// <summary>
    /// Class firebaseRef.
    /// </summary>
    public class firebaseRef
    {

        /// <summary>
        /// The last value
        /// </summary>
        public string Value = null;
        /// <summary>
        /// The key
        /// </summary>
        public string Key = null;
        /// <summary>
        /// Occurs when [on value changed].
        /// </summary>
        public event EventHandler onValue = null;

        /// <summary>
        /// Occurs when [on child changed].
        /// </summary>
        public event EventHandler onChildChanged = null;
        /// <summary>
        /// Occurs when [on value added].
        /// </summary>
        public event EventHandler onChildAdded = null;
        /// <summary>
        /// Occurs when [on value deleted].
        /// </summary>
        public event EventHandler onChildRemoved = null;
        /// <summary>
        /// The name
        /// </summary>
        public string name = firebaseRef.generateName(Firebase.security);

        /// <summary>
        /// The code
        /// </summary>
        public string code = "";


        /// <summary>
        /// The parents
        /// </summary>
        List<firebaseRef> parents = new List<firebaseRef>();

        /// <summary>
        /// The child object
        /// </summary>
        string childObject = "";
        /// <summary>
        /// Prevents a default instance of the <see cref="firebaseRef" /> class from being created.
        /// </summary>
        public firebaseRef()
        {
            compile();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="firebaseRef" /> class.
        /// </summary>
        /// <param name="parent_ref">The parent reference.</param>
        /// <param name="path">The path.</param>
        public firebaseRef(firebaseRef parent_ref, string path)
        {
            this.childObject = path;
            this.parents.Add(parent_ref);
            compile();
        }

        /// <summary>
        /// Sets the specified object.
        /// </summary>
        /// <param name="Object">The object.</param>
        public void set(string Object)
        {
            Firebase.set(this.path(), Object);
        }

        /// <summary>
        /// Pushes the specified object.
        /// </summary>
        /// <param name="Object">The object.</param>
        /// <returns>System.String.</returns>
        public string push(string Object)
        {
            return Firebase.push(this.path(), Object);
        }
        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="Object">The object.</param>
        /// <returns>System.String.</returns>
        public string update(string Object)
        {
            return Firebase.update(this.path(), Object);
        }
        /// <summary>
        /// Removes this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string remove()
        {
            return Firebase.remove(this.path());
        }
        /// <summary>
        /// Pathes this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string path()
        {
            return Firebase.exec(this, "toString", null);
        }


        /// <summary>
        /// Compiles this instance.
        /// </summary>
        private void compile()
        {
            //reset  code
            code = "var " + this.name + "_sender='" + this.name + "';\n";
            // listener and event
            string onValue = "  " + this.name + @".on('value', function(snap) {
                                if((typeof snap.val())=='object') {
                                  window.external.onValue(JSON.stringify(snap.val())," + this.name + @"_sender);      
                                }
                                else
                                {
                                  window.external.onValue(snap.val()," + this.name + @"_sender);      
                                }                
                         });";
            // listener and event
            string onChildAdded = "  " + this.name + @".on('child_added', function(snap) {                            
                                if((typeof snap.val())=='object') {    
                                    window.external.onChildAdded(JSON.stringify(snap.val())," + this.name + @"_sender);      
                                }
                                else
                                {
                                    window.external.onChildAdded(snap.val()," + this.name + @"_sender);      
                                }                
                         });";
            // listener and event
            string onChildChanged = "  " + this.name + @".on('child_changed', function(snap) {                            
                                if((typeof snap.val())=='object') {    
                                    window.external.onChildChanged(JSON.stringify(snap.val())," + this.name + @"_sender);      
                                }
                                else
                                {
                                    window.external.onChildChanged(snap.val()," + this.name + @"_sender);      
                                }                
                         });";
            // listener and event
            string onChildRemoved = "  " + this.name + @".on('child_removed', function(snap) {                            
                                if((typeof snap.val())=='object') {    
                                    window.external.onChildRemoved(JSON.stringify(snap.val())," + this.name + @"_sender);      
                                }
                                else
                                {
                                    window.external.onChildRemoved(snap.val()," + this.name + @"_sender);      
                                }                
                         });";




            //push event
            string toSringFunc = "function " + this.name + @"_toString(){
                                   return " + this.name + @".toString().substring(firebase.database().ref().toString().length-1)
                                 }";


            if (parents.Count == 0)
            {
                code += this.ToString() + "\n";
            }

            for (int i = 0; i < parents.Count; i++)
            {
                code += parents[i].ToString() + "\n";
            }

            if (parents.Count > 0)
            {
                code += this.ToString() + "\n";
            }
            //events
            code += onValue + "\n" + onChildAdded + "\n" + onChildChanged + "\n" + onChildRemoved + "\n";
            code += toSringFunc + "\n";

            //update
            Firebase.AddChild(this);
        }


        /// <summary>
        /// Childs the specified listen.
        /// </summary>
        /// <param name="object_">The object.</param>
        /// <returns>childRef.</returns>
        public firebaseRef child(string object_)
        {
            firebaseRef newRef = new firebaseRef(this, object_);
            return newRef;
        }

        /// <summary>
        /// Gets to string.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <value>To string.</value>
        public string ToString()
        {

            string declare;
            if (parents.Count == 0)
            {
                declare = "var " + this.name + "=firebase.database().ref();";
            }
            else
            {
                declare = "var " + this.name + "=" + parents[parents.Count - 1].name + ".child('" + this.childObject + "');";

            }
            return declare;

        }

        /// <summary>
        /// The random
        /// </summary>
        private static Random random = new Random();
        /// <summary>
        /// Generates the name.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public static string generateName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        /// <summary>
        /// Performs the on change.
        /// </summary>
        /// <param name="Value">The value.</param>
        public void performOnChange(string Value)
        {
            if (this.onValue != null)
            {
                this.Value = Value;
                this.onValue.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// Performs the on child added.
        /// </summary>
        /// <param name="Value">The value.</param>
        public void performOnChildAdded(string Value)
        {
            if (this.onChildAdded != null)
            {
                this.Value = Value;
                this.onChildAdded.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// Performs the on child changed.
        /// </summary>
        /// <param name="Value">The value.</param>
        public void performOnChildChanged(string Value)
        {
            if (this.onChildChanged != null)
            {
                this.Value = Value;
                this.onChildChanged.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Performs the on child removed.
        /// </summary>
        /// <param name="Value">The value.</param>
        public void performOnChildRemoved(string Value)
        {
            if (this.onChildRemoved != null)
            {
                this.Value = Value;
                this.onChildRemoved.Invoke(this, new EventArgs());
            }
        }


    }
}
