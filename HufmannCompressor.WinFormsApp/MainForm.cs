namespace HufmannCompressor.WinFormsApp;
using HuffmanCompressor.Lib;

public partial class MainForm : Form
{
    private readonly IFileCompressor compressor;
    public MainForm(IFileCompressor compressor)
    {
        this.compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
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

        toolStripStatusLabel.Text = "Compressing...";
        this.compressor.Compress(openFileDialog.FileName, openFileDialog.FileName + ".huf");
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

        toolStripStatusLabel.Text = "Inflating...";
        this.compressor.Inflate(openFileDialog.FileName, openFileDialog.FileName + ".inflated");
        toolStripStatusLabel.Text = "Ready";
    }
}