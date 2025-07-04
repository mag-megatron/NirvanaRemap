using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using NirvanaMAUI.Services;
using NirvanaMAUI.ViewModels;
using System.Runtime.InteropServices;
using Profile = NirvanaMAUI.Models.Profile;

namespace NirvanaMAUI;

public partial class MainPage : ContentPage
{
    private readonly GamepadRemapService _remapService;
    private readonly MainPageViewModel _vm;
    private readonly ProfileStorageService _storage = new();

    public MainPage(MainPageViewModel vm, GamepadRemapService remapService)
    {
        InitializeComponent();
        _vm = vm;
        _remapService = remapService;
        BindingContext = _vm;

        _remapService.InputReceived += (name, value) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
                _vm.UpdateInput(name, value)
            );
        };
        _remapService.OutputApplied += (name, value) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
                _vm.UpdateOutput(name, value)
            );
        };
        _remapService.GamepadConnectionChanged += connected =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
                _vm.GamepadConnected = connected
            );
        };
    }

    private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _vm.ProfileSelected += p =>
        {
            ApplyProfileMapping(p);
            _remapService.Start(p);
        };

        if (_vm.SelectedProfile != null)
        {
            ApplyProfileMapping(_vm.SelectedProfile);
            _remapService.Start(_vm.SelectedProfile);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
    }

    private void ApplyProfileMapping(Profile? profile)
    {
        if (profile == null || AButton == null) return;

        AButton.Text = profile.A;
        BButton.Text = profile.B;
        XButton.Text = profile.X;
        YButton.Text = profile.Y;

        LBButton.Text = profile.LB;
        RBButton.Text = profile.RB;

        LTButton.Text = profile.LT;
        RTButton.Text = profile.RT;

        L3Button.Text = profile.L3;
        R3Button.Text = profile.R3;

        DLeftButton.Text = profile.DLeft;
        DRightButton.Text = profile.DRight;
        DUpButton.Text = profile.DUp;
        DDownButton.Text = profile.DDown;

        ViewButton.Text = profile.View;
        MenuButton.Text = profile.Menu;

        _remapService.Start(profile);
    }

    private async void OnMappingButtonClicked(object sender, EventArgs e)
    {
        if (ViewModel.SelectedProfile == null || sender is not Button button)
            return;

        string[] options = new[]
      {
            "A", "B", "X", "Y",
            "LB", "RB", "LT", "RT",
            "L3", "R3",
            "DLeft", "DRight", "DUp", "DDown",
            "View", "Menu"
        };

        string selected = await DisplayActionSheet("Selecione um input", "Cancelar", null, options);

        if (string.IsNullOrEmpty(selected) || selected == "Cancelar") return;

        var profile = ViewModel.SelectedProfile;

        switch (button)
        {
            case var _ when button == AButton: profile.A = selected; break;
            case var _ when button == BButton: profile.B = selected; break;
            case var _ when button == XButton: profile.X = selected; break;
            case var _ when button == YButton: profile.Y = selected; break;

            case var _ when button == LBButton: profile.LB = selected; break;
            case var _ when button == RBButton: profile.RB = selected; break;

            case var _ when button == LTButton: profile.LT = selected; break;
            case var _ when button == RTButton: profile.RT = selected; break;

            case var _ when button == L3Button: profile.L3 = selected; break;
            case var _ when button == R3Button: profile.R3 = selected; break;

            case var _ when button == DLeftButton: profile.DLeft = selected; break;
            case var _ when button == DRightButton: profile.DRight = selected; break;
            case var _ when button == DUpButton: profile.DUp = selected; break;
            case var _ when button == DDownButton: profile.DDown = selected; break;

            case var _ when button == ViewButton: profile.View = selected; break;
            case var _ when button == MenuButton: profile.Menu = selected; break;

            default: return;
        }

        bool saved = _storage.SaveProfiles(ViewModel.Profiles);
        if (!saved)
            await SafeSnackbarAsync("Erro ao salvar perfil", async () =>
            {
                bool retry = _storage.SaveProfiles(ViewModel.Profiles);
                if (retry)
                    await SafeToastAsync("Salvo na segunda tentativa.");
            }, "Tentar novamente");

        _remapService.Restart();
        ApplyProfileMapping(ViewModel.SelectedProfile);
    }


    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        bool success = _storage.SaveProfiles(ViewModel.Profiles);

        if (success)
            await SafeToastAsync("Alterações salvas com sucesso");
        else
            await ShowSaveErrorSnackbar();
    }

    private async Task SafeToastAsync(string text, ToastDuration duration = ToastDuration.Short, double textSize = 14)
    {
        try
        {
            await Toast.Make(text, duration, textSize).Show();
        }
        catch (COMException)
        {
            await DisplayAlert(string.Empty, text, "OK");
        }
    }

    private async void OnAddProfileClicked(object sender, EventArgs e)
    {
        ViewModel.AddNewProfile();
        bool success = _storage.SaveProfiles(ViewModel.Profiles);

        if (success)
            await SafeToastAsync("Perfil salvo com sucesso");
        else
            await ShowSaveErrorSnackbar();
    }

    private async void OnResetProfilesClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Resetar Perfis",
            "Tem certeza que deseja apagar todos os perfis e restaurar para o padrão?",
            "Sim", "Cancelar");

        if (!confirmar) return;

        _storage.Reset();

        if (Microsoft.Maui.Controls.Application.Current?.Windows.Count > 0)
        {
            Microsoft.Maui.Controls.Application.Current.Windows[0].Page = new AppShell();
        }
    }

    private async Task SafeSnackbarAsync(
        string message,
        Func<Task>? action = null,
        string actionText = "OK",
        TimeSpan? duration = null)
    {
        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.DarkRed,
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.LightBlue,
            CornerRadius = 10,
            Font = Microsoft.Maui.Font.SystemFontOfSize(14)
        };

        try
        {
            var snackbar = Snackbar.Make(
                message,
                async () =>
                {
                    if (action is not null)
                        await action.Invoke();
                },
                actionText,
                duration ?? TimeSpan.FromSeconds(5),
                snackbarOptions
            );

            await snackbar.Show();
        }
        catch (COMException)
        {
            await DisplayAlert("Aviso", message, actionText);
            if (action is not null)
                await action.Invoke();
        }
    }

    private async Task ShowSaveErrorSnackbar()
    {
        await SafeSnackbarAsync(
            "Erro ao salvar perfil",
            async () =>
            {
                bool retrySuccess = _storage.SaveProfiles(ViewModel.Profiles);
                if (retrySuccess)
                    await SafeToastAsync("Salvo com sucesso na segunda tentativa");
            },
            "Tentar novamente"
        );
    }
}
