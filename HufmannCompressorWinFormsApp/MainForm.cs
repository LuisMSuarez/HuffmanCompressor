namespace HufmannCompressorWinFormsApp
{
    using HuffmanCompressorLib;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            var openFileDialogResult = openFileDialog.ShowDialog(this);
            if (openFileDialogResult != DialogResult.OK)
            {
                return;
            }

            var compressor = new HuffmanCompressor();
            toolStripStatusLabel.Text = "Compressing...";
            compressor.Compress(openFileDialog.FileName, openFileDialog.FileName + ".huf");
            toolStripStatusLabel.Text = "Ready";
        }
    }
}