using Microsoft.Maui.Controls;
using HulluKyla.Services;
using HulluKyla.Models;

namespace HulluKyla.Pages;

public partial class UusiAluePage : ContentPage {
    public UusiAluePage() {
        InitializeComponent();
    }

    // Sivun p�ivitystoiminnot, kun sivu tulee esiin
    protected override void OnAppearing() {
        base.OnAppearing();
        TyhjennaKentat();
    }

    private async void Navigoi_Clicked(object sender, EventArgs e) {
        if (sender is Button btn && btn.CommandParameter is string target)
            await NavigointiService.Navigoi(target);
    }

    private async void LisaaClicked(object sender, EventArgs e) {
        try {
            var uusiAlue = new Alue(
                NimiEntry.Text
            );

            AlueService.Lisaa(uusiAlue);

            await DisplayAlert("Onnistui", "Alue lis�tty.", "OK");
            TyhjennaKentat();
            await NavigointiService.Navigoi("AlueListaPage");
        }
        catch (Exception ex) {
            await DisplayAlert("Virhe", $"Aluetta lis�tt�ess� tapahtui virhe: {ex.Message}", "OK");
        }
    }

    // Kenttien tyhjennys
    private void TyhjennaKentat() {
        NimiEntry.Text = string.Empty;
    }
}
