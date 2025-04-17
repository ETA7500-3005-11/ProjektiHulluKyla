using Microsoft.Maui.Controls;
using HulluKyla.Services;

namespace HulluKyla.Pages;

public partial class MokkiListaPage : ContentPage {
    public MokkiListaPage() {
        InitializeComponent();
    }

    private async void Navigoi_Clicked(object sender, EventArgs e) {
        if (sender is Button btn && btn.CommandParameter is string target) {
            await NavigointiService.Navigoi(target);
        }
    }
}
