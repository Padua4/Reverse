namespace Reverse
{
    partial class TriagemForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelecionarPalete = new System.Windows.Forms.Button();
            this.chkUltimosPrimeiro = new System.Windows.Forms.CheckBox();
            this.btnCriarPalete = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnEditarItem = new System.Windows.Forms.Button();
            this.btnNovoItem = new System.Windows.Forms.Button();
            this.btnExportarPDF = new System.Windows.Forms.Button();
            this.btnExportarExcel = new System.Windows.Forms.Button();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAdicionarItem = new System.Windows.Forms.Button();
            this.btnAtualizarItem = new System.Windows.Forms.Button();
            this.btnRemoverItem = new System.Windows.Forms.Button();
            this.lblTotalPalete = new System.Windows.Forms.Label();
            this.btnFinalizado = new System.Windows.Forms.Button();
            this.dgvItensPalete = new System.Windows.Forms.DataGridView();
            this.lblPaleteAtual = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItensPalete)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.panelHeader.Controls.Add(this.label1);
            this.panelHeader.Controls.Add(this.btnSelecionarPalete);
            this.panelHeader.Controls.Add(this.chkUltimosPrimeiro);
            this.panelHeader.Controls.Add(this.btnCriarPalete);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1920, 56);
            this.panelHeader.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 37);
            this.label1.TabIndex = 8;
            this.label1.Text = "Triagem";
            // 
            // btnSelecionarPalete
            // 
            this.btnSelecionarPalete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelecionarPalete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelecionarPalete.ForeColor = System.Drawing.Color.White;
            this.btnSelecionarPalete.Location = new System.Drawing.Point(304, 5);
            this.btnSelecionarPalete.Name = "btnSelecionarPalete";
            this.btnSelecionarPalete.Size = new System.Drawing.Size(158, 45);
            this.btnSelecionarPalete.TabIndex = 7;
            this.btnSelecionarPalete.Text = "Selecionar Palete";
            this.btnSelecionarPalete.UseVisualStyleBackColor = true;
            this.btnSelecionarPalete.Click += new System.EventHandler(this.btnSelecionarPalete_Click);
            // 
            // chkUltimosPrimeiro
            // 
            this.chkUltimosPrimeiro.AutoSize = true;
            this.chkUltimosPrimeiro.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.chkUltimosPrimeiro.ForeColor = System.Drawing.Color.White;
            this.chkUltimosPrimeiro.Location = new System.Drawing.Point(478, 13);
            this.chkUltimosPrimeiro.Name = "chkUltimosPrimeiro";
            this.chkUltimosPrimeiro.Size = new System.Drawing.Size(203, 29);
            this.chkUltimosPrimeiro.TabIndex = 6;
            this.chkUltimosPrimeiro.Text = "Ordem de Cadastro";
            this.chkUltimosPrimeiro.UseVisualStyleBackColor = true;
            this.chkUltimosPrimeiro.Click += new System.EventHandler(this.chkUltimosPrimeiro_CheckedChanged);
            // 
            // btnCriarPalete
            // 
            this.btnCriarPalete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCriarPalete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCriarPalete.ForeColor = System.Drawing.Color.White;
            this.btnCriarPalete.Location = new System.Drawing.Point(140, 5);
            this.btnCriarPalete.Name = "btnCriarPalete";
            this.btnCriarPalete.Size = new System.Drawing.Size(158, 45);
            this.btnCriarPalete.TabIndex = 1;
            this.btnCriarPalete.Text = "Criar Palete";
            this.btnCriarPalete.UseVisualStyleBackColor = true;
            this.btnCriarPalete.Click += new System.EventHandler(this.btnCriarPalete_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.txtBusca);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 54);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnEditarItem, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnNovoItem, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExportarPDF, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExportarExcel, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(720, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1200, 54);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btnEditarItem
            // 
            this.btnEditarItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditarItem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnEditarItem.ForeColor = System.Drawing.Color.White;
            this.btnEditarItem.Location = new System.Drawing.Point(243, 3);
            this.btnEditarItem.Name = "btnEditarItem";
            this.btnEditarItem.Size = new System.Drawing.Size(234, 48);
            this.btnEditarItem.TabIndex = 4;
            this.btnEditarItem.Text = "Editar Produto";
            this.btnEditarItem.UseVisualStyleBackColor = true;
            this.btnEditarItem.Click += new System.EventHandler(this.btnEditarItem_Click);
            // 
            // btnNovoItem
            // 
            this.btnNovoItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovoItem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnNovoItem.ForeColor = System.Drawing.Color.White;
            this.btnNovoItem.Location = new System.Drawing.Point(3, 3);
            this.btnNovoItem.Name = "btnNovoItem";
            this.btnNovoItem.Size = new System.Drawing.Size(234, 48);
            this.btnNovoItem.TabIndex = 3;
            this.btnNovoItem.Text = "Cadastrar Produto";
            this.btnNovoItem.UseVisualStyleBackColor = true;
            this.btnNovoItem.Click += new System.EventHandler(this.btnNovoItem_Click);
            // 
            // btnExportarPDF
            // 
            this.btnExportarPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarPDF.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnExportarPDF.ForeColor = System.Drawing.Color.White;
            this.btnExportarPDF.Location = new System.Drawing.Point(963, 3);
            this.btnExportarPDF.Name = "btnExportarPDF";
            this.btnExportarPDF.Size = new System.Drawing.Size(234, 48);
            this.btnExportarPDF.TabIndex = 5;
            this.btnExportarPDF.Text = "Exportar PDF";
            this.btnExportarPDF.UseVisualStyleBackColor = true;
            this.btnExportarPDF.Click += new System.EventHandler(this.btnExportarPDF_Click);
            // 
            // btnExportarExcel
            // 
            this.btnExportarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarExcel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnExportarExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportarExcel.Location = new System.Drawing.Point(723, 3);
            this.btnExportarExcel.Name = "btnExportarExcel";
            this.btnExportarExcel.Size = new System.Drawing.Size(234, 48);
            this.btnExportarExcel.TabIndex = 6;
            this.btnExportarExcel.Text = "Exportar Excel";
            this.btnExportarExcel.UseVisualStyleBackColor = true;
            this.btnExportarExcel.Click += new System.EventHandler(this.btnExportarExcel_Click);
            // 
            // txtBusca
            // 
            this.txtBusca.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBusca.Location = new System.Drawing.Point(12, 12);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(697, 33);
            this.txtBusca.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.dgvProdutos, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 110);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1920, 856);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdutos.Location = new System.Drawing.Point(3, 3);
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.Size = new System.Drawing.Size(762, 850);
            this.dgvProdutos.TabIndex = 1;
            this.dgvProdutos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvProdutos_CellFormatting);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.dgvItensPalete, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblPaleteAtual, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(771, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.909953F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.98104F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.109005F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1146, 850);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 5;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Controls.Add(this.btnAdicionarItem, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnAtualizarItem, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnRemoverItem, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblTotalPalete, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnFinalizado, 3, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 792);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1140, 55);
            this.tableLayoutPanel4.TabIndex = 6;
            // 
            // btnAdicionarItem
            // 
            this.btnAdicionarItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdicionarItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionarItem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnAdicionarItem.ForeColor = System.Drawing.Color.White;
            this.btnAdicionarItem.Location = new System.Drawing.Point(3, 3);
            this.btnAdicionarItem.Name = "btnAdicionarItem";
            this.btnAdicionarItem.Size = new System.Drawing.Size(222, 49);
            this.btnAdicionarItem.TabIndex = 0;
            this.btnAdicionarItem.Text = "Adicionar →";
            this.btnAdicionarItem.UseVisualStyleBackColor = true;
            this.btnAdicionarItem.Click += new System.EventHandler(this.btnAdicionarItem_Click);
            // 
            // btnAtualizarItem
            // 
            this.btnAtualizarItem.AutoSize = true;
            this.btnAtualizarItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAtualizarItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtualizarItem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnAtualizarItem.ForeColor = System.Drawing.Color.White;
            this.btnAtualizarItem.Location = new System.Drawing.Point(459, 3);
            this.btnAtualizarItem.Name = "btnAtualizarItem";
            this.btnAtualizarItem.Size = new System.Drawing.Size(222, 49);
            this.btnAtualizarItem.TabIndex = 2;
            this.btnAtualizarItem.Text = "Atualizar Qtde/Valor";
            this.btnAtualizarItem.UseVisualStyleBackColor = true;
            this.btnAtualizarItem.Click += new System.EventHandler(this.btnAtualizarItem_Click);
            // 
            // btnRemoverItem
            // 
            this.btnRemoverItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoverItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoverItem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnRemoverItem.ForeColor = System.Drawing.Color.White;
            this.btnRemoverItem.Location = new System.Drawing.Point(231, 3);
            this.btnRemoverItem.Name = "btnRemoverItem";
            this.btnRemoverItem.Size = new System.Drawing.Size(222, 49);
            this.btnRemoverItem.TabIndex = 1;
            this.btnRemoverItem.Text = "← Remover";
            this.btnRemoverItem.UseVisualStyleBackColor = true;
            this.btnRemoverItem.Click += new System.EventHandler(this.btnRemoverItem_Click);
            // 
            // lblTotalPalete
            // 
            this.lblTotalPalete.AutoSize = true;
            this.lblTotalPalete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalPalete.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPalete.ForeColor = System.Drawing.Color.White;
            this.lblTotalPalete.Location = new System.Drawing.Point(915, 0);
            this.lblTotalPalete.Name = "lblTotalPalete";
            this.lblTotalPalete.Size = new System.Drawing.Size(222, 55);
            this.lblTotalPalete.TabIndex = 3;
            this.lblTotalPalete.Text = "Total";
            this.lblTotalPalete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFinalizado
            // 
            this.btnFinalizado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFinalizado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinalizado.ForeColor = System.Drawing.Color.GreenYellow;
            this.btnFinalizado.Location = new System.Drawing.Point(687, 3);
            this.btnFinalizado.Name = "btnFinalizado";
            this.btnFinalizado.Size = new System.Drawing.Size(222, 49);
            this.btnFinalizado.TabIndex = 4;
            this.btnFinalizado.Text = "Finalizar";
            this.btnFinalizado.UseVisualStyleBackColor = true;
            this.btnFinalizado.Click += new System.EventHandler(this.btnFinalizado_Click);
            // 
            // dgvItensPalete
            // 
            this.dgvItensPalete.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.dgvItensPalete.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItensPalete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItensPalete.Location = new System.Drawing.Point(3, 36);
            this.dgvItensPalete.Name = "dgvItensPalete";
            this.dgvItensPalete.Size = new System.Drawing.Size(1140, 750);
            this.dgvItensPalete.TabIndex = 5;
            this.dgvItensPalete.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvItensPalete_CellFormatting);
            // 
            // lblPaleteAtual
            // 
            this.lblPaleteAtual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.lblPaleteAtual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPaleteAtual.Font = new System.Drawing.Font("Segoe UI", 18.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaleteAtual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.lblPaleteAtual.Location = new System.Drawing.Point(3, 0);
            this.lblPaleteAtual.Name = "lblPaleteAtual";
            this.lblPaleteAtual.Size = new System.Drawing.Size(1140, 33);
            this.lblPaleteAtual.TabIndex = 4;
            this.lblPaleteAtual.Text = "PALETE ---";
            this.lblPaleteAtual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormTriagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1920, 966);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormTriagem";
            this.Text = "FormTriagem";
            this.Load += new System.EventHandler(this.TriagemForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItensPalete)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Button btnSelecionarPalete;
        private System.Windows.Forms.CheckBox chkUltimosPrimeiro;
        private System.Windows.Forms.Button btnCriarPalete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnExportarExcel;
        private System.Windows.Forms.Button btnEditarItem;
        private System.Windows.Forms.Button btnExportarPDF;
        private System.Windows.Forms.Button btnNovoItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblPaleteAtual;
        private System.Windows.Forms.DataGridView dgvItensPalete;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblTotalPalete;
        private System.Windows.Forms.Button btnAdicionarItem;
        private System.Windows.Forms.Button btnAtualizarItem;
        private System.Windows.Forms.Button btnRemoverItem;
        private System.Windows.Forms.Button btnFinalizado;
    }
}