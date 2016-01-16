namespace OpenCvKinectV2
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxColorFrame = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxColorFrame
            // 
            this.pictureBoxColorFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxColorFrame.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxColorFrame.Name = "pictureBoxColorFrame";
            this.pictureBoxColorFrame.Size = new System.Drawing.Size(624, 321);
            this.pictureBoxColorFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxColorFrame.TabIndex = 0;
            this.pictureBoxColorFrame.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 321);
            this.Controls.Add(this.pictureBoxColorFrame);
            this.Name = "Form1";
            this.Text = "OpenCvSharp3 & Kinect v2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorFrame)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxColorFrame;
    }
}

