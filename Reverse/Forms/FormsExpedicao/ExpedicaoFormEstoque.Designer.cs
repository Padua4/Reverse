namespace Reverse.Forms.FormsExpedicao
{
    partial class ExpedicaoFormEstoque
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpedicaoFormEstoque));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPesoFiltro = new System.Windows.Forms.Label();
            this.btnMaterial = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnExcluirLinha = new System.Windows.Forms.Button();
            this.btnNovaLinha = new System.Windows.Forms.Button();
            this.btnExportarExcel = new System.Windows.Forms.Button();
            this.cmbAno = new System.Windows.Forms.ComboBox();
            this.cmbMes = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cmbMaterial = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.rbmPeso = new System.Windows.Forms.RadioButton();
            this.rbmQuantidade = new System.Windows.Forms.RadioButton();
            this.txtQuantidade = new System.Windows.Forms.TextBox();
            this.lblPesoQuantidade = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtObs = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.cmbCliente = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvMaterial = new ADGV.AdvancedDataGridView();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvMaterialFiltrado = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.lblVolumes = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.lblSegregado = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblTotal30 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel9.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterial)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterialFiltrado)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel1.Controls.Add(this.lblPesoFiltro);
            this.panel1.Controls.Add(this.btnMaterial);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1870, 40);
            this.panel1.TabIndex = 5;
            // 
            // lblPesoFiltro
            // 
            this.lblPesoFiltro.AutoSize = true;
            this.lblPesoFiltro.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPesoFiltro.Location = new System.Drawing.Point(280, 14);
            this.lblPesoFiltro.Name = "lblPesoFiltro";
            this.lblPesoFiltro.Size = new System.Drawing.Size(155, 21);
            this.lblPesoFiltro.TabIndex = 4;
            this.lblPesoFiltro.Text = "Peso Total Filtrado:";
            // 
            // btnMaterial
            // 
            this.btnMaterial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaterial.Location = new System.Drawing.Point(1665, 5);
            this.btnMaterial.Name = "btnMaterial";
            this.btnMaterial.Size = new System.Drawing.Size(193, 32);
            this.btnMaterial.TabIndex = 3;
            this.btnMaterial.TabStop = false;
            this.btnMaterial.Text = "Material";
            this.btnMaterial.UseVisualStyleBackColor = true;
            this.btnMaterial.Click += new System.EventHandler(this.btnMaterial_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Estoque de Material";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.65625F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.34375F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1870, 960);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel3);
            this.panel2.Controls.Add(this.cmbAno);
            this.panel2.Controls.Add(this.cmbMes);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1268, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(599, 954);
            this.panel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.btnSalvar, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnExcluirLinha, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnNovaLinha, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnExportarExcel, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 906);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(599, 48);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.ForeColor = System.Drawing.Color.Black;
            this.btnSalvar.Location = new System.Drawing.Point(450, 3);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(146, 42);
            this.btnSalvar.TabIndex = 13;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            // 
            // btnExcluirLinha
            // 
            this.btnExcluirLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExcluirLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirLinha.ForeColor = System.Drawing.Color.Black;
            this.btnExcluirLinha.Location = new System.Drawing.Point(152, 3);
            this.btnExcluirLinha.Name = "btnExcluirLinha";
            this.btnExcluirLinha.Size = new System.Drawing.Size(143, 42);
            this.btnExcluirLinha.TabIndex = 11;
            this.btnExcluirLinha.Text = "Excluir";
            this.btnExcluirLinha.UseVisualStyleBackColor = true;
            this.btnExcluirLinha.Click += new System.EventHandler(this.btnExcluirLinha_Click);
            // 
            // btnNovaLinha
            // 
            this.btnNovaLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNovaLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovaLinha.ForeColor = System.Drawing.Color.Black;
            this.btnNovaLinha.Location = new System.Drawing.Point(3, 3);
            this.btnNovaLinha.Name = "btnNovaLinha";
            this.btnNovaLinha.Size = new System.Drawing.Size(143, 42);
            this.btnNovaLinha.TabIndex = 10;
            this.btnNovaLinha.Text = "Novo";
            this.btnNovaLinha.UseVisualStyleBackColor = true;
            this.btnNovaLinha.Click += new System.EventHandler(this.btnNovaLinha_Click);
            // 
            // btnExportarExcel
            // 
            this.btnExportarExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarExcel.ForeColor = System.Drawing.Color.Black;
            this.btnExportarExcel.Location = new System.Drawing.Point(301, 3);
            this.btnExportarExcel.Name = "btnExportarExcel";
            this.btnExportarExcel.Size = new System.Drawing.Size(143, 42);
            this.btnExportarExcel.TabIndex = 12;
            this.btnExportarExcel.Text = "Excel";
            this.btnExportarExcel.UseVisualStyleBackColor = true;
            // 
            // cmbAno
            // 
            this.cmbAno.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAno.FormattingEnabled = true;
            this.cmbAno.Location = new System.Drawing.Point(64, 106);
            this.cmbAno.Name = "cmbAno";
            this.cmbAno.Size = new System.Drawing.Size(543, 28);
            this.cmbAno.TabIndex = 2;
            // 
            // cmbMes
            // 
            this.cmbMes.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMes.FormattingEnabled = true;
            this.cmbMes.Location = new System.Drawing.Point(64, 72);
            this.cmbMes.Name = "cmbMes";
            this.cmbMes.Size = new System.Drawing.Size(543, 28);
            this.cmbMes.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(4, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 25);
            this.label7.TabIndex = 4;
            this.label7.Text = "Ano:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(4, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 25);
            this.label5.TabIndex = 3;
            this.label5.Text = "Mês:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(251, 32);
            this.label3.TabIndex = 2;
            this.label3.Text = "Selecione o mês/ano";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.36601F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.63399F));
            this.tableLayoutPanel2.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblPesoQuantidade, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel10, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.panel3, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.panel9, 1, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 159);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(612, 603);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.cmbMaterial);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(145, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(464, 114);
            this.panel6.TabIndex = 1;
            // 
            // cmbMaterial
            // 
            this.cmbMaterial.FormattingEnabled = true;
            this.cmbMaterial.Location = new System.Drawing.Point(3, 48);
            this.cmbMaterial.Name = "cmbMaterial";
            this.cmbMaterial.Size = new System.Drawing.Size(456, 33);
            this.cmbMaterial.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 120);
            this.label6.TabIndex = 0;
            this.label6.Text = "Material:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.rbmPeso);
            this.panel7.Controls.Add(this.rbmQuantidade);
            this.panel7.Controls.Add(this.txtQuantidade);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(145, 123);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(464, 114);
            this.panel7.TabIndex = 2;
            // 
            // rbmPeso
            // 
            this.rbmPeso.AutoSize = true;
            this.rbmPeso.ForeColor = System.Drawing.Color.Black;
            this.rbmPeso.Location = new System.Drawing.Point(114, 13);
            this.rbmPeso.Name = "rbmPeso";
            this.rbmPeso.Size = new System.Drawing.Size(71, 29);
            this.rbmPeso.TabIndex = 5;
            this.rbmPeso.TabStop = true;
            this.rbmPeso.Text = "Peso";
            this.rbmPeso.UseVisualStyleBackColor = true;
            // 
            // rbmQuantidade
            // 
            this.rbmQuantidade.AutoSize = true;
            this.rbmQuantidade.ForeColor = System.Drawing.Color.Black;
            this.rbmQuantidade.Location = new System.Drawing.Point(3, 13);
            this.rbmQuantidade.Name = "rbmQuantidade";
            this.rbmQuantidade.Size = new System.Drawing.Size(105, 29);
            this.rbmQuantidade.TabIndex = 4;
            this.rbmQuantidade.TabStop = true;
            this.rbmQuantidade.Text = "Unidade";
            this.rbmQuantidade.UseVisualStyleBackColor = true;
            // 
            // txtQuantidade
            // 
            this.txtQuantidade.Location = new System.Drawing.Point(2, 48);
            this.txtQuantidade.Name = "txtQuantidade";
            this.txtQuantidade.Size = new System.Drawing.Size(458, 33);
            this.txtQuantidade.TabIndex = 6;
            // 
            // lblPesoQuantidade
            // 
            this.lblPesoQuantidade.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.lblPesoQuantidade.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPesoQuantidade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPesoQuantidade.Location = new System.Drawing.Point(3, 120);
            this.lblPesoQuantidade.Name = "lblPesoQuantidade";
            this.lblPesoQuantidade.Size = new System.Drawing.Size(136, 120);
            this.lblPesoQuantidade.TabIndex = 2;
            this.lblPesoQuantidade.Text = "Unidade:";
            this.lblPesoQuantidade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.cmbStatus);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(145, 243);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(464, 114);
            this.panel10.TabIndex = 3;
            // 
            // cmbStatus
            // 
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(3, 48);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(460, 33);
            this.cmbStatus.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(3, 240);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(136, 120);
            this.label10.TabIndex = 3;
            this.label10.Text = "Status:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 360);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(136, 120);
            this.label11.TabIndex = 4;
            this.label11.Text = "Cliente:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.txtObs);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(145, 483);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(464, 117);
            this.panel3.TabIndex = 5;
            // 
            // txtObs
            // 
            this.txtObs.Location = new System.Drawing.Point(3, 49);
            this.txtObs.Name = "txtObs";
            this.txtObs.Size = new System.Drawing.Size(458, 33);
            this.txtObs.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(3, 480);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(136, 123);
            this.label9.TabIndex = 5;
            this.label9.Text = "Observação:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.cmbCliente);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(145, 363);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(464, 114);
            this.panel9.TabIndex = 4;
            // 
            // cmbCliente
            // 
            this.cmbCliente.FormattingEnabled = true;
            this.cmbCliente.Location = new System.Drawing.Point(4, 52);
            this.cmbCliente.Name = "cmbCliente";
            this.cmbCliente.Size = new System.Drawing.Size(458, 33);
            this.cmbCliente.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.dgvMaterial, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1259, 954);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // dgvMaterial
            // 
            this.dgvMaterial.AutoGenerateContextFilters = true;
            this.dgvMaterial.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.dgvMaterial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaterial.DateWithTime = false;
            this.dgvMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMaterial.Location = new System.Drawing.Point(3, 3);
            this.dgvMaterial.Name = "dgvMaterial";
            this.dgvMaterial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMaterial.Size = new System.Drawing.Size(1253, 471);
            this.dgvMaterial.TabIndex = 1;
            this.dgvMaterial.TabStop = false;
            this.dgvMaterial.TimeFilter = false;
            this.dgvMaterial.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMaterial_CellFormatting);
            this.dgvMaterial.SelectionChanged += new System.EventHandler(this.dgvMaterial_SelectionChanged);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 89F));
            this.tableLayoutPanel5.Controls.Add(this.dgvMaterialFiltrado, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 480);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1253, 471);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // dgvMaterialFiltrado
            // 
            this.dgvMaterialFiltrado.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.dgvMaterialFiltrado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaterialFiltrado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMaterialFiltrado.Location = new System.Drawing.Point(140, 3);
            this.dgvMaterialFiltrado.Name = "dgvMaterialFiltrado";
            this.dgvMaterialFiltrado.Size = new System.Drawing.Size(1110, 465);
            this.dgvMaterialFiltrado.TabIndex = 3;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.panel13, 0, 5);
            this.tableLayoutPanel6.Controls.Add(this.panel12, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.panel11, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.panel8, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.panel5, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(131, 465);
            this.tableLayoutPanel6.TabIndex = 4;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.lblVolumes);
            this.panel13.Location = new System.Drawing.Point(3, 388);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(125, 70);
            this.panel13.TabIndex = 5;
            // 
            // lblVolumes
            // 
            this.lblVolumes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVolumes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVolumes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolumes.ForeColor = System.Drawing.Color.Black;
            this.lblVolumes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblVolumes.Location = new System.Drawing.Point(0, 0);
            this.lblVolumes.Name = "lblVolumes";
            this.lblVolumes.Size = new System.Drawing.Size(125, 70);
            this.lblVolumes.TabIndex = 1;
            this.lblVolumes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel12.Controls.Add(this.label13);
            this.panel12.Location = new System.Drawing.Point(3, 311);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(125, 70);
            this.panel12.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label13.Location = new System.Drawing.Point(0, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(125, 70);
            this.label13.TabIndex = 1;
            this.label13.Text = "Volumes";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.lblSegregado);
            this.panel11.Location = new System.Drawing.Point(3, 234);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(125, 70);
            this.panel11.TabIndex = 3;
            // 
            // lblSegregado
            // 
            this.lblSegregado.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSegregado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSegregado.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSegregado.ForeColor = System.Drawing.Color.Black;
            this.lblSegregado.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSegregado.Location = new System.Drawing.Point(0, 0);
            this.lblSegregado.Name = "lblSegregado";
            this.lblSegregado.Size = new System.Drawing.Size(125, 70);
            this.lblSegregado.TabIndex = 1;
            this.lblSegregado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel8.Controls.Add(this.label8);
            this.panel8.Location = new System.Drawing.Point(3, 157);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(125, 70);
            this.panel8.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 70);
            this.label8.TabIndex = 1;
            this.label8.Text = "Total Segregado";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblTotal30);
            this.panel5.Location = new System.Drawing.Point(3, 80);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(125, 70);
            this.panel5.TabIndex = 1;
            // 
            // lblTotal30
            // 
            this.lblTotal30.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotal30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotal30.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal30.ForeColor = System.Drawing.Color.Black;
            this.lblTotal30.Location = new System.Drawing.Point(0, 0);
            this.lblTotal30.Name = "lblTotal30";
            this.lblTotal30.Size = new System.Drawing.Size(125, 70);
            this.lblTotal30.TabIndex = 1;
            this.lblTotal30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(125, 70);
            this.panel4.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 70);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total em 30 dias";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExpedicaoFormEstoque
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1870, 1000);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExpedicaoFormEstoque";
            this.Text = "FormEstoque";
            this.Load += new System.EventHandler(this.FormEstoque_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterial)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterialFiltrado)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ADGV.AdvancedDataGridView dgvMaterial;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbAno;
        private System.Windows.Forms.ComboBox cmbMes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.TextBox txtObs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnNovaLinha;
        private System.Windows.Forms.Button btnExcluirLinha;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox txtQuantidade;
        private System.Windows.Forms.Label lblPesoQuantidade;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbCliente;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ComboBox cmbMaterial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbmQuantidade;
        private System.Windows.Forms.RadioButton rbmPeso;
        private System.Windows.Forms.Button btnMaterial;
        private System.Windows.Forms.Label lblPesoFiltro;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridView dgvMaterialFiltrado;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Label lblVolumes;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label lblSegregado;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblTotal30;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnExportarExcel;
    }
}