namespace Reverse
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblGreeting = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblGreetingg = new System.Windows.Forms.Label();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.flpModules = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTriagem = new System.Windows.Forms.Panel();
            this.lblDescTriagem = new System.Windows.Forms.Label();
            this.picTriagem = new System.Windows.Forms.PictureBox();
            this.pnlAlmoxarifado = new System.Windows.Forms.Panel();
            this.lblDescAlmo = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pnlRH = new System.Windows.Forms.Panel();
            this.lblDescRH = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pnlFin = new System.Windows.Forms.Panel();
            this.lblDescFin = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pnlHeader.SuspendLayout();
            this.flpModules.SuspendLayout();
            this.pnlTriagem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).BeginInit();
            this.pnlAlmoxarifado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlRH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.pnlFin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGreeting
            // 
            this.lblGreeting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.Location = new System.Drawing.Point(-154, 9);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(0, 24);
            this.lblGreeting.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.pnlHeader.Controls.Add(this.lblGreetingg);
            this.pnlHeader.Controls.Add(this.lblSeparator);
            this.pnlHeader.Controls.Add(this.lblGreeting);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1311, 51);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblGreetingg
            // 
            this.lblGreetingg.AutoSize = true;
            this.lblGreetingg.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblGreetingg.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblGreetingg.ForeColor = System.Drawing.Color.White;
            this.lblGreetingg.Location = new System.Drawing.Point(0, 0);
            this.lblGreetingg.Name = "lblGreetingg";
            this.lblGreetingg.Size = new System.Drawing.Size(145, 24);
            this.lblGreetingg.TabIndex = 3;
            this.lblGreetingg.Text = "Texto Modulo";
            // 
            // lblSeparator
            // 
            this.lblSeparator.AutoSize = true;
            this.lblSeparator.BackColor = System.Drawing.Color.Gray;
            this.lblSeparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 2.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeparator.ForeColor = System.Drawing.Color.Gray;
            this.lblSeparator.Location = new System.Drawing.Point(-1, 43);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(1962, 4);
            this.lblSeparator.TabIndex = 2;
            this.lblSeparator.Text = resources.GetString("lblSeparator.Text");
            // 
            // flpModules
            // 
            this.flpModules.AutoScroll = true;
            this.flpModules.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(247)))), ((int)(((byte)(217)))));
            this.flpModules.Controls.Add(this.pnlTriagem);
            this.flpModules.Controls.Add(this.pnlAlmoxarifado);
            this.flpModules.Controls.Add(this.pnlRH);
            this.flpModules.Controls.Add(this.pnlFin);
            this.flpModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpModules.Location = new System.Drawing.Point(0, 51);
            this.flpModules.Name = "flpModules";
            this.flpModules.Padding = new System.Windows.Forms.Padding(20);
            this.flpModules.Size = new System.Drawing.Size(1311, 610);
            this.flpModules.TabIndex = 2;
            // 
            // pnlTriagem
            // 
            this.pnlTriagem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(247)))), ((int)(((byte)(217)))));
            this.pnlTriagem.Controls.Add(this.lblDescTriagem);
            this.pnlTriagem.Controls.Add(this.picTriagem);
            this.pnlTriagem.Location = new System.Drawing.Point(23, 23);
            this.pnlTriagem.Name = "pnlTriagem";
            this.pnlTriagem.Size = new System.Drawing.Size(310, 575);
            this.pnlTriagem.TabIndex = 0;
            this.pnlTriagem.MouseEnter += new System.EventHandler(this.pnlTriagem_MouseEnter);
            this.pnlTriagem.MouseLeave += new System.EventHandler(this.pnlTriagem_MouseLeave);
            // 
            // lblDescTriagem
            // 
            this.lblDescTriagem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescTriagem.Location = new System.Drawing.Point(14, 498);
            this.lblDescTriagem.Name = "lblDescTriagem";
            this.lblDescTriagem.Size = new System.Drawing.Size(280, 60);
            this.lblDescTriagem.TabIndex = 1;
            this.lblDescTriagem.Text = "Área responsável por avaliar e separar itens";
            this.lblDescTriagem.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDescTriagem.Visible = false;
            // 
            // picTriagem
            // 
            this.picTriagem.Image = global::Reverse.Properties.Resources._20250723_1629_Setor_Triagem_Ilustrado_remix_01k0wc2njmf7creq2p7yhvj108;
            this.picTriagem.Location = new System.Drawing.Point(0, 0);
            this.picTriagem.Name = "picTriagem";
            this.picTriagem.Size = new System.Drawing.Size(310, 480);
            this.picTriagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTriagem.TabIndex = 0;
            this.picTriagem.TabStop = false;
            this.picTriagem.Click += new System.EventHandler(this.picTriagem_Click);
            this.picTriagem.MouseEnter += new System.EventHandler(this.pnlTriagem_MouseEnter);
            this.picTriagem.MouseLeave += new System.EventHandler(this.pnlTriagem_MouseLeave);
            // 
            // pnlAlmoxarifado
            // 
            this.pnlAlmoxarifado.Controls.Add(this.lblDescAlmo);
            this.pnlAlmoxarifado.Controls.Add(this.pictureBox2);
            this.pnlAlmoxarifado.Location = new System.Drawing.Point(339, 23);
            this.pnlAlmoxarifado.Name = "pnlAlmoxarifado";
            this.pnlAlmoxarifado.Size = new System.Drawing.Size(310, 575);
            this.pnlAlmoxarifado.TabIndex = 1;
            this.pnlAlmoxarifado.MouseEnter += new System.EventHandler(this.pnlAlmoxarifado_MouseEnter);
            this.pnlAlmoxarifado.MouseLeave += new System.EventHandler(this.pnlAlmoxarifado_MouseLeave);
            // 
            // lblDescAlmo
            // 
            this.lblDescAlmo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescAlmo.Location = new System.Drawing.Point(15, 498);
            this.lblDescAlmo.Name = "lblDescAlmo";
            this.lblDescAlmo.Size = new System.Drawing.Size(280, 60);
            this.lblDescAlmo.TabIndex = 2;
            this.lblDescAlmo.Text = "Área responsável por controlar, armazenar e distribuir materiais e insumos da emp" +
    "resa";
            this.lblDescAlmo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDescAlmo.Visible = false;
            this.lblDescAlmo.Click += new System.EventHandler(this.lblDescAlmo_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Reverse.Properties.Resources._20250723_1633_Setor_Almoxarifado_Ilustrado_remix_01k0wc9z41fa78c8mteezcz80d;
            this.pictureBox2.Location = new System.Drawing.Point(3, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(310, 480);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseEnter += new System.EventHandler(this.pnlAlmoxarifado_MouseEnter);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.pnlAlmoxarifado_MouseLeave);
            // 
            // pnlRH
            // 
            this.pnlRH.Controls.Add(this.lblDescRH);
            this.pnlRH.Controls.Add(this.pictureBox3);
            this.pnlRH.Location = new System.Drawing.Point(655, 23);
            this.pnlRH.Name = "pnlRH";
            this.pnlRH.Size = new System.Drawing.Size(310, 575);
            this.pnlRH.TabIndex = 2;
            this.pnlRH.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.pnlRH.MouseEnter += new System.EventHandler(this.pnlRH_MouseEnter);
            this.pnlRH.MouseLeave += new System.EventHandler(this.pnlRH_MouseLeave);
            // 
            // lblDescRH
            // 
            this.lblDescRH.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescRH.Location = new System.Drawing.Point(12, 498);
            this.lblDescRH.Name = "lblDescRH";
            this.lblDescRH.Size = new System.Drawing.Size(280, 60);
            this.lblDescRH.TabIndex = 2;
            this.lblDescRH.Text = "Departamento encarregado da gestão de pessoas, recrutamento, treinamentos e benef" +
    "ícios";
            this.lblDescRH.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDescRH.Visible = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Reverse.Properties.Resources._20250723_1637_Setor_Recursos_Humanos_remix_01k0wcgqs4ezvb7vrd27147a2n;
            this.pictureBox3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(310, 480);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            this.pictureBox3.MouseEnter += new System.EventHandler(this.pnlRH_MouseEnter);
            this.pictureBox3.MouseLeave += new System.EventHandler(this.pnlRH_MouseLeave);
            // 
            // pnlFin
            // 
            this.pnlFin.Controls.Add(this.lblDescFin);
            this.pnlFin.Controls.Add(this.pictureBox4);
            this.pnlFin.Location = new System.Drawing.Point(971, 23);
            this.pnlFin.Name = "pnlFin";
            this.pnlFin.Size = new System.Drawing.Size(310, 575);
            this.pnlFin.TabIndex = 3;
            this.pnlFin.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFin_Paint);
            this.pnlFin.MouseEnter += new System.EventHandler(this.pnlFin_MouseEnter);
            this.pnlFin.MouseLeave += new System.EventHandler(this.pnlFin_MouseLeave);
            // 
            // lblDescFin
            // 
            this.lblDescFin.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescFin.Location = new System.Drawing.Point(15, 498);
            this.lblDescFin.Name = "lblDescFin";
            this.lblDescFin.Size = new System.Drawing.Size(280, 60);
            this.lblDescFin.TabIndex = 2;
            this.lblDescFin.Text = "Setor responsável pelo planejamento orçamentário, fluxo de caixa, contas a pagar " +
    "e receber\r\n";
            this.lblDescFin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDescFin.Visible = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::Reverse.Properties.Resources._20250724_1051_Setor_Financeiro_Ilustrado_remix_01k0yb3tk5fwss77c2m3yc19kk;
            this.pictureBox4.Location = new System.Drawing.Point(0, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(310, 480);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.MouseEnter += new System.EventHandler(this.pnlFin_MouseEnter);
            this.pictureBox4.MouseLeave += new System.EventHandler(this.pnlFin_MouseLeave);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 661);
            this.Controls.Add(this.flpModules);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hub";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.flpModules.ResumeLayout(false);
            this.pnlTriagem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTriagem)).EndInit();
            this.pnlAlmoxarifado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.pnlRH.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.pnlFin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.FlowLayoutPanel flpModules;
        private System.Windows.Forms.Panel pnlTriagem;
        private System.Windows.Forms.PictureBox picTriagem;
        private System.Windows.Forms.Panel pnlAlmoxarifado;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel pnlRH;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label lblGreetingg;
        private System.Windows.Forms.Panel pnlFin;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label lblDescTriagem;
        private System.Windows.Forms.Label lblDescAlmo;
        private System.Windows.Forms.Label lblDescRH;
        private System.Windows.Forms.Label lblDescFin;
    }
}