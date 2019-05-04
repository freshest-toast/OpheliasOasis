using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace OpheliasOasis
{
    public partial class GenerateIncentiveReport : Form
    {
        private TextBox output;

        public GenerateIncentiveReport()
        {
            // Initialize the output window (to display if the save was successful)
            InitializeComponent();

            // open a save dialog box to user, and let user choose where they are saving the document
            SaveFileDialog dialogWindow = new SaveFileDialog();
            dialogWindow.FileName = "Incentive Report";
            dialogWindow.Filter = "Comma Separated Values (.csv)|*.csv";
            dialogWindow.Title = "Save Report As";
            var cancelClick = dialogWindow.ShowDialog();

            // If the user clicks cancel button, then return
            if (cancelClick == DialogResult.Cancel)
            {
                return;
            }

            // else, we save the report to the location the user specified
            string filePath = dialogWindow.FileName;

            // Get the data from the table
            DataTable table = DatabaseManager.getIncentiveReport();

            // Build the string in such a way that it will be converted into a .csv file
            StringBuilder input = new StringBuilder();

            // Put the column names first so that they are at the top of the table (in the .csv file)
            string[] colNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            input.AppendLine(string.Join(",", colNames));

            // Insert the values from the DataTable into the table in the .csv file
            foreach (DataRow row in table.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                input.AppendLine(string.Join(",", fields));
            }

            // save file to that location
            File.WriteAllText(filePath, input.ToString());

            // set the window name to Successful so we know that the file was saved
            this.Name = "Successful";
        }

        private void InitializeComponent()
        {
            this.output = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(1, 84);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(300, 20);
            this.output.TabIndex = 0;
            this.output.Text = "Incentive Report Saved Successfully";
            // 
            // GenerateIncentiveReport
            // 
            this.ClientSize = new System.Drawing.Size(300, 200);
            this.Controls.Add(this.output);
            this.Name = "GenerateIncentiveReport";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
