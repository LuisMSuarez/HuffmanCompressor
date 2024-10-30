namespace HufmannCompressorWinFormsApp
{
    using HuffmanCompressorLib;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnCompressFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "All files (*.*)|*.*";
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

        private void btnInflateFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Compressed files (*.huf)|*.huf";
            openFileDialog.AddExtension = true;
            var openFileDialogResult = openFileDialog.ShowDialog(this);
            if (openFileDialogResult != DialogResult.OK)
            {
                return;
            }

            var compressor = new HuffmanCompressor();
            toolStripStatusLabel.Text = "Inflating...";
            compressor.Inflate(openFileDialog.FileName, openFileDialog.FileName + ".inflated");
            toolStripStatusLabel.Text = "Ready";
        }
    }
}