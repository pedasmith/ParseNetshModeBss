using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using NetshG.Properties;
using Utilities.ConfigurableParser;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        string filename = "NetshG_UserPreferences.json";

        private void Log(string str)
        {
            Console.WriteLine(str);
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Restore application-scope property from isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Open, storage))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var pref = JsonConvert.DeserializeObject<UserPreferences>(json);
                    if (pref != null)
                    {
                        pref.ReplaceTabs = false; // Never persists
                        UP.CurrUserPrefs = pref;
                    }
                }
            }
            catch (Exception ex)
            {
                ; // do nothing
                Log($"Note: Init: no usable user preferences file found: {ex.Message}");
                // Handle when file is not found in isolated storage:
                // * When the first application session
                // * When file has been deleted
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // Persist application-scope property to isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Create, storage))
            {
                var settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };
                var json = JsonConvert.SerializeObject(UP.CurrUserPrefs, settings);
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
                // Persist each application-scope property individually
                //foreach (string key in this.Properties.Keys)
                //{
                //    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                //}
        }
    }
}
