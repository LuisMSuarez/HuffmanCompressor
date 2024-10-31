namespace HufmannCompressorWinFormsApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCompressFile = new Button();
            openFileDialog = new OpenFileDialog();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            btnInflateFile = new Button();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // btnCompressFile
            // 
            btnCompressFile.Location = new Point(38, 44);
            btnCompressFile.Margin = new Padding(2);
            btnCompressFile.Name = "btnCompressFile";
            btnCompressFile.Size = new Size(152, 36);
            btnCompressFile.TabIndex = 0;
            btnCompressFile.Text = "Compress file";
            btnCompressFile.UseVisualStyleBackColor = true;
            btnCompressFile.Click += btnCompressFile_Click;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(24, 24);
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripProgressBar1 });
            statusStrip.Location = new Point(0, 320);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(615, 32);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(60, 25);
            toolStripStatusLabel.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(200, 24);
            // 
            // btnInflateFile
            // 
            btnInflateFile.Location = new Point(222, 44);
            btnInflateFile.Margin = new Padding(2);
            btnInflateFile.Name = "btnInflateFile";
            btnInflateFile.Size = new Size(152, 36);
            btnInflateFile.TabIndex = 2;
            btnInflateFile.Text = "Inflate file";
            btnInflateFile.UseVisualStyleBackColor = true;
            btnInflateFile.Click += btnInflateFile_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 352);
            Controls.Add(btnInflateFile);
            Controls.Add(statusStrip);
            Controls.Add(btnCompressFile);
            Margin = new Padding(2);
            Name = "MainForm";
            Text = "Huffman Compressor";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCompressFile;
        private OpenFileDialog openFileDialog;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripProgressBar toolStripProgressBar1;
        private Button btnInflateFile;
    }
}

