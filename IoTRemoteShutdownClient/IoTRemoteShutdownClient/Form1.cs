using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace IoTRemoteShutdownClient
{
    public partial class Form1 : Form
    {
        string id = "";

        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            
        }

        private async void load() {
            id = Properties.Settings.Default.id;

            if (id.Equals(""))
            {
                var request = await client.PostAsync("https://us-central1-iotremoteshutdown.cloudfunctions.net/api/create", null);
                var strings = await request.Content.ReadAsStringAsync();

                dynamic obj = JObject.Parse(strings);

                id = (String)obj.response.id;
                Properties.Settings.Default["id"] = id;
                Properties.Settings.Default.Save();
            }

            txtID.Text = id;
            updater.Start();
        }

        private void Form1_LoadAsync(object sender, EventArgs e)
        {
            load();
        }

        private void shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams = mcWin32.GetMethodParameters("Win32Shutdown");

            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";

            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown", mboShutdownParams, null);
            }
        }

        private async void Updater_Tick(object sender, EventArgs e)
        {
            var strings = await client.GetStringAsync("https://us-central1-iotremoteshutdown.cloudfunctions.net/api?id=" + id);

            dynamic obj = JObject.Parse(strings);
            bool isShutdown = (Boolean)obj.response.isShutdown;

            if (isShutdown) {
                await client.PutAsync("https://us-central1-iotremoteshutdown.cloudfunctions.net/api/unshutdown?id=" + id, null);
                shutdown();
            }
        }
    }
}
