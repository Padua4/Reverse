using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reverse
{
    public partial class TriagemForm : Form
    {
        public TriagemForm()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;
            Load += TriagemForm_Load;
            Resize += TriagemForm_Resize;
        }

        private void TriagemForm_Load(object sender, EventArgs e)
        {
            AjustarSplitter();
            InitializeLegend();
        }

        private void TriagemForm_Resize(object sender, EventArgs e)
        {
            AjustarSplitter();
        }

        private void AjustarSplitter()
        {
            splitContainerMain.SplitterDistance = splitContainerMain.Width / 2;
        }

        private void InitializeLegend()
        {
            flpLegend.Controls.Clear();
            flpLegend.Controls.Add(CreateLegendItem(Color.Gold, "Mercado Livre"));
            flpLegend.Controls.Add(CreateLegendItem(Color.DodgerBlue, "Amazon"));
            flpLegend.Controls.Add(CreateLegendItem(Color.LightGray, "Sites variados"));
            flpLegend.Controls.Add(CreateLegendItem(Color.Black, "Importado"));
            flpLegend.Controls.Add(CreateLegendItem(Color.White, ">1 mês sem alteração"));
        }

        private Control CreateLegendItem(Color circleColor, string text)
        {
            const int diameter = 12;
            const int spacing = 4;
            var font = new Font("Segoe UI", 9);
            var textSize = TextRenderer.MeasureText(text, font);
            int height = Math.Max(diameter, textSize.Height);
            int width = diameter + spacing + textSize.Width;

            var panel = new Panel
            {
                Width = width,
                Height = height,
                Margin = new Padding(5, 3, 5, 3),
                BackColor = Color.Transparent
            };

            // desenha o círculo colorido
            var bmp = new Bitmap(diameter, diameter);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(circleColor))
                    g.FillEllipse(brush, 0, 0, diameter - 1, diameter - 1);

                if (circleColor.GetBrightness() > 0.9f)
                    using (var pen = new Pen(Color.Gray, 1))
                        g.DrawEllipse(pen, 0, 0, diameter - 1, diameter - 1);
            }

            var pic = new PictureBox
            {
                Image = bmp,
                Size = new Size(diameter, diameter),
                Location = new Point(0, (height - diameter) / 2),
                BackColor = Color.Transparent
            };

            var lbl = new Label
            {
                Text = text,
                Font = font,
                AutoSize = true,
                Location = new Point(diameter + spacing, (height - textSize.Height) / 2),
                BackColor = Color.Transparent
            };

            panel.Controls.Add(pic);
            panel.Controls.Add(lbl);
            return panel;
        }

        private void flpLegend_Paint(object sender, PaintEventArgs e)
        {
            // pintura customizada desabilitada
        }
    }
}
