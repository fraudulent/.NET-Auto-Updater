using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization; /* Reference System.Web.Extensions */
using System.Windows.Forms;

public class AutoUpdater
{
    private class Properties
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public string ChangeLog { get; set; }
        public bool IsClosed { get; set; }
        public string ClosedMessage { get; set; }
    }

    private static string MediaFireDirectLink(string mediaFirelink)
    {
        string content = new WebClient().DownloadString(mediaFirelink);
        return content.Contains("mediafire") ? Regex.Match(content, @"http://download.*.mediafire.com.*(.[\w])").Value : content;
    }

    private static void Close()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    private static DialogResult Message(object msg, MessageBoxIcon icon, bool ques = false)
    {
        return MessageBox.Show(msg.ToString(), "Message", ques ? MessageBoxButtons.YesNo : MessageBoxButtons.OK, icon);
    }

    /// <summary>
    /// Checks For Updates With Message Box
    /// </summary>
    /// <param name="currentVersion">Current Application Version</param>
    /// <param name="jsonContent">The url that contains your application details as JSON</param>
    /// <returns>Return Current Version</returns>
    public static decimal CheckForUpdate(decimal currentVersion, string jsonContent)
    {
        try
        {
            using (WebClient wb = new WebClient())
            {
                wb.Encoding = Encoding.UTF8;
                Properties prop = new JavaScriptSerializer().Deserialize<Properties>(wb.DownloadString(jsonContent));
                if (prop.IsClosed)
                {
                    Message(prop.ClosedMessage, MessageBoxIcon.Error);
                    Close();
                }
                if ((decimal.Parse(prop.Version) > currentVersion))
                {
                    if (Message("New Update Available!\nCurrent Version: " + currentVersion + "\nNew Version: " + prop.Version, MessageBoxIcon.Information, true) == DialogResult.Yes)
                    {
                        using (SaveFileDialog sf = new SaveFileDialog() { Filter = "Rar File |*.rar", Title = prop.Version })
                        {
                            if (sf.ShowDialog() == DialogResult.OK)
                            {
                                wb.DownloadFile(MediaFireDirectLink(prop.DownloadUrl), sf.FileName);
                                Message("File Has Been Downloaded!", MessageBoxIcon.Asterisk);
                                Close();
                            }
                            else
                            {
                                Close();
                            }
                        }
                    }
                    else
                    {
                        Message("You have to use the new Version: " + prop.Version, MessageBoxIcon.Error);
                        Close();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Message(ex.Message, MessageBoxIcon.Error);
            Close();
        }
        return currentVersion;
    }
}