using Microsoft.Maui.Controls;
using HulluKyla.Services;

namespace HulluKyla.Pages;

public partial class Etusivu : ContentPage {
    public Etusivu() {
        InitializeComponent();
    }

    private async void Navigoi_Clicked(object sender, EventArgs e) {
        if (sender is Button btn && btn.CommandParameter is string target)
            await NavigointiService.Navigoi(target);
    }

    private void Tyhja_Clicked(object sender, EventArgs e) {
        // Ei määritettyä logiikkaa
    }
}
