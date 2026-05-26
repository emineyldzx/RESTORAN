using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class PanelRaporlar : Panel
    {
        private Label lblToplamSiparis, lblToplamKazanc, lblBugunSiparis, lblBugunKazanc;
        private DataGridView dgvTopUrunler;

        public PanelRaporlar()
        {
            BackColor = Color.FromArgb(15, 15, 15);
            Build();
            Yukle();
        }
        
        private void Build()
        {
            var lblBaslik = new Label
            {
                Text = "RAPORLAR",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Stat kartları
            lblToplamSiparis = new Label();
            lblToplamKazanc = new Label();
            lblBugunSiparis = new Label();
            lblBugunKazanc = new Label();


            int x = 10;
            Label[] lblRefs = new Label[4];
            string[] titles = { "Toplam Sipariş", "Toplam Kazanç", "Bugün Sipariş", "Bugün Kazanç" };

            for (int i = 0; i < 4; i++)
            {
                var card = new Panel { Location = new Point(x, 55), Size = new Size(200, 100), BackColor = Color.FromArgb(26,26,26) };
                var lTitle = new Label { Text = titles[i], ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 10), Font = new Font("Segoe UI", 8, FontStyle.Bold) };
                var lVal = new Label { Text = "...", ForeColor = Color.FromArgb(76,175,80), Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, Location = new Point(10, 35) };
                lblRefs[i] = lVal;
                card.Controls.AddRange(new Control[] { lTitle, lVal });
                this.Controls.Add(card);
                x += 215;
            }

            lblToplamSiparis = lblRefs[0];
            lblToplamKazanc  = lblRefs[1];
            lblBugunSiparis  = lblRefs[2];
            lblBugunKazanc   = lblRefs[3];

            // En çok satılan
            var pTop = new Panel { Location = new Point(10, 170), Size = new Size(860, 300), BackColor = Color.FromArgb(26,26,26) };
            var lTop = new Label { Text = "EN ÇOK SATILAN ÜRÜNLER", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 10), Font = new Font("Segoe UI", 8, FontStyle.Bold) };

            dgvTopUrunler = new DataGridView
            {
                Location = new Point(10, 35),
                Size = new Size(830, 240),
                BackgroundColor = Color.FromArgb(26,26,26),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(50,50,50),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvTopUrunler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36,36,36);
            dgvTopUrunler.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            dgvTopUrunler.DefaultCellStyle.BackColor = Color.FromArgb(26,26,26);
            dgvTopUrunler.DefaultCellStyle.ForeColor = Color.White;
            dgvTopUrunler.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50,50,50);
            dgvTopUrunler.Columns.Add("Urun", "Ürün Adı");
            dgvTopUrunler.Columns.Add("Adet", "Toplam Adet");
            dgvTopUrunler.Columns.Add("Gelir", "Toplam Gelir");

            pTop.Controls.AddRange(new Control[] { lTop, dgvTopUrunler });
            this.Controls.AddRange(new Control[] { lblBaslik, pTop });
        }

        public void Yukle()
        {
            if (lblToplamSiparis == null) return;
            // Toplamlar
            var total = Database.ExecuteQuery("SELECT COUNT(*) as C, SUM(ToplamTutar) as S FROM Siparisler");
            if (total.Rows.Count > 0)
            {
                lblToplamSiparis.Text = total.Rows[0]["C"].ToString();
            }
            else
            {
                lblToplamSiparis.Text = "0";
            }
            decimal totalKazanc = 0;

            if (total.Rows.Count > 0 && total.Rows[0]["S"] != DBNull.Value)
            {
                totalKazanc = Convert.ToDecimal(total.Rows[0]["S"]);
            }
            lblToplamKazanc.Text = totalKazanc.ToString("N2") + " ₺";

            string bugun = DateTime.Now.ToString("yyyy-MM-dd");
            var bugunData = Database.ExecuteQuery($"SELECT COUNT(*) as C, SUM(ToplamTutar) as S FROM Siparisler WHERE date(SiparisTarihi)='{bugun}'");
            if (bugunData.Rows.Count > 0)
            {
                lblBugunSiparis.Text = bugunData.Rows[0]["C"].ToString();
            }
            else
            {
                lblBugunSiparis.Text = "0";
            }

            decimal bugunKazanc = 0;

            if (bugunData.Rows.Count > 0 && bugunData.Rows[0]["S"] != DBNull.Value)
            {
                bugunKazanc = Convert.ToDecimal(bugunData.Rows[0]["S"]);
            }

            lblBugunKazanc.Text = bugunKazanc.ToString("N2") + " ₺";
            // Top ürünler
            dgvTopUrunler.Rows.Clear();
            var topUrunler = Database.ExecuteQuery(@"
                SELECT UrunAdi, SUM(Adet) as TotalAdet, SUM(ToplamFiyat) as TotalGelir
                FROM SiparisDetay GROUP BY UrunAdi ORDER BY TotalAdet DESC LIMIT 10");
            foreach (System.Data.DataRow row in topUrunler.Rows)
                dgvTopUrunler.Rows.Add(row["UrunAdi"], row["TotalAdet"], Convert.ToDecimal(row["TotalGelir"]).ToString("N2") + " ₺");
        }
    }
}
