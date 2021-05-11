
namespace ExamplesRunner {
    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.calcPage = new System.Windows.Forms.TabPage();
            this.calcResultBox = new System.Windows.Forms.TextBox();
            this.calcInputBox = new System.Windows.Forms.TextBox();
            this.btnCalcSolve = new System.Windows.Forms.Button();
            this.coloringPage = new System.Windows.Forms.TabPage();
            this.codeColoringBox = new System.Windows.Forms.RichTextBox();
            this.colorLangBox = new System.Windows.Forms.ComboBox();
            this.btnColorExample = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.calcPage.SuspendLayout();
            this.coloringPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.calcPage);
            this.tabControl.Controls.Add(this.coloringPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 450);
            this.tabControl.TabIndex = 0;
            // 
            // calcPage
            // 
            this.calcPage.Controls.Add(this.calcResultBox);
            this.calcPage.Controls.Add(this.calcInputBox);
            this.calcPage.Controls.Add(this.btnCalcSolve);
            this.calcPage.Location = new System.Drawing.Point(4, 24);
            this.calcPage.Name = "calcPage";
            this.calcPage.Padding = new System.Windows.Forms.Padding(3);
            this.calcPage.Size = new System.Drawing.Size(792, 422);
            this.calcPage.TabIndex = 0;
            this.calcPage.Text = "Calculator";
            this.calcPage.UseVisualStyleBackColor = true;
            // 
            // calcResultBox
            // 
            this.calcResultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calcResultBox.Location = new System.Drawing.Point(8, 32);
            this.calcResultBox.Multiline = true;
            this.calcResultBox.Name = "calcResultBox";
            this.calcResultBox.ReadOnly = true;
            this.calcResultBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.calcResultBox.Size = new System.Drawing.Size(776, 382);
            this.calcResultBox.TabIndex = 2;
            this.calcResultBox.WordWrap = false;
            // 
            // calcInputBox
            // 
            this.calcInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calcInputBox.Location = new System.Drawing.Point(8, 3);
            this.calcInputBox.Name = "calcInputBox";
            this.calcInputBox.Size = new System.Drawing.Size(695, 23);
            this.calcInputBox.TabIndex = 1;
            this.calcInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.calcInputBox_KeyDown);
            // 
            // btnCalcSolve
            // 
            this.btnCalcSolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcSolve.Location = new System.Drawing.Point(709, 3);
            this.btnCalcSolve.Name = "btnCalcSolve";
            this.btnCalcSolve.Size = new System.Drawing.Size(75, 23);
            this.btnCalcSolve.TabIndex = 0;
            this.btnCalcSolve.Text = "Solve";
            this.btnCalcSolve.UseVisualStyleBackColor = true;
            this.btnCalcSolve.Click += new System.EventHandler(this.btnCalcSolve_Click);
            // 
            // coloringPage
            // 
            this.coloringPage.Controls.Add(this.codeColoringBox);
            this.coloringPage.Controls.Add(this.colorLangBox);
            this.coloringPage.Controls.Add(this.btnColorExample);
            this.coloringPage.Location = new System.Drawing.Point(4, 24);
            this.coloringPage.Name = "coloringPage";
            this.coloringPage.Padding = new System.Windows.Forms.Padding(3);
            this.coloringPage.Size = new System.Drawing.Size(792, 422);
            this.coloringPage.TabIndex = 1;
            this.coloringPage.Text = "Coloring";
            this.coloringPage.UseVisualStyleBackColor = true;
            // 
            // codeColoringBox
            // 
            this.codeColoringBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeColoringBox.Location = new System.Drawing.Point(8, 33);
            this.codeColoringBox.Name = "codeColoringBox";
            this.codeColoringBox.Size = new System.Drawing.Size(776, 381);
            this.codeColoringBox.TabIndex = 3;
            this.codeColoringBox.Text = "";
            this.codeColoringBox.TextChanged += new System.EventHandler(this.codeColoringBox_TextChanged);
            // 
            // colorLangBox
            // 
            this.colorLangBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorLangBox.FormattingEnabled = true;
            this.colorLangBox.Location = new System.Drawing.Point(8, 4);
            this.colorLangBox.Name = "colorLangBox";
            this.colorLangBox.Size = new System.Drawing.Size(695, 23);
            this.colorLangBox.TabIndex = 2;
            this.colorLangBox.SelectedIndexChanged += new System.EventHandler(this.colorLangBox_SelectedIndexChanged);
            // 
            // btnColorExample
            // 
            this.btnColorExample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorExample.Location = new System.Drawing.Point(709, 3);
            this.btnColorExample.Name = "btnColorExample";
            this.btnColorExample.Size = new System.Drawing.Size(75, 23);
            this.btnColorExample.TabIndex = 1;
            this.btnColorExample.Text = "Example";
            this.btnColorExample.UseVisualStyleBackColor = true;
            this.btnColorExample.Click += new System.EventHandler(this.btnColorExample_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl);
            this.Name = "MainForm";
            this.Text = "Petite Parser Examples";
            this.tabControl.ResumeLayout(false);
            this.calcPage.ResumeLayout(false);
            this.calcPage.PerformLayout();
            this.coloringPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage calcPage;
        private System.Windows.Forms.TabPage coloringPage;
        private System.Windows.Forms.TextBox calcResultBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnCalcSolve;
        private System.Windows.Forms.TextBox calcInputBox;
        private System.Windows.Forms.Button btnColorExample;
        private System.Windows.Forms.ComboBox colorLangBox;
        private System.Windows.Forms.RichTextBox codeColoringBox;
    }
}

