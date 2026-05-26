using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace RestoranSiparisSistemi
{
    public class PanelSiparis : Panel
    {
        private ComboBox cmbMasa, cmbUrun;
        private NumericUpDown nudAdet;
        private Label lblFiyat, lblToplam;
        private DataGridView dgvSepet;
        private List<SepetItem> sepet = new List<SepetItem>();

        public PanelSiparis()
        {
            BackColor = Color.FromArgb(15, 15, 15);
            Build();
            AutoScroll = true;
        }

        private void Build()
        {
            var lblBaslik = new Label
            {
                Text = "SİPARİŞ OLUŞTUR",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 20)
            };

            // -- MASA PANELİ --
            var pMasa = CreatePanel("Masa Seçimi", 40, 55, 220, 380);
            var lMasa = new Label { Text = "Masa No", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 35), Font = new Font("Segoe UI", 8) };
            cmbMasa = new ComboBox { Location = new Point(10, 55), Size = new Size(180, 28), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            for (int i = 1; i <= 15; i++) cmbMasa.Items.Add(i);
            pMasa.Controls.AddRange(new Control[] { lMasa, cmbMasa });

            // -- ÜRÜN PANELİ --
            var pUrun = CreatePanel("Ürün Seçimi", 300, 55, 300, 380);
            var lUrun = new Label { Text = "Ürün", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 35), Font = new Font("Segoe UI", 8) };
            cmbUrun = new ComboBox { Location = new Point(10, 53), Size = new Size(240, 28), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            cmbUrun.SelectedIndexChanged += (s, e) => GuncelleFiyat();

            var lAdet = new Label { Text = "Adet", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 95), Font = new Font("Segoe UI", 8) };
            nudAdet = new NumericUpDown { Location = new Point(10, 113), Size = new Size(240, 28), Minimum = 1, Maximum = 99, Value = 1, BackColor = Color.FromArgb(36,36,36), ForeColor = Color.White };
            nudAdet.ValueChanged += (s, e) => GuncelleFiyat();

            var lFiyat = new Label { Text = "Fiyat", ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 265), Font = new Font("Segoe UI", 8) };
            lblFiyat = new Label { Text = "0,00 ₺", ForeColor = Color.FromArgb(76,175,80), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(10, 280) };

            var btnEkle = new Button
            {
                Text = "Sepete Ekle",
                Location = new Point(10, 320),
                Size = new Size(240, 36),
                BackColor = Color.FromArgb(229,57,53),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                AutoSize = false
            };
            btnEkle.FlatAppearance.BorderSize = 0;
            btnEkle.Click += BtnEkle_Click;
            pUrun.Controls.AddRange(new Control[] { lUrun, cmbUrun, lAdet, nudAdet, lFiyat, lblFiyat, btnEkle });

            // -- SEPET PANELİ --
            var pSepet = CreatePanel("Sipariş Detayı", 640, 55, 420, 380);
            dgvSepet = new DataGridView
            {
                Location = new Point(10, 30),
                Size = new Size(350, 220),
                BackgroundColor = Color.FromArgb(26,26,26),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(50,50,50),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvSepet.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36,36,36);
            dgvSepet.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            dgvSepet.DefaultCellStyle.BackColor = Color.FromArgb(26,26,26);
            dgvSepet.DefaultCellStyle.ForeColor = Color.White;
            dgvSepet.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50,50,50);
            dgvSepet.Columns.Add(new DataGridViewTextBoxColumn { Name = "Urun", HeaderText = "Ürün", Width = 130 });
            dgvSepet.Columns.Add(new DataGridViewTextBoxColumn { Name = "Adet", HeaderText = "Adet", Width = 50 });
            dgvSepet.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fiyat", HeaderText = "Fiyat", Width = 80 });
            dgvSepet.Columns.Add(new DataGridViewTextBoxColumn { Name = "Toplam", HeaderText = "Toplam", Width = 85 });

            lblToplam = new Label { Text = "Toplam: 0,00 ₺", ForeColor = Color.FromArgb(76,175,80), Font = new Font("Segoe UI", 13, FontStyle.Bold), AutoSize = true, Location = new Point(10, 260) };

            var btnKaydet = new Button
            {
                Text = "Siparişi Kaydet",
                Location = new Point(10, 300),
                Size = new Size(350, 38),
                BackColor = Color.FromArgb(27,94,32),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnKaydet.FlatAppearance.BorderSize = 0;
            btnKaydet.Click += BtnKaydet_Click;

            var btnTemizle = new Button
            {
                Text = "Temizle",
                Location = new Point(10, 345),
                Size = new Size(350, 30),
                BackColor = Color.FromArgb(40,40,40),
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnTemizle.FlatAppearance.BorderColor = Color.FromArgb(60,60,60);
            btnTemizle.Click += (s, e) => TemizleSepet();

            pSepet.Controls.AddRange(new Control[] { dgvSepet, lblToplam, btnKaydet, btnTemizle });

            this.Controls.AddRange(new Control[] { lblBaslik, pMasa, pUrun, pSepet });
        }

        public void Yukle()
        {
            cmbUrun.Items.Clear();
            var dt = Database.ExecuteQuery("SELECT UrunID, UrunAdi, Fiyat FROM Urunler ORDER BY Kategori, UrunAdi");
            foreach (System.Data.DataRow row in dt.Rows)
                cmbUrun.Items.Add(new UrunItem { ID = Convert.ToInt32(row["UrunID"]), Ad = row["UrunAdi"].ToString(), Fiyat = Convert.ToDecimal(row["Fiyat"]) });
            if (cmbUrun.Items.Count > 0) cmbUrun.SelectedIndex = 0;
            GuncelleFiyat();
        }

        private void GuncelleFiyat()
        {
            if (cmbUrun.SelectedItem is UrunItem u)
                lblFiyat.Text = (u.Fiyat * (decimal)nudAdet.Value).ToString("N2") + " ₺";
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (cmbMasa.SelectedIndex < 0) { MessageBox.Show("Lütfen masa seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!(cmbUrun.SelectedItem is UrunItem u)) { MessageBox.Show("Lütfen ürün seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int adet = (int)nudAdet.Value;
            var mevcut = sepet.Find(x => x.UrunID == u.ID);
            if (mevcut != null) { mevcut.Adet += adet; mevcut.Toplam = mevcut.Fiyat * mevcut.Adet; }
            else sepet.Add(new SepetItem { UrunID = u.ID, UrunAdi = u.Ad, Fiyat = u.Fiyat, Adet = adet, Toplam = u.Fiyat * adet });

            RenderSepet();
            nudAdet.Value = 1;
        }

        private void RenderSepet()
        {
            dgvSepet.Rows.Clear();
            decimal toplam = 0;
            foreach (var item in sepet)
            {
                dgvSepet.Rows.Add(item.UrunAdi, item.Adet, item.Fiyat.ToString("N2") + " ₺", item.Toplam.ToString("N2") + " ₺");
                toplam += item.Toplam;
            }
            lblToplam.Text = "Toplam: " + toplam.ToString("N2") + " ₺";
        }

        private void TemizleSepet() { sepet.Clear(); RenderSepet(); }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbMasa.SelectedIndex < 0) { MessageBox.Show("Lütfen masa seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (sepet.Count == 0) { MessageBox.Show("Sepet boş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int masaNo = (int)cmbMasa.SelectedItem;
            decimal toplam = 0;
            foreach (var i in sepet) toplam += i.Toplam;

            string sqlSiparis = "INSERT INTO Siparisler (MasaNo, ToplamTutar, Durum) VALUES (@m, @t, 'Hazırlanıyor'); SELECT last_insert_rowid();";
            long siparisID = Convert.ToInt64(Database.ExecuteScalar(sqlSiparis, new[] {
                new SQLiteParameter("@m", masaNo),
                new SQLiteParameter("@t", toplam)
            }));

            foreach (var item in sepet)
            {
                Database.ExecuteNonQuery(
                    "INSERT INTO SiparisDetay (SiparisID, UrunAdi, Adet, BirimFiyat, ToplamFiyat) VALUES (@s,@u,@a,@b,@t)",
                    new[] {
                        new SQLiteParameter("@s", siparisID),
                        new SQLiteParameter("@u", item.UrunAdi),
                        new SQLiteParameter("@a", item.Adet),
                        new SQLiteParameter("@b", item.Fiyat),
                        new SQLiteParameter("@t", item.Toplam)
                    });
            }

            MessageBox.Show($"Sipariş #{siparisID} kaydedildi!\nToplam: {toplam:N2} ₺", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TemizleSepet();
            cmbMasa.SelectedIndex = -1;
        }

        private Panel CreatePanel(string baslik, int x, int y, int w, int h)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.FromArgb(26,26,26) };
            var lbl = new Label { Text = baslik, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(10, 10) };
            p.Controls.Add(lbl);
            this.Controls.Add(p);
            return p;
        }
    }

    class UrunItem { public int ID; public string Ad; public decimal Fiyat; public override string ToString() => Ad; }
    class SepetItem { public int UrunID; public string UrunAdi; public decimal Fiyat; public int Adet; public decimal Toplam; }
}
