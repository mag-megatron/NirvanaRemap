using LiteDB;
using System.ComponentModel;

namespace NirvanaMAUI.Models
{
    public class Profile : INotifyPropertyChanged
    {
        [BsonId]
        public string Name { get; set; } = string.Empty;

        private string _a = "A";
        public string A
        {
            get => _a;
            set { _a = value; OnPropertyChanged(nameof(A)); }
        }

        private string _b = "B";
        public string B
        {
            get => _b;
            set { _b = value; OnPropertyChanged(nameof(B)); }
        }

        private string _x = "X";
        public string X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        private string _y = "Y";
        public string Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        private string _lb = "LB";
        public string LB
        {
            get => _lb;
            set { _lb = value; OnPropertyChanged(nameof(LB)); }
        }

        private string _rb = "RB";
        public string RB
        {
            get => _rb;
            set { _rb = value; OnPropertyChanged(nameof(RB)); }
        }

        private string _lt = "LT";
        public string LT
        {
            get => _lt;
            set { _lt = value; OnPropertyChanged(nameof(LT)); }
        }

        private string _rt = "RT";
        public string RT
        {
            get => _rt;
            set { _rt = value; OnPropertyChanged(nameof(RT)); }
        }

        private string _dLeft = "DLeft";
        public string DLeft
        {
            get => _dLeft;
            set { _dLeft = value; OnPropertyChanged(nameof(DLeft)); }
        }

        private string _dRight = "DRight";
        public string DRight
        {
            get => _dRight;
            set { _dRight = value; OnPropertyChanged(nameof(DRight)); }
        }

        private string _dUp = "DUp";
        public string DUp
        {
            get => _dUp;
            set { _dUp = value; OnPropertyChanged(nameof(DUp)); }
        }

        private string _dDown = "DDown";
        public string DDown
        {
            get => _dDown;
            set { _dDown = value; OnPropertyChanged(nameof(DDown)); }
        }

        private string _l3 = "L3";
        public string L3
        {
            get => _l3;
            set { _l3 = value; OnPropertyChanged(nameof(L3)); }
        }

        private string _r3 = "R3";
        public string R3
        {
            get => _r3;
            set { _r3 = value; OnPropertyChanged(nameof(R3)); }
        }

        private string _view = "View";
        public string View
        {
            get => _view;
            set { _view = value; OnPropertyChanged(nameof(View)); }
        }

        private string _menu = "Menu";
        public string Menu
        {
            get => _menu;
            set { _menu = value; OnPropertyChanged(nameof(Menu)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        /// <summary>
        /// Converte este <see cref="Profile"/> para uma instância de
        /// <see cref="Core.Services.ProfileMapping"/> utilizada pelo motor
        /// de remapeamento.
        /// </summary>
        /// <returns>Objeto <see cref="Core.Services.ProfileMapping"/> configurado.</returns>
        public Core.Services.ProfileMapping ToProfileMapping()
        {
            var map = new Core.Services.ProfileMapping();

            static string Map(string name) => name switch
            {
                "A" => "ButtonA",
                "B" => "ButtonB",
                "X" => "ButtonX",
                "Y" => "ButtonY",
                "LB" => "ButtonLeftShoulder",
                "RB" => "ButtonRightShoulder",
                "LT" => "TriggerLeft",
                "RT" => "TriggerRight",
                "L3" => "ThumbLPressed",
                "R3" => "ThumbRPressed",
                "DLeft" => "DPadLeft",
                "DRight" => "DPadRight",
                "DUp" => "DPadUp",
                "DDown" => "DPadDown",
                "View" => "ButtonBack",
                "Menu" => "ButtonStart",
                _ => name
            };

            map.AddMapping(new Core.Services.SimpleMapping("ButtonA", Map(A)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonB", Map(B)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonX", Map(X)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonY", Map(Y)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonLeftShoulder", Map(LB)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonRightShoulder", Map(RB)));
            map.AddMapping(new Core.Services.SimpleMapping("TriggerLeft", Map(LT)));
            map.AddMapping(new Core.Services.SimpleMapping("TriggerRight", Map(RT)));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbLPressed", Map(L3)));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbRPressed", Map(R3)));
            map.AddMapping(new Core.Services.SimpleMapping("DPadLeft", Map(DLeft)));
            map.AddMapping(new Core.Services.SimpleMapping("DPadRight", Map(DRight)));
            map.AddMapping(new Core.Services.SimpleMapping("DPadUp", Map(DUp)));
            map.AddMapping(new Core.Services.SimpleMapping("DPadDown", Map(DDown)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonBack", Map(View)));
            map.AddMapping(new Core.Services.SimpleMapping("ButtonStart", Map(Menu)));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbLX", "ThumbLX"));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbLY", "ThumbLY"));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbRX", "ThumbRX"));
            map.AddMapping(new Core.Services.SimpleMapping("ThumbRY", "ThumbRY"));

            return map;
        }
    }

}
