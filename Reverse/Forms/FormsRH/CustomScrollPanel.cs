using System;
using System.Drawing;
using System.Windows.Forms;

public class CustomScrollPanel : Panel
{
    public int ScrollBarSize { get; set; } = 8;           // Largura/altura da barra
    public Color ScrollBarColor { get; set; } = Color.DimGray;
    public Color ScrollBarHoverColor { get; set; } = Color.Gray;

    private bool _hoverVertical;
    private bool _hoverHorizontal;

    public CustomScrollPanel()
    {
        this.DoubleBuffered = true;
        this.AutoScroll = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Desenha a barra vertical se necessário
        if (this.DisplayRectangle.Height > this.ClientSize.Height)
        {
            Rectangle vertBar = new Rectangle(
                this.ClientSize.Width - ScrollBarSize,
                0,
                ScrollBarSize,
                this.ClientSize.Height
            );

            using (SolidBrush brush = new SolidBrush(_hoverVertical ? ScrollBarHoverColor : ScrollBarColor))
                e.Graphics.FillRectangle(brush, vertBar);
        }

        // Desenha a barra horizontal se necessário
        if (this.DisplayRectangle.Width > this.ClientSize.Width)
        {
            Rectangle horBar = new Rectangle(
                0,
                this.ClientSize.Height - ScrollBarSize,
                this.ClientSize.Width,
                ScrollBarSize
            );

            using (SolidBrush brush = new SolidBrush(_hoverHorizontal ? ScrollBarHoverColor : ScrollBarColor))
                e.Graphics.FillRectangle(brush, horBar);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        _hoverVertical = e.X >= this.ClientSize.Width - ScrollBarSize;
        _hoverHorizontal = e.Y >= this.ClientSize.Height - ScrollBarSize;

        Invalidate(); // repinta para mudar a cor se estiver sobre a barra
    }
}