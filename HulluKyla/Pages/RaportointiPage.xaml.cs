using Microsoft.Maui.Controls;
using HulluKyla.Services;
using HulluKyla.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HulluKyla.Pages;

public partial class RaportointiPage : ContentPage {

    private List<Alue>? kaikkiAlueet;

    public RaportointiPage() {
        InitializeComponent();
    }

    // Sivun p�ivitysmetodi
    protected override void OnAppearing() {
        base.OnAppearing();
        PaivitaSivu();    
    }

    // Sivun p�ivitystoiminnot
    private void PaivitaSivu() {
        AluePicker.SelectedItem = null;
        VuosiPicker.SelectedItem = null;
        TuottoLista.ItemsSource = null;
        VuosiPicker.Items.Clear();

        AlkuPvmPicker.Date = DateTime.Now;
        LoppuPvmPicker.Date = DateTime.Now;

        VuosiYhteensaLabel.Text = "";
        MokkienTuottoLabel.Text = "";
        PalveluidenTuottoLabel.Text = "";
        TuototYhteensaLabel.Text = "";


        kaikkiAlueet = AlueService.HaeKaikki();
        AluePicker.ItemsSource = kaikkiAlueet;

        int nykyvuosi = DateTime.Now.Year;
        for (int v = 2020; v <= nykyvuosi; v++) {
            VuosiPicker.Items.Add(v.ToString());
        }
    }

    private async void Navigoi_Clicked(object sender, EventArgs e) {
        if (sender is Button btn && btn.CommandParameter is string target)
            await NavigointiService.Navigoi(target);
    }

    // Valitun vuoden yhteistulot varauksista kuukausittain eroteltuna
    private async void VuosiSelected(object sender, EventArgs e) {

        if (VuosiPicker.SelectedItem is not string valittuVuosiStr || !int.TryParse(valittuVuosiStr, out int valittuVuosi)) {
            return;
        }

        try {
            var kuukausiTuotot = RaportointiService.LaskeVuodenKokonaistuotto(valittuVuosi);

            if (kuukausiTuotot.Count == 0) {
                await DisplayAlert("Ei tuloksia", "Valitulle vuodelle ei l�ytynyt tuottoja.", "OK");
            }

            TuottoLista.ItemsSource = kuukausiTuotot.Select(k => new {
                Kuukausi = new DateTime(valittuVuosi, k.Kuukausi, 1).ToString("MMMM"),
                Tuotto = k.Tuotto
            }).ToList();

            double yhteensa = kuukausiTuotot.Sum(t => t.Tuotto);
            VuosiYhteensaLabel.Text = $"Yhteens�: {yhteensa:F2} �";
        }
        catch (Exception ex) {
            await DisplayAlert("Virhe", $"Vuosituoton hakeminen ep�onnistui: {ex.Message}", "OK");
        }
    }

    // M�kkien ja palveluiden tuoton laskenta valitulla aikav�lill� ja alueella
    // (jos ei valitse aluetta, niin etsii kaikkien alueiden tuoton kyseisell� aikav�lill�)
    private async void LaskeTuottoClicked(object sender, EventArgs e) {
        var alku = AlkuPvmPicker.Date;
        var loppu = LoppuPvmPicker.Date;

        if (alku > loppu) {
            await DisplayAlert("Virhe", "Alkup�iv�m��r� ei voi olla loppup�iv�m��r�n j�lkeen.", "OK");
            return;
        }

        try {
            uint? alueId = null;
            if (AluePicker.SelectedItem is Alue valittuAlue) {
                alueId = valittuAlue.AlueId;
            }

            double mokkiTuotto = RaportointiService.LaskeMokkienTuotto(alku, loppu, alueId);
            double palveluTuotto = RaportointiService.LaskePalveluidenTuotto(alku, loppu, alueId);
            double yhteensa = mokkiTuotto + palveluTuotto;

            MokkienTuottoLabel.Text = $"M�kkien tuotto: {mokkiTuotto:F2} �";
            PalveluidenTuottoLabel.Text = $"Palveluiden tuotto: {palveluTuotto:F2} �";
            TuototYhteensaLabel.Text = $"Yhteens�: {yhteensa:F2} �";
        }
        catch (Exception ex) {
            await DisplayAlert("Virhe", $"Tuottojen laskeminen ep�onnistui: {ex.Message}", "OK");
        }
    }
}
