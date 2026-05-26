using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class PanelAnaMenu : Panel
    {
        private Action<string> navigate;

        public PanelAnaMenu(Action<string> nav)
        {
            navigate = nav;
            BackColor = Color.FromArgb(15, 15, 15);
            Build();
        }

        private void Build()
        {
            this.Controls.Clear();
            var lblBaslik = new Label
            {
                Text = "ANA MENÜ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 40)
            };
            var lblHos = new Label
            {
                Text = "Hoş geldiniz!",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(15, 80)
            };

            var cards = new[]
            {
                ("🛒", "SİPARİŞ OLUŞTUR", "Yeni sipariş oluştur",   Color.FromArgb(183, 28, 28), "Siparis"),
                ("📋", "SİPARİŞLER",      "Tüm siparişleri görüntüle", Color.FromArgb(230, 81, 0), "Siparisler"),
                ("🍔", "MENÜ",            "Ürünleri görüntüle",     Color.FromArgb(27, 94, 32),  "Menu"),
                ("📊", "RAPORLAR",        "Satış raporları",        Color.FromArgb(13, 71, 161), "Raporlar"),
            };

            int kartGenislik = 250;
            int kartYukseklik = 180;
            int bosluk = 30;
           

            int x = 15;
            int y = 150;
            foreach (var (icon, title, sub, color, page) in cards)
            {
                if (page == "Raporlar" && Program.AktifRol != "Admin") continue;

                var card = new Panel
                {
                    Location = new Point(x, y),
                    Size = new Size(kartGenislik, kartYukseklik),
                    BackColor = Color.FromArgb(26, 26, 26),
                    Cursor = Cursors.Hand
                };
                card.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(new SolidBrush(color), 0, 0, card.Width, 5);
                };

                var lIcon = new Label
                {
                    Text = icon,
                    Font = new Font("Segoe UI Emoji", 34),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(kartGenislik, 80),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 20)
                };

                var lTitle = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 13, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(kartGenislik, 35),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 95)
                };

                var lSub = new Label
                {
                    Text = sub,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Gray,
                    AutoSize = false,
                    Size = new Size(kartGenislik, 25),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 125)
                };
                card.Controls.AddRange(new Control[] { lIcon, lTitle, lSub });
                var p = page;
                card.Click += (s, e) => navigate(p);
                lIcon.Click += (s, e) => navigate(p);
                lTitle.Click += (s, e) => navigate(p);
                lSub.Click += (s, e) => navigate(p);

                this.Controls.Add(card);
                x += kartGenislik + bosluk;
               
            }

            this.Controls.AddRange(new Control[] { lblBaslik, lblHos });
        }
    }
}
