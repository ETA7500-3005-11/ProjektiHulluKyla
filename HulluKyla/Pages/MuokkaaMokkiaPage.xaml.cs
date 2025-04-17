using Microsoft.Maui.Controls;
using HulluKyla.Services;

namespace HulluKyla.Pages;

public partial class MuokkaaMokkiaPage : ContentPage {
    public MuokkaaMokkiaPage() {
        InitializeComponent();
    }

    private async void Navigoi_Clicked(object sender, EventArgs e) {
        if (sender is Button btn && btn.CommandParameter is string target)
            await NavigointiService.Navigoi(target);
    }

    private void Tallenna_Clicked(object sender, EventArgs e) {
        // TODO: P�ivit� m�kin tiedot SQL:��n valitun M�kkiID:n perusteella
    }

}
