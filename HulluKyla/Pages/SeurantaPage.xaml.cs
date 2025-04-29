using HulluKyla.Models;
using HulluKyla.Services;

namespace HulluKyla.Pages {
    public partial class SeurantaPage : ContentPage {
        private List<Lasku> kaikkiLaskut = new();
        private List<Asiakas> kaikkiAsiakkaat = new();
        private List<Asiakas> suodatetutAsiakkaat = new();
        private Dictionary<uint, uint> varausAsiakasMap = new();
        private bool naytaVainMaksamattomat = true;

        public SeurantaPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            LataaData();
        }

        private void LataaData() {
            // Luo laskut tarvittaessa
            LaskuService.LuoLaskutPaattyneille();

            // Haetaan data
            kaikkiLaskut = LaskuService.HaeKaikki();
            kaikkiAsiakkaat = AsiakasService.HaeKaikki();
            suodatetutAsiakkaat = kaikkiAsiakkaat;

            // Asiakaslista
            AsiakasListaView.ItemsSource = suodatetutAsiakkaat;

            // Varausten asiakas-ID:t
            var varaukset = VarausService.HaeKaikki();
            varausAsiakasMap.Clear();
            foreach (var varaus in varaukset) {
                varausAsiakasMap[varaus.VarausId] = varaus.Asiakas.AsiakasId;
            }

            // N�yt� aluksi maksamattomat
            PaivitaNakyma(null);
        }

        private void AsiakasSearchBar_TextChanged(object sender, TextChangedEventArgs e) {
            string haku = AsiakasSearchBar.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(haku)) {
                suodatetutAsiakkaat = kaikkiAsiakkaat;
                AsiakasListaView.SelectedItem = null;
                PaivitaNakyma(null);
            } else {
                suodatetutAsiakkaat = kaikkiAsiakkaat
                    .Where(a => a.KokoNimi.ToLower().Contains(haku))
                    .ToList();

                AsiakasListaView.SelectedItem = null;
            }

            AsiakasListaView.ItemsSource = suodatetutAsiakkaat;
        }

        private void AsiakasListaView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.CurrentSelection.FirstOrDefault() is Asiakas valittu) {
                PaivitaNakyma(valittu);
            }
        }

        private void VaihdaNakyma_Clicked(object sender, EventArgs e) {
            naytaVainMaksamattomat = !naytaVainMaksamattomat;
            VaihdaNakymaButton.Text = naytaVainMaksamattomat
                ? "Maksamattomat laskut"
                : "Kaikki laskut";

            Asiakas valittu = AsiakasListaView.SelectedItem as Asiakas;
            PaivitaNakyma(valittu);
        }

        private void PaivitaNakyma(Asiakas valittuAsiakas) {
            IEnumerable<Lasku> suodatettu = kaikkiLaskut;

            if (valittuAsiakas != null) {
                suodatettu = suodatettu.Where(l =>
                    varausAsiakasMap.TryGetValue(l.VarausId, out uint asiakasId) &&
                    asiakasId == valittuAsiakas.AsiakasId);
            }

            if (naytaVainMaksamattomat) {
                suodatettu = suodatettu.Where(l => !l.Maksettu);
            }

            var lista = suodatettu.ToList();
            LaskuLista.ItemsSource = lista;
            TyhjaIlmoitus.IsVisible = lista.Count == 0;
        }

        private async void Navigoi_Clicked(object sender, EventArgs e) {
            var reitti = (sender as Button)?.CommandParameter as string;
            if (!string.IsNullOrEmpty(reitti))
                await NavigointiService.Navigoi(reitti);
        }
    }
}

