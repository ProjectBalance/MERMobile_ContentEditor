namespace Contentful.Importer.App.Interfaces
{
    partial class ContentTypeGrid
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grid = new System.Windows.Forms.DataGridView();
            this.btnUploadToGSheets = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnBulkUpload = new System.Windows.Forms.Button();
            this.openExcelFile = new System.Windows.Forms.OpenFileDialog();
            this.btnExportAsNew = new System.Windows.Forms.Button();
            this.saveExcelFile = new System.Windows.Forms.SaveFileDialog();
            this.Edit_col_btn_col = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete_col_btn_col = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Edit_col_btn_col,
            this.Delete_col_btn_col});
            this.grid.Location = new System.Drawing.Point(3, 32);
            this.grid.Name = "grid";
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(724, 487);
            this.grid.TabIndex = 0;
            this.grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellClick);
            // 
            // btnUploadToGSheets
            // 
            this.btnUploadToGSheets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUploadToGSheets.Location = new System.Drawing.Point(547, 3);
            this.btnUploadToGSheets.Name = "btnUploadToGSheets";
            this.btnUploadToGSheets.Size = new System.Drawing.Size(180, 23);
            this.btnUploadToGSheets.TabIndex = 1;
            this.btnUploadToGSheets.Text = "Export to existing file";
            this.btnUploadToGSheets.UseVisualStyleBackColor = true;
            this.btnUploadToGSheets.Click += new System.EventHandler(this.btnUploadToGSheets_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "+Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnBulkUpload
            // 
            this.btnBulkUpload.Location = new System.Drawing.Point(84, 3);
            this.btnBulkUpload.Name = "btnBulkUpload";
            this.btnBulkUpload.Size = new System.Drawing.Size(124, 23);
            this.btnBulkUpload.TabIndex = 3;
            this.btnBulkUpload.Text = "+Bulk Upload";
            this.btnBulkUpload.UseVisualStyleBackColor = true;
            this.btnBulkUpload.Click += new System.EventHandler(this.btnBulkUpload_Click);
            // 
            // openExcelFile
            // 
            this.openExcelFile.FileName = "openImportExcelFile";
            this.openExcelFile.Filter = "Excel Files|*.xlsx;";
            this.openExcelFile.Title = "Open Excel Import File";
            // 
            // btnExportAsNew
            // 
            this.btnExportAsNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportAsNew.Location = new System.Drawing.Point(361, 3);
            this.btnExportAsNew.Name = "btnExportAsNew";
            this.btnExportAsNew.Size = new System.Drawing.Size(180, 23);
            this.btnExportAsNew.TabIndex = 4;
            this.btnExportAsNew.Text = "Export as new File";
            this.btnExportAsNew.UseVisualStyleBackColor = true;
            this.btnExportAsNew.Click += new System.EventHandler(this.btnExportAsNew_Click);
            // 
            // saveExcelFile
            // 
            this.saveExcelFile.Filter = "Excel Files|*.xlsx;";
            // 
            // Edit_col_btn_col
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Edit_col_btn_col.DefaultCellStyle = dataGridViewCellStyle1;
            this.Edit_col_btn_col.HeaderText = "";
            this.Edit_col_btn_col.Name = "Edit_col_btn_col";
            this.Edit_col_btn_col.ReadOnly = true;
            // 
            // Delete_col_btn_col
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Delete_col_btn_col.DefaultCellStyle = dataGridViewCellStyle2;
            this.Delete_col_btn_col.HeaderText = "";
            this.Delete_col_btn_col.Name = "Delete_col_btn_col";
            this.Delete_col_btn_col.ReadOnly = true;
            // 
            // ContentTypeGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.btnExportAsNew);
            this.Controls.Add(this.btnBulkUpload);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnUploadToGSheets);
            this.Controls.Add(this.grid);
            this.Name = "ContentTypeGrid";
            this.Size = new System.Drawing.Size(730, 522);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Button btnUploadToGSheets;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnBulkUpload;
        private System.Windows.Forms.OpenFileDialog openExcelFile;
        private System.Windows.Forms.Button btnExportAsNew;
        private System.Windows.Forms.SaveFileDialog saveExcelFile;
        private System.Windows.Forms.DataGridViewButtonColumn Edit_col_btn_col;
        private System.Windows.Forms.DataGridViewButtonColumn Delete_col_btn_col;
    }
}
