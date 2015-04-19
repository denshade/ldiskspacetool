using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;


namespace WindowsFormsApplication1
{

    
    
    public partial class FileSystemView : Form
    {
        private System.ComponentModel.BackgroundWorker backgroundWorker;

        private DuFileSystemCrawler crawler;

        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitializeBackgoundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
           
            crawler = new DuFileSystemCrawler(worker);
            e.Result = crawler.getFilesystemFileDictionary(folderName.Text);
            
        }

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker1_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                textBox1.Text = "Canceled";
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                String build = "";
                foreach (var item in (Dictionary<string, long>)e.Result)
                {
                    build += item.Value + "," + item.Key + Environment.NewLine;
                }

                textBox1.Text = build;
            }

            // Enable the Start button.
            scanButton.Enabled = true;
            saveButton.Enabled = true;
            openViewerButton.Enabled = true;
            scanButton.Text = "Scan";
            progressBar1.Value = 0;


            // Disable the Cancel button.
            //cancelAsyncButton.Enabled = false;
        }

        // This event handler updates the progress bar.
        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= progressBar1.Maximum)
            {
                progressBar1.Value = e.ProgressPercentage;
            }            
        }

        public FileSystemView()
        {
            InitializeBackgoundWorker();
            InitializeComponent();
        }


        private void browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName.Text = dialog.SelectedPath;
            }
        }

        private void scan_Click(object sender, EventArgs e)
        {
            
            string folderpath = folderName.Text;

            if (!Directory.Exists(folderpath))
            {
                MessageBox.Show(folderpath + " doesn'this exist");
                return;
            }
            scanButton.Enabled = false;
            scanButton.Text = "Running";
            saveButton.Enabled = false;
            openViewerButton.Enabled = false;
            long used = 0;
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                //There are more attributes you can use.
                //Check the MSDN link for a complete example.


                if (folderName.Text.Contains(drive.RootDirectory.FullName))
                {
                    used = drive.TotalSize - drive.AvailableFreeSpace;
                }
                if (drive.IsReady) Console.WriteLine(drive.TotalSize);
            }
            progressBar1.Maximum = (int)(used / 1000);

            backgroundWorker.RunWorkerAsync();
        }

        private void setText(string text)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = "results.csv";
            DialogResult result = this.saveFileDialog.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, textBox1.Text, Encoding.ASCII);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string tempPath = System.IO.Path.GetTempPath();
            try
            {
                System.IO.File.WriteAllText(tempPath + "\\results.csv", textBox1.Text, Encoding.ASCII);
                System.Diagnostics.Process.Start(@tempPath + "\\results.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

    }

   
}
