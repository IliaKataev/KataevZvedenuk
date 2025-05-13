using gos.services;

namespace gos
{
    public partial class Form1 : Form
    {
        private readonly IAuthService _authService;

        public Form1(IAuthService authService)
        {
            _authService = authService;
            InitializeComponent();
        }
    }
}
