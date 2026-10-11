namespace ClipboardInputer_via_DeepSeek_v_SentKeys;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox logOutput;

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
        this.logOutput = new System.Windows.Forms.TextBox();
        this.SuspendLayout();

        // logOutput
        this.logOutput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.logOutput.Multiline = true;
        this.logOutput.ReadOnly = true;
        this.logOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.logOutput.Font = new System.Drawing.Font("Consolas", 10F);
        this.logOutput.Name = "logOutput";

        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 361);
        this.Controls.Add(this.logOutput);
        this.Name = "Form1";
        this.Text = "Clipboard Typer";
        this.ResumeLayout(false);
    }

    #endregion
}