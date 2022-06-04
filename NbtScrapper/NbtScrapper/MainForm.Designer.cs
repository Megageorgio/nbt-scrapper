namespace NbtScrapper
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
            this.inputPathTextBox = new System.Windows.Forms.TextBox();
            this.inputOpenButton = new System.Windows.Forms.Button();
            this.extractButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.outputPathTextBox = new System.Windows.Forms.TextBox();
            this.outputOpenButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // inputPathTextBox
            // 
            this.inputPathTextBox.Location = new System.Drawing.Point(95, 37);
            this.inputPathTextBox.Name = "inputPathTextBox";
            this.inputPathTextBox.Size = new System.Drawing.Size(368, 27);
            this.inputPathTextBox.TabIndex = 0;
            // 
            // inputOpenButton
            // 
            this.inputOpenButton.Location = new System.Drawing.Point(478, 37);
            this.inputOpenButton.Name = "inputOpenButton";
            this.inputOpenButton.Size = new System.Drawing.Size(94, 29);
            this.inputOpenButton.TabIndex = 1;
            this.inputOpenButton.Text = "Open...";
            this.inputOpenButton.UseVisualStyleBackColor = true;
            this.inputOpenButton.Click += new System.EventHandler(this.inputOpenButton_Click);
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(361, 220);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(113, 38);
            this.extractButton.TabIndex = 2;
            this.extractButton.Text = "Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(95, 145);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(477, 32);
            this.progressBar.TabIndex = 3;
            // 
            // outputPathTextBox
            // 
            this.outputPathTextBox.Location = new System.Drawing.Point(95, 92);
            this.outputPathTextBox.Name = "outputPathTextBox";
            this.outputPathTextBox.Size = new System.Drawing.Size(368, 27);
            this.outputPathTextBox.TabIndex = 4;
            // 
            // outputOpenButton
            // 
            this.outputOpenButton.Location = new System.Drawing.Point(478, 90);
            this.outputOpenButton.Name = "outputOpenButton";
            this.outputOpenButton.Size = new System.Drawing.Size(94, 29);
            this.outputOpenButton.TabIndex = 5;
            this.outputOpenButton.Text = "Open...";
            this.outputOpenButton.UseVisualStyleBackColor = true;
            this.outputOpenButton.Click += new System.EventHandler(this.outputOpenButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Map";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Output";
            // 
            // infoTextBox
            // 
            this.infoTextBox.Location = new System.Drawing.Point(600, 37);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.Size = new System.Drawing.Size(185, 140);
            this.infoTextBox.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 270);
            this.Controls.Add(this.infoTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputOpenButton);
            this.Controls.Add(this.outputPathTextBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.inputOpenButton);
            this.Controls.Add(this.inputPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "NBT Scrapper by Megageorgio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox inputPathTextBox;
        private Button inputOpenButton;
        private Button extractButton;
        private ProgressBar progressBar;
        private TextBox outputPathTextBox;
        private Button outputOpenButton;
        private Label label1;
        private Label label2;
        private TextBox infoTextBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}