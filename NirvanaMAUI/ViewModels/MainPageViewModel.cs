using NirvanaMAUI.Models;
using NirvanaMAUI.Services;
using Core.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace NirvanaMAUI.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly ProfileStorageService _storage = new();
    private readonly GamepadRemapService _remapService;

    public ICommand ProfileActionCommand { get; }

    private ObservableCollection<Profile> _profiles;
    public ReadOnlyObservableCollection<Profile> Profiles { get; }
    public ObservableCollection<InputStatus> CurrentInputs { get; }
    public ObservableCollection<InputStatus> CurrentOutputs { get; }

    private bool _gamepadConnected;
    public bool GamepadConnected
    {
        get => _gamepadConnected;
        set
        {
            if (_gamepadConnected != value)
            {
                _gamepadConnected = value;
                OnPropertyChanged(nameof(GamepadConnected));
            }
        }
    }

    private Profile _selectedProfile = null!;
    public Profile SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (_selectedProfile != value)
            {
                if (_selectedProfile != null)
                    _selectedProfile.IsSelected = false;

                _selectedProfile = value;

                if (_selectedProfile != null)
                    _selectedProfile.IsSelected = true;

                OnPropertyChanged(nameof(SelectedProfile));
                ProfileSelected?.Invoke(value);
            }
        }
    }

    public MainPageViewModel(GamepadRemapService remapService)
    {
        _remapService = remapService;
        ProfileActionCommand = new Command<Profile>(ExecuteProfileAction);

        var loadedProfiles = _storage.LoadProfiles();

        if (!loadedProfiles.Any())
        {
            var defaultProfile = new Profile
            {
                Name = "Padrão",
                A = "A",
                B = "B",
                X = "X",
                Y = "Y",
                LB = "LB",
                RB = "RB",
                LT = "LT",
                RT = "RT",
                DLeft = "DLeft",
                DRight = "DRight",
                DUp = "DUp",
                DDown = "DDown",
                L3 = "L3",
                R3 = "R3",
                View = "View",
                Menu = "Menu",
                IsSelected = true
            };

            _profiles = new ObservableCollection<Profile> { defaultProfile };
            Profiles = new ReadOnlyObservableCollection<Profile>(_profiles);
            SelectedProfile = defaultProfile;
        }
        else
        {
            var ordered = loadedProfiles.OrderBy(p => p.Name != "Padrão").ToList();

            _profiles = new ObservableCollection<Profile>(ordered);
            Profiles = new ReadOnlyObservableCollection<Profile>(_profiles);

            var selected = Profiles.FirstOrDefault(p => p.IsSelected) ?? Profiles.First();
            selected.IsSelected = true;
            SelectedProfile = selected;
        }

        var inputNames = typeof(GamepadState)
            .GetProperties()
            .Select(p => p.Name)
            .ToArray();

        CurrentInputs = new ObservableCollection<InputStatus>(inputNames.Select(n => new InputStatus(n)));
        CurrentOutputs = new ObservableCollection<InputStatus>(inputNames.Select(n => new InputStatus(n)));
        _remapService.InputReceived += UpdateInput;
        _remapService.GamepadConnectionChanged += connected => GamepadConnected = connected;
        GamepadConnected = _remapService.IsGamepadConnected;
    }

    public void UpdateInput(string name, float value)
    {
        var status = CurrentInputs.FirstOrDefault(i => i.Name == name);
        if (status != null)
            status.Value = value;
    }
    public void UpdateOutput(string name, float value)
    {
        var status = CurrentOutputs.FirstOrDefault(i => i.Name == name);
        if (status != null)
            status.Value = value;
    }

    private void ExecuteProfileAction(Profile profile)
    {
        if (profile != Profiles.First())
        {
            _profiles.Remove(profile);
            SelectedProfile = Profiles.FirstOrDefault()!;
        }
        else
        {
            AddNewProfile();
        }
    }

    public void AddNewProfile()
    {
        var newProfile = new Profile
        {
            Name = $"Novo Perfil {Profiles.Count + 1}",
            A = "A",
            B = "B",
            X = "X",
            Y = "Y",
            LB = "LB",
            RB = "RB",
            LT = "LT",
            RT = "RT",
            DLeft = "DLeft",
            DRight = "DRight",
            DUp = "DUp",
            DDown = "DDown",
            L3 = "L3",
            R3 = "R3",
            View = "View",
            Menu = "Menu",
            IsSelected = false
        };

        foreach (var p in Profiles)
            p.IsSelected = false;

        newProfile.IsSelected = true;
        _profiles.Add(newProfile);
        SelectedProfile = newProfile;
    }

    public event Action<Profile>? ProfileSelected;
    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
